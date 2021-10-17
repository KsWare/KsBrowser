using System;

namespace KsWare.Presentation.ViewModels {

	public class TabItemCreatedEventArgs : TabItemEventArgs {

		public TabItemCreatedEventArgs(ChromeTabItemVM tabItem) :base(tabItem) {
		}

		public TabItemCreatedEventArgs(ChromeTabItemVM tabItem, ITabItemCreationOptions options) : base(tabItem) {
			Options = options;
		}

		public ITabItemCreationOptions Options { get; }
	}

}