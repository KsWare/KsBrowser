using System;
using System.Collections;
using System.Collections.Generic;
using ChromeTabs;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModels {

	/// <summary>
	/// Interface IChromeTabsHostVM
	/// </summary>
	public interface IChromeTabsHostVM {

		/// <summary>
		/// Creates and add a new tab item.
		/// </summary>
		/// <param name="options">The options.</param>
		void AddNewTabItem(ITabItemCreationOptions options = null);

		/// <summary>
		/// Adds the tab item.
		/// </summary>
		/// <param name="tabItem">The tab item.</param>
		/// <param name="position">The position.</param>
		/// <param name="oldTabHost">The old tab host.</param>
		void AddTabItem(ChromeTabItemVM tabItem, int position, IChromeTabsHostVM oldTabHost);

		/// <summary>
		/// Moves the tab item.
		/// </summary>
		/// <param name="tabItem">The tab item.</param>
		/// <param name="newHost">The new host.</param>
		void MoveTabItem(ChromeTabItemVM tabItem, IChromeTabsHostVM newHost);

		/// <summary>
		/// Removes the tab item.
		/// </summary>
		/// <param name="tabItem">The tab item.</param>
		void RemoveTabItem(ChromeTabItemVM tabItem);

		/// <summary>
		/// Closes the tab item.
		/// </summary>
		/// <param name="tabItem">The tab item.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		bool CloseTabItem(ChromeTabItemVM tabItem); //TODO revise CloseTabItem return value


		/// <summary>
		/// Gets the number of tab items.
		/// </summary>
		/// <value>The number of tab items.</value>
		int CountTabItems { get; }

		/// <summary>
		/// Gets the 'tab item created' event.
		/// </summary>
		/// <value>The tab item created event.</value>
		IEventSource<EventHandler<TabItemCreatedEventArgs>> TabItemCreatedEvent { get; }

		/// <summary>
		/// Gets the 'tab item closing' event.
		/// </summary>
		/// <value>The tab item closing event.</value>
		IEventSource<EventHandler<TabItemClosingEventArgs>> TabItemClosingEvent { get; }

		/// <summary>
		/// Gets the 'tab item removed' event.
		/// </summary>
		/// <value>The tab item removed event.</value>
		IEventSource<EventHandler<TabItemEventArgs>> TabItemRemovedEvent { get; }

		/// <summary>
		/// Gets the 'tab item closed' event.
		/// </summary>
		/// <value>The tab item closed event.</value>
		IEventSource<EventHandler<TabItemEventArgs>> TabItemClosedEvent { get; }

		/// <summary>
		/// Gets the 'tab item added' event.
		/// </summary>
		/// <value>The tab item added event.</value>
		IEventSource<EventHandler<TabItemAddedEventArgs>> TabItemAddedEvent { get; }

		/***************************************************************************/

		/// <summary>
		/// Gets the tab items.
		/// </summary>
		/// <value>The tab items.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.ItemsSource"/></remarks>
		IEnumerable TabItems { get; } //TODO revise return type.

		/// <summary>
		/// Gets the current tab item.
		/// </summary>
		/// <value>The current tab item.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.SelectedItem"/></remarks>
		ChromeTabItemVM CurrentTabItem { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether tabs are allowed to be moved.
		/// </summary>
		/// <value><c>true</c> if [allow move tabs]; otherwise, <c>false</c>.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.CanMoveTabs"/></remarks>
		bool AllowMoveTabs { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the 'add tab button' shall be displayed.
		/// </summary>
		/// <value><c>true</c> if [show add button]; otherwise, <c>false</c>.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.IsAddButtonVisible"/></remarks>
		bool ShowAddButton { get; set; }

		/// <summary>
		/// Gets the 'add new tab' action. 
		/// </summary>
		/// <value>The add new tab action.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.AddTabCommand"/></remarks>
		ActionVM AddNewTabAction { get; }

		/// <summary>
		/// Gets the 'close tab' action. 
		/// </summary>
		/// <value>The close tab action.</value>
		/// <remarks>Bind to <see cref="ChromeTabControl.CloseTabCommand"/></remarks>
		ActionVM CloseTabAction { get; }

		/// <summary>
		/// Gets the 'reorder tabs' action. Bind to <see cref="ChromeTabControl.ReorderTabsCommand"/>
		/// </summary>
		/// <value>The reorder tabs action.</value>
		/// <remarks>The view model uses the 'reorder tabs' event to reorder the tab item view models. </remarks>
		ActionVM ReorderTabsAction { get; }

		/// <summary>
		/// Gets the 'pin tab' action.
		/// </summary>
		/// <value>The pin tab action.</value>
		/// <remarks>The 'pin tab' action pins a tab. See <see cref="ChromeTabItemVM.IsPinned"/></remarks>
		ActionVM PinTabAction { get; }

	}

}