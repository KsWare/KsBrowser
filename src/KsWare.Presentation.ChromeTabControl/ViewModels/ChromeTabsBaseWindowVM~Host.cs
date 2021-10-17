using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KsWare.Presentation.ViewModels {

	// partial for tab managing

	partial class ChromeTabsBaseWindowVM {

		private void InitChromeTabsHost() {
			TabItems.CollectionChangedEvent.add = (s, e) => {
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
						e.NewItems.OfType<ChromeTabItemVM>().ForEach(OnTabItemAdded);
						break;
					case NotifyCollectionChangedAction.Remove: 
						e.OldItems.OfType<ChromeTabItemVM>().ForEach(OnTabItemRemoved);
						break;
				}
			};
			Fields[nameof(CurrentTabItem)].ValueChangedEvent.add = (s, e) => {
				(e.OldValue as IHandleTabItemNotifications)?.NotifyDeactivated();
				(e.NewValue as IHandleTabItemNotifications)?.NotifyActivated();
			};
		}
		 
		public virtual void SelectTabItem(int idx) {
			if (idx >= TabItems.Count) CurrentTabItem = TabItems.LastOrDefault();
			else if (idx < 0) CurrentTabItem = TabItems.FirstOrDefault();
			else CurrentTabItem = TabItems[idx];
		}

		/// <summary>
		/// Raises the <see cref="TabItemCreatedEvent"/>.
		/// </summary>
		/// <param name="tabItem">The tab item that has been created.</param>
		/// <param name="options"></param>
		/// <remarks>
		/// This is called after a tab item as been created.
		/// </remarks>
		protected virtual void OnTabItemCreated(ChromeTabItemVM tabItem, ITabItemCreationOptions options) {
			(tabItem as IHandleTabItemNotifications)?.NotifyCreated(options);
			EventManager.Raise<EventHandler<TabItemCreatedEventArgs>, TabItemCreatedEventArgs>(LazyWeakEventStore,
				nameof(TabItemCreatedEvent),
				new Lazy<TabItemCreatedEventArgs>(() => new TabItemCreatedEventArgs(tabItem, options)));
		}

		/// <summary>
		/// Raises the <see cref="TabItemAddedEvent"/>.
		/// </summary>
		/// <param name="tabItem">The tab item that has been added.</param>
		/// <param name="options"></param>
		/// <remarks>
		/// This is called after a tab item as been added.
		/// </remarks>
		private void OnTabItemAdded(ChromeTabItemVM tabItem) {
			(tabItem as IHandleTabItemNotifications)?.NotifyAdded(this);
			EventManager.Raise<EventHandler<TabItemAddedEventArgs>, TabItemAddedEventArgs>(LazyWeakEventStore,
				nameof(TabItemAddedEvent),
				new Lazy<TabItemAddedEventArgs>(() => new TabItemAddedEventArgs(tabItem)));
		}

		/// <summary>
		/// Raises the <see cref="TabItemClosedEvent"/>.
		/// </summary>
		/// <param name="tabItem">The tab item that has been removed.</param>
		/// <remarks>
		/// This is called after a tab item has been removed.
		/// </remarks>
		protected virtual void OnTabItemRemoved(ChromeTabItemVM tabItem) {
			(tabItem as IHandleTabItemNotifications)?.NotifyRemoved();
			EventManager.Raise<EventHandler<TabItemEventArgs>, TabItemEventArgs>(LazyWeakEventStore,
				nameof(TabItemRemovedEvent),
				new Lazy<TabItemEventArgs>(() => new TabItemEventArgs(tabItem)));
		}

		/// <summary>
		/// Raises the <see cref="TabItemClosingEvent"/>.
		/// </summary>
		/// <param name="tabItem">The tab item that will be closed.</param>
		/// <param name="e">The <see cref="CancelEventArgs"/>.</param>
		/// <remarks>
		/// This is called before a tab item will be closed.
		/// </remarks>
		protected virtual void OnTabItemClosing(ChromeTabItemVM tabItem, CancelEventArgs e) {
			(tabItem as IHandleTabItemNotifications)?.NotifyClosing(e);
			TabItemClosingEventArgs e2 = null;
			EventManager.Raise<EventHandler<TabItemClosingEventArgs>, TabItemClosingEventArgs>(LazyWeakEventStore,
				nameof(TabItemClosingEvent),
				new Lazy<TabItemClosingEventArgs>(() => e2 = new TabItemClosingEventArgs(tabItem){Cancel = e.Cancel}));
			if (e2 != null) e.Cancel = e2.Cancel;
		}

		/// <summary>
		/// Raises the <see cref="TabItemClosedEvent"/>.
		/// </summary>
		/// <param name="tabItem">The tab item that has been closed.</param>
		/// <remarks>
		/// This is called after a tab item has been closed.
		/// </remarks>
		protected virtual void OnTabItemClosed(ChromeTabItemVM tabItem) {
			(tabItem as IHandleTabItemNotifications)?.NotifyClosed();
			EventManager.Raise<EventHandler<TabItemEventArgs>, TabItemEventArgs>(LazyWeakEventStore,
				nameof(TabItemClosedEvent),
				new Lazy<TabItemEventArgs>(() => new TabItemEventArgs(tabItem)));
		}


	}
}
