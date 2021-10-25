using System;

namespace KsWare.Presentation.ViewModels {

	public interface IChromeTabHostVM {

		void AddNewTabItem(ITabItemCreationOptions options);
		void AddTabItem(ChromeTabItemVM tabItem, int newPosition, IChromeTabHostVM oldTabHost);
		void RemoveTabItem(ChromeTabItemVM tabItem);
		bool CloseTabItem(ChromeTabItemVM tabItem);
		void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabHostVM newHost);
		int CountTabItems { get; }
		IEventSource<EventHandler<TabItemCreatedEventArgs>> TabItemCreatedEvent { get; }
		IEventSource<EventHandler<TabItemClosingEventArgs>> TabItemClosingEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemRemovedEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemClosedEvent { get; }
		IEventSource<EventHandler<TabItemAddedEventArgs>> TabItemAddedEvent { get; }
	}

}