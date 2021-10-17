using System;

namespace KsWare.Presentation.ViewModels {

	public class TabItemClosingEventArgs : TabItemEventArgs {

		public TabItemClosingEventArgs(ChromeTabItemVM tabItem) : base(tabItem) {
		}

		public bool Cancel { get; set; }
	}

}