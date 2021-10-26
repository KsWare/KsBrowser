using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using ChromeTabs;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModels {

	public abstract partial class ChromeTabsBaseWindowVM : WindowVM, IChromeTabHostVM, IProvideTabHostVM {

		/// <inheritdoc />
		public ChromeTabsBaseWindowVM() {
			RegisterChildren(() => this);
			AllowMoveTabs = true;
			ShowAddButton = true;
			InitChromeTabsHost();
			TabItems.CollectionChanged += TabItems_CollectionChanged;

			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criteria.
			SortDescriptions.Add(new SortDescription(nameof(ChromeTabItemVM.IsPinned), ListSortDirection.Descending));
			SortDescriptions.Add(new SortDescription(nameof(ChromeTabItemVM.TabNumber), ListSortDirection.Ascending));
		}

		/// <summary>
		/// Gets the tab items.
		/// </summary>
		/// <value>The tab items.</value>
		public ListVM<ChromeTabItemVM> TabItems { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets or sets the current tab item.
		/// </summary>
		/// <value>The current tab item.</value>
		[Hierarchy(HierarchyType.Reference)]
		public ChromeTabItemVM CurrentTabItem { get => Fields.GetValue<ChromeTabItemVM>(); set => Fields.SetValue(value); }

		public bool AllowMoveTabs { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		public bool ShowAddButton { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		public ActionVM AddNewTabAction { get; [UsedImplicitly] private set; }
		public ActionVM CloseTabAction { get; [UsedImplicitly] private set; }
		public ActionVM ReorderTabsAction { get; [UsedImplicitly] private set; } // //since we don't know what kind of objects are bound, so the sorting happens outside with the ReorderTabsCommand.
		public ActionVM PinTabAction { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		// implements IProvideChromeTabHostVM.TabHost
		public virtual IChromeTabHostVM TabHost => this;

		protected SortDescriptionCollection SortDescriptions => CollectionViewSource.GetDefaultView(TabItems).SortDescriptions;

		[UsedImplicitly]
		protected virtual void DoAddNewTab(object parameter) {
		}

		[UsedImplicitly]
		protected virtual void DoCloseTab(object parameter) {
			if (parameter is ChromeTabItemVM vm) CloseTabItem(vm);
			else Debug.WriteLine($"DoCloseTab {parameter}"); // {{DisconnectedItem}}
		}

		[UsedImplicitly]
		protected virtual void DoReorderTabs(object parameter) {
			var reorder = (TabReorder)parameter;

			var view = CollectionViewSource.GetDefaultView(TabItems);
			var from = reorder.FromIndex;
			var to = reorder.ToIndex;
			var tabCollection = view.Cast<ChromeTabItemVM>().ToList(); //Get the ordered collection of our tab control

			tabCollection[from].TabNumber = tabCollection[to].TabNumber; //Set the new index of our dragged tab

			if (to > from) {
				for (var i = from + 1; i <= to; i++) {
					tabCollection[i].TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
				}
			}
			else if (from > to) { //when we decrement the tab index
				for (var i = to; i < from; i++) {
					tabCollection[i].TabNumber++; //When we decrement the tab index, we need to increment all other tabs.
				}
			}

			view.Refresh(); //Refresh the view to force the sort description to do its work.
		}

		[UsedImplicitly]
		protected virtual void DoPinTab(object parameter) {
			var tab = (ChromeTabItemVM)parameter;
			tab.IsPinned = !tab.IsPinned;
			var view = CollectionViewSource.GetDefaultView(TabItems);
			view.Refresh();
		}

		protected virtual void OnLastTabItemClosed() {
			// A) close window
			// this.Close();
			// B) add empty tab
			// DoNewTab();
		}

		//We need to set the TabNumber property on the viewmodels when the item source changes to keep it in sync.
		void TabItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add) {
				foreach (ChromeTabItemVM tab in e.NewItems) {
					if (TabItems.Count > 1) {
						//If the new tab don't have an existing number, we increment one to add it to the end.
						if (tab.TabNumber == 0)
							tab.TabNumber = TabItems.OrderBy(x => x.TabNumber).LastOrDefault().TabNumber + 1;
					}
				}
			}
			else {
				ICollectionView view = CollectionViewSource.GetDefaultView(TabItems);
				view.Refresh();
				var tabCollection = view.Cast<ChromeTabItemVM>().ToList();
				foreach (var item in tabCollection)
					item.TabNumber = tabCollection.IndexOf(item);
			}
		}

		
	}

}