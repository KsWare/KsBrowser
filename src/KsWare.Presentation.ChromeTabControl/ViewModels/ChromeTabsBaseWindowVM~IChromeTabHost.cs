using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.Presentation;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace KsWare.Presentation.ViewModels {

	// This part implements the IChromeTabHostVM members
	public partial class ChromeTabsBaseWindowVM /*: IChromeTabHostVM*/ {

		#region events

		/// <inheritdoc />
		public IEventSource<EventHandler<TabItemCreatedEventArgs>> TabItemCreatedEvent =>
			LazyWeakEventStore.Value.Get<EventHandler<TabItemCreatedEventArgs>>();

		/// <inheritdoc />
		public IEventSource<EventHandler<TabItemClosingEventArgs>> TabItemClosingEvent =>
			LazyWeakEventStore.Value.Get<EventHandler<TabItemClosingEventArgs>>();

		/// <inheritdoc />
		public IEventSource<EventHandler<TabItemEventArgs>> TabItemClosedEvent =>
			LazyWeakEventStore.Value.Get<EventHandler<TabItemEventArgs>>();

		/// <inheritdoc />
		public IEventSource<EventHandler<TabItemEventArgs>> TabItemRemovedEvent =>
			LazyWeakEventStore.Value.Get<EventHandler<TabItemEventArgs>>();

		/// <inheritdoc />
		public IEventSource<EventHandler<TabItemAddedEventArgs>> TabItemAddedEvent =>
			LazyWeakEventStore.Value.Get<EventHandler<TabItemAddedEventArgs>>();

		#endregion

		/// <inheritdoc />
		/// Implements <see cref="IChromeTabHostVM.AddNewTabItem"/>
		public virtual void AddNewTabItem(ITabItemCreationOptions options) {
			var newTab = new ChromeTabItemVM();
			OnTabItemCreated(newTab, options);
			TabItems.Add(newTab);
			CurrentTabItem = newTab;
		}

		public virtual bool CloseTabItem(ChromeTabItemVM tabItem) {
			var cancelEventArgs = new CancelEventArgs();
			OnTabItemClosing(tabItem, cancelEventArgs);
			if(cancelEventArgs.Cancel) return false;
			var idx = TabItems.IndexOf(tabItem);
			TabItems.RemoveAt(idx);
			OnTabItemClosed(tabItem);
			if (idx == TabItems.Count) idx--;
			if (idx >= 0) CurrentTabItem = TabItems[idx];
			else OnLastTabItemClosed();
			return true;
		}
		
		/// <inheritdoc />
		public void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabHostVM newHost) {
			var idx = TabItems.IndexOf(tabItem);
			CurrentTabItem = null;
			TabItems.RemoveAt(idx);
			SelectTabItem(idx);
			if (newHost != null) newHost.AddTabItem(tabItem, -1, this);
		}

		/// <inheritdoc />
		public void AddTabItem(ChromeTabItemVM tabItem, int newPosition, IChromeTabHostVM oldHost) {
			if (newPosition > TabItems.Count || newPosition < 0) newPosition = TabItems.Count;
			TabItems.Insert(newPosition,tabItem);
			CurrentTabItem = tabItem;
		}

		[Bindable(false)]
		public int CountTabItems => TabItems.Count;

		/// <inheritdoc />
		public void RemoveTabItem(ChromeTabItemVM tabItem) {
			if (tabItem.TabHost == this) tabItem.TabHost = null;
			TabItems.Remove(tabItem);
		}
	}
}
