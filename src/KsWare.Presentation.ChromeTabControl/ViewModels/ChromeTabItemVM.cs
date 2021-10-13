/* ORIGINAL: ChromeTabsDemo\ViewModel\TabBase.cs
 CHANGES 
- remove using GalaSoft.MvvmLight;
- add KsWare.Presentation.ViewModelFramework
- replace properties 
- rename to ChromeTabItemVM
- remove 'abstract'
*/

using System;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.ViewModels {

	public class ChromeTabItemVM : TabItemVM {

		/// <inheritdoc />
		public ChromeTabItemVM() : this(null) { }

		protected ChromeTabItemVM(ITabHostVM tabHost) {
			RegisterChildren(() => this);
			Fields[nameof(ParentTab)].ValueChangedEvent.add = (d, e) => OnParentTabChanged(e);
			Fields[nameof(TabHost)].ValueChangedEvent.add = (d, e) => OnTabHostChanged(e);
			TabHost = tabHost;
		}

		public int TabNumber { get => Fields.GetValue<int>(); set => Fields.SetValue(value); }

		[Obsolete("Use Title", true)]
		public string TabName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <summary>Gets or sets a value indicating whether this instance is pinned.</summary>
		/// <value> <c>true</c> if this instance is pinned; otherwise, <c>false</c>.</value>
		public bool IsPinned { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		// TODO don't use ImageSource use a more common type
		public ImageSource TabIcon { get => Fields.GetValue<ImageSource>(); set => Fields.SetValue(value); }

		public ActionVM CloseAction { get; [UsedImplicitly] private set; }

		/// <summary> Gets or sets the title. </summary>
		/// <value>The title.</value>
		public string Title { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public ITabHostVM TabHost { get => Fields.GetValue<ITabHostVM>(); set => Fields.SetValue(value); }

		public IInputElement CommandTarget { get => Fields.GetValue<IInputElement>(); set => Fields.SetValue(value); }

		public RefListVM<TabItemVM> SubTabs { get; [UsedImplicitly] private set; }

		[Hierarchy(HierarchyType.Reference)]
		public TabItemVM ParentTab { get => Fields.GetValue<TabItemVM>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Called when <see cref="TabHost"/> property changed.
		/// </summary>
		/// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnTabHostChanged(ValueChangedEventArgs e) {
			if (e.OldValue is ITabHostVM oldHost) {
				oldHost.TabCreatedEvent.Release(this, null);
			}
			if (e.NewValue is ITabHostVM tabHost) {
				tabHost.TabCreatedEvent.add = TabHost_TabCreated;
			}
		}

		private void TabHost_TabCreated(object sender, TabCreatedEventArgs e) {
			if (e.NewTab == this) OnTabCreated(e.Options);
		}

		/// <summary>
		/// Called when <see cref="ITabHostVM"/> created this tab.
		/// </summary>
		/// <param name="options">The <see cref="ITabCreationOptions"/>.</param>
		protected virtual void OnTabCreated(ITabCreationOptions options) { }

		/// <summary>
		/// Called when <see cref="ParentTab"/> property changed.
		/// </summary>
		/// <param name="e">The <see cref="ValueChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnParentTabChanged(ValueChangedEventArgs e) { }

		/// <summary>
		/// Called when this tab is closed.
		/// </summary>
		public void NotifyClosing() {
			OnClosing();
		}

		/// <summary>
		/// Called when this tab is closed.
		/// </summary>
		protected virtual void OnClosing() {
			
		}

		/// <summary>
		/// Called when this tab becomes the active tab.
		/// </summary>
		public void NotifyActivated() {
			OnActivated();
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

		

		
	}

}