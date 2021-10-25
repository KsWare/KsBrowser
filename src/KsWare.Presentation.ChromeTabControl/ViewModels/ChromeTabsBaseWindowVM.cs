using System;
using System.ComponentModel;
using System.Diagnostics;
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
		public ActionVM ReorderTabsAction { get; [UsedImplicitly] private set; }
		public ActionVM PinTabAction { get; [UsedImplicitly] private set; }

		protected virtual void DoAddNewTab(object parameter) {
		}

		protected virtual void DoCloseTab(object parameter) {
			if(parameter is ChromeTabItemVM vm) CloseTabItem(vm);
			else Debug.WriteLine($"DoCloseTab {parameter}"); // {{DisconnectedItem}}
		}

		protected virtual void DoReorderTabs() {
		}

		protected virtual void DoPinTab(object parameter) {
		}

		protected virtual void OnLastTabItemClosed() {
			// A) close window
			// this.Close();
			// B) add empty tab
			// DoNewTab();
		}

		/// <inheritdoc />
		// implements IProvideChromeTabHostVM.TabHost
		public virtual IChromeTabHostVM TabHost => this;
	}

}