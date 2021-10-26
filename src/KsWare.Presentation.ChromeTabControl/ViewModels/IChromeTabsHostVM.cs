using System;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModels {

	public interface IChromeTabsHostVM {

		void AddNewTabItem(ITabItemCreationOptions options);
		void AddTabItem(ChromeTabItemVM tabItem, int newPosition, IChromeTabsHostVM oldTabHost);
		void RemoveTabItem(ChromeTabItemVM tabItem);
		bool CloseTabItem(ChromeTabItemVM tabItem);
		void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabsHostVM newHost);
		int CountTabItems { get; }
		IEventSource<EventHandler<TabItemCreatedEventArgs>> TabItemCreatedEvent { get; }
		IEventSource<EventHandler<TabItemClosingEventArgs>> TabItemClosingEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemRemovedEvent { get; }
		IEventSource<EventHandler<TabItemEventArgs>> TabItemClosedEvent { get; }
		IEventSource<EventHandler<TabItemAddedEventArgs>> TabItemAddedEvent { get; }

		ListVM<ChromeTabItemVM> TabItems { get; }
		ChromeTabItemVM CurrentTabItem { get; }
		bool AllowMoveTabs { get; set; }
		bool ShowAddButton { get; set; }
		ActionVM AddNewTabAction { get; }
		ActionVM CloseTabAction { get; }
		ActionVM ReorderTabsAction { get; }
		ActionVM PinTabAction { get; }

	}

}