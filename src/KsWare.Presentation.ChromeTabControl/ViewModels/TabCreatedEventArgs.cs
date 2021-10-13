using System;

namespace KsWare.Presentation.ViewModels {

	public class TabCreatedEventArgs : EventArgs {

		public TabCreatedEventArgs(TabItemVM newTab) {
			NewTab = newTab;
		}

		public TabCreatedEventArgs(TabItemVM newTab, ITabCreationOptions options) {
			NewTab = newTab;
			Options = options;
		}

		public TabItemVM NewTab { get; }
		public ITabCreationOptions Options { get; }
	}

}