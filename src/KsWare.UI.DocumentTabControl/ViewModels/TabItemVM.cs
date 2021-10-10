using System.Windows;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.Presentation.Controls {

	public class TabItemVM : ObjectVM {

		/// <inheritdoc />
		public TabItemVM():this(null) { }

		protected TabItemVM(ITabHostVM tabHost) {
			RegisterChildren(() => this);
			Fields[nameof(ParentTab)].ValueChangedEvent.add = (d, e) => OnParentTabChanged(e);
			Fields[nameof(TabHost)].ValueChangedEvent.add = (d, e) => OnTabHostChanged(e);
			TabHost = tabHost;
		}

		public ActionVM CloseAction { get; [UsedImplicitly] private set; }

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
