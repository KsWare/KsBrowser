using System;

namespace KsWare.Presentation.ViewModels {

	public interface IChromeTabHostVM {

		void AddNewTabItem(ITabItemCreationOptions options);
		bool CloseTabItem(ChromeTabItemVM tabItem);
		IEventSource<EventHandler<TabItemCreatedEventArgs>> TabItemCreatedEvent { get; }
		IEventSource<EventHandler<TabItemClosingEventArgs>> TabItemClosingEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemRemovedEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemClosedEvent { get; }
		IEventSource<EventHandler<TabItemAddedEventArgs>> TabItemAddedEvent { get; }
		void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabHostVM newHost);
		void AddTabItem(ChromeTabItemVM tabItem, int newPosition, IChromeTabHostVM oldTabHost);
	}

}