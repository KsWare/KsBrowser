/* ORIGINAL: ChromeTabsDemo\ViewModel\TabBase.cs
 CHANGES 
- remove using GalaSoft.MvvmLight;
- add KsWare.Presentation.ViewModelFramework
- replace properties 
- rename to ChromeTabItemVM
- remove 'abstract'
- add notification interface and virtual members
*/

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ChromeTabs;
using JetBrains.Annotations;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModels {

	public partial class ChromeTabItemVM : ObjectVM /*TabItemVM*/ {

		/// <inheritdoc />
		public ChromeTabItemVM() {
			RegisterChildren(() => this);
			// Fields[nameof(ParentTab)].ValueChangedEvent.add = (d, e) => OnParentTabChanged(e);//< DEACTIVATED. managing the three for dragging between windows is currently not supported.
			Fields[nameof(TabHost)].ValueChangedEvent.add = (d, e) => OnTabHostChanged(e);
		}

		// protected ChromeTabItemVM(IChromeTabHostVM tabHost) : this() {
		// 	TabHost = tabHost;
		// }

		public int TabNumber { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }
		
		/// <summary>Gets or sets a value indicating whether this instance is pinned.</summary>
		/// <value> <c>true</c> if this instance is pinned; otherwise, <c>false</c>.</value>
		public bool IsPinned { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		// TODO don't use ImageSource use a more common type
		public ImageSource TabIcon { get => Fields.GetValue<ImageSource>(); set => Fields.SetValue(value); }

		public ActionVM CloseAction { get; [UsedImplicitly] private set; }

		/// <summary> Gets or sets the title. </summary>
		/// <value>The title.</value>
		public string Title { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public object Tag { get => Fields.GetValue<object>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public IChromeTabHostVM TabHost { get => Fields.GetValue<IChromeTabHostVM>(); set => Fields.SetValue(value); }

		public IInputElement CommandTarget { get => Fields.GetValue<IInputElement>(); set => Fields.SetValue(value); }

		[Obsolete("DEACTIVATED. managing the three for dragging between windows is currently not supported.", true)]
		public RefListVM<ChromeTabItemVM> SubTabs { get; [UsedImplicitly] private set; }

		[Obsolete("DEACTIVATED. managing the three for dragging between windows is currently not supported.", true)]
		[Hierarchy(HierarchyType.Reference)]
		public ChromeTabItemVM ParentTab { get => Fields.GetValue<ChromeTabItemVM>(); set => Fields.SetValue(value); }

		internal ChromeTabItem View { get; set; }

		/// <summary>
		/// Called when <see cref="TabHost"/> property changed.
		/// </summary>
		/// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnTabHostChanged(ValueChangedEventArgs e) {
		}

		/// <summary>
		/// Called when <see cref="IChromeTabHostVM"/> created this tab.
		/// </summary>
		/// <param name="options">The <see cref="ITabItemCreationOptions"/>.</param>
		protected virtual void OnCreated(ITabItemCreationOptions options) { }

		/// <summary>
		/// Called when <see cref="ParentTab"/> property changed.
		/// </summary>
		/// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnParentTabChanged(ValueChangedEventArgs e) { }

		/// <summary>
		/// Called when this tab is closed.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnClosing(CancelEventArgs e) {
		}

		/// <summary>
		/// Called when this tab becomes the active tab.
		/// </summary>
		protected virtual void OnActivated() {
		}

		// public void NotifyDeactivated() {
		// 	OnDeactivated();
		// }

		// /// <summary>
		// /// Called when this tab is no longer the active tab.
		// /// </summary>
		// protected override void OnDeactivated() {
		//
		// }

		/// <seealso cref="CloseAction"/>
		protected virtual void DoClose() {
		}

		/// <summary>
		/// Called on new <see cref="DockingWindow"/> created. EXPERIMENTAL. Not MVVM compliant.
		/// </summary>
		/// <param name="window">The window.</param>
		[Obsolete("",true)]
		public virtual void NotifyMoveToNewHost(DockingWindow window) {
		}

		protected virtual void OnRemoving(CancelEventArgs cancelEventArgs) {
		}

		protected virtual void OnMoved(IChromeTabHostVM newHost, IChromeTabHostVM oldHost) {
		}

		protected virtual void OnClosed() {
		}

		protected virtual void OnRemoved() {
		}

		protected virtual void OnAdded(IChromeTabHostVM host) {
		}
	}

}