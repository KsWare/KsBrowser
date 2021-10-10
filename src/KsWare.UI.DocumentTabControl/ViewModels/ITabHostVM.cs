using System;

namespace KsWare.Presentation.Controls {

	public interface ITabHostVM {

		void CreateNewTab(ITabCreationOptions options);
		void CloseTab(TabItemVM tab);
		IEventSource<EventHandler<TabCreatedEventArgs>> TabCreatedEvent { get; }
	}

}