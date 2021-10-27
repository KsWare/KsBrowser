using System;
using System.Collections;
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
	public partial class ChromeTabsBaseWindowVM : IChromeTabsHostVM {

		private void InitIChromeTabHostVM() {
			TabItems.PropertyChanged += (s, e) => {
				if (e.PropertyName == nameof(TabItems.Count)) OnPropertyChanged(nameof(CountTabItems));
			};
		}

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
		/// Implements <see cref="IChromeTabsHostVM.AddNewTabItem"/>
		public virtual void AddNewTabItem(ITabItemCreationOptions options = null) {
			options ??= new TabItemCreationOptions();
			var newTab = options.GetInstance();
			OnTabItemCreated(newTab, options);
			
			if (options is TabItemCreationOptions o) {
				if (o.InsertPosition == -1) TabItems.Add(newTab);
				else if (o.InsertPosition < 0) TabItems.Insert(Math.Max(TabItems.Count + 1 - o.InsertPosition, 0), newTab);
				else TabItems.Insert(Math.Max(o.InsertPosition, TabItems.Count), newTab);

				if (o.Activate) CurrentTabItem = newTab;
			}
			else {
				TabItems.Add(newTab);
				CurrentTabItem = newTab;
			}
		}

		/// <inheritdoc />
		IEnumerable IChromeTabsHostVM.TabItems => TabItems;

		/// <inheritdoc />
		[Bindable(true)]
		public int CountTabItems => TabItems.Count;

		/// <inheritdoc />
		public void AddTabItem(ChromeTabItemVM tabItem, int position, IChromeTabsHostVM oldHost) {
			if (position > TabItems.Count || position < 0) position = TabItems.Count;
			TabItems.Insert(position,tabItem);
			CurrentTabItem = tabItem;
		}

		/// <inheritdoc />
		public void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabsHostVM newHost) {
			var idx = TabItems.IndexOf(tabItem);
			CurrentTabItem = null;
			TabItems.RemoveAt(idx);
			SelectTabItem(idx);
			if (newHost != null) newHost.AddTabItem(tabItem, -1, this);
		}

		/// <inheritdoc />
		public void RemoveTabItem(ChromeTabItemVM tabItem) {
			if (tabItem.TabHost == this) tabItem.TabHost = null;
			TabItems.Remove(tabItem);
		}

		/// <inheritdoc />
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
	}
}
