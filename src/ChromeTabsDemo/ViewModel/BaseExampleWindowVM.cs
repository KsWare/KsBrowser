// ORIGINAL ChromeTabsDemo\ViewModel\ViewModelExampleBase.cs
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ChromeTabs;
using Demo.Properties;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModels;

namespace Demo.ViewModel {

	public class BaseExampleWindowVM : ObjectVM {

		/// <inheritdoc />
		public BaseExampleWindowVM() {
			ItemCollection = new ObservableCollection<ChromeTabItemVM>();
			ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
			ReorderTabsCommand = new RelayCommand(ReorderTabsCommandAction);
			AddTabCommand = new RelayCommand(AddTabCommandAction, () => CanAddTabs);
			CloseTabCommand = new RelayCommand(CloseTabCommandAction);
			CanAddTabs = true;

			Fields[nameof(CanAddTabs)].ValueChangedEvent.add = (s, e) => AddTabCommand.OnCanExecuteChanged();
		}

		//since we don't know what kind of objects are bound, so the sorting happens outside with the ReorderTabsCommand.
		public RelayCommand ReorderTabsCommand { get; }
		public RelayCommand AddTabCommand { get; }
		public RelayCommand CloseTabCommand { get; }
		public ObservableCollection<ChromeTabItemVM> ItemCollection { get; }

		//This is the current selected tab, if you change it, the tab is selected in the tab control.
		public ChromeTabItemVM SelectedTab { get => Fields.GetValue<ChromeTabItemVM>(); set => Fields.SetValue(value); }

		public bool CanAddTabs {
			get => Fields.GetValue<bool>();
			set => Fields.SetValue(value);
		}


		protected Class1TabItemVM CreateTab1() {
			var tab = new Class1TabItemVM {
				Title = "Tab class 1", MyStringContent = "Try drag the tab from left to right",
				TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative))
			};
			return tab;
		}

		protected Class2TabItemVM CreateTab2() {
			var tab = new Class2TabItemVM {
				Title = "Tab class 2, with a long name",
				MyStringContent = "Try drag the tab outside the bonds of the tab control",
				MyNumberCollection = new[] { 1, 2, 3, 4 }, MySelectedNumber = 1,
				TabIcon = new BitmapImage(new Uri("/Resources/2.png", UriKind.Relative))
			};
			return tab;

		}

		protected Class3TabItemVM CreateTab3() {
			var tab = new Class3TabItemVM {
				Title = "Tab class 3",
				MyStringContent =
					"Try right clicking on the tab header. This tab can not be dragged out to a new window, to demonstrate that you can dynamically choose what tabs can, based on the viewmodel.",
				MyImageUrl = new Uri("/Resources/Kitten.jpg", UriKind.Relative),
				TabIcon = new BitmapImage(new Uri("/Resources/3.png", UriKind.Relative))
			};
			return tab;
		}

		protected Class4TabItemVM CreateTab4() {
			var tab = new Class4TabItemVM {
				Title = "Tab class 4", MyStringContent = "This tab demonstrates a custom tab header implementation",
				IsBlinking = true
			};
			return tab;
		}

		protected Class1TabItemVM CreateTabLoremIpsum() {
			var tab = new Class1TabItemVM {
				Title = "Tab class 1", MyStringContent = Resources.LoremIpsum,
				TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative))
			};
			return tab;
		}

		/// <summary>
		/// Reorder the tabs and refresh collection sorting.
		/// </summary>
		/// <param name="parameter">The <see cref="TabReorder"/>.</param>
		protected virtual void ReorderTabsCommandAction(object parameter) {
			TabReorder reorder = (TabReorder)parameter;

			ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
			int from = reorder.FromIndex;
			int to = reorder.ToIndex;
			var tabCollection = view.Cast<ChromeTabItemVM>().ToList(); //Get the ordered collection of our tab control

			tabCollection[from].TabNumber = tabCollection[to].TabNumber; //Set the new index of our dragged tab

			if (to > from) {
				for (int i = from + 1; i <= to; i++) {
					tabCollection[i]
						.TabNumber--; //When we increment the tab index, we need to decrement all other tabs.
				}
			}
			else if (from > to) //when we decrement the tab index
			{
				for (int i = to; i < from; i++) {
					tabCollection[i]
						.TabNumber++; //When we decrement the tab index, we need to increment all other tabs.
				}
			}

			view.Refresh(); //Refresh the view to force the sort description to do its work.
		}

		//We need to set the TabNumber property on the viewmodels when the item source changes to keep it in sync.
		void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add) {
				foreach (ChromeTabItemVM tab in e.NewItems) {
					if (ItemCollection.Count > 1) {
						//If the new tab don't have an existing number, we increment one to add it to the end.
						if (tab.TabNumber == 0)
							tab.TabNumber = ItemCollection.OrderBy(x => x.TabNumber).LastOrDefault().TabNumber + 1;
					}
				}
			}
			else {
				ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
				view.Refresh();
				var tabCollection = view.Cast<ChromeTabItemVM>().ToList();
				foreach (var item in tabCollection)
					item.TabNumber = tabCollection.IndexOf(item);
			}
		}

		//To close a tab, we simply remove the viewmodel from the source collection.
		private void CloseTabCommandAction(object parameter) {
			ChromeTabItemVM vm = (ChromeTabItemVM)parameter;
			ItemCollection.Remove(vm);
		}

		//Adds a random tab
		private void AddTabCommandAction() {
			Random r = new Random();
			int num = r.Next(1, 100);
			if (num < 33)
				ItemCollection.Add(CreateTab1());
			else if (num < 66)
				ItemCollection.Add(CreateTab2());
			else
				ItemCollection.Add(CreateTab3());
		}
	}

}
