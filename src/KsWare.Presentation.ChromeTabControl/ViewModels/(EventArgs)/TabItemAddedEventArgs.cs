using System;

namespace KsWare.Presentation.ViewModels {

	public class TabItemAddedEventArgs : TabItemEventArgs {

		//TODO revise use of ITabItemCreationOptions

		public TabItemAddedEventArgs(ChromeTabItemVM tabItem) : base(tabItem) {
		}

		public TabItemAddedEventArgs(ChromeTabItemVM tabItem, ITabItemCreationOptions options):base(tabItem) {
			Options = options;
		}

		public ITabItemCreationOptions Options { get; }
	}

}