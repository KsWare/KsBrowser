using System;

namespace KsWare.Presentation.ViewModels {

	public class TabItemEventArgs : EventArgs {

		public TabItemEventArgs(ChromeTabItemVM tabItem) {
			TabItem = tabItem;
		}

		public ChromeTabItemVM TabItem { get; }
	}

}