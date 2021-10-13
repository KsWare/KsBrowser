using System;

namespace KsWare.Presentation.ViewModels {

	public interface ITabHostVM {

		void CreateNewTab(ITabCreationOptions options);
		void CloseTab(ChromeTabItemVM tab);
		IEventSource<EventHandler<TabCreatedEventArgs>> TabCreatedEvent { get; }
	}

}