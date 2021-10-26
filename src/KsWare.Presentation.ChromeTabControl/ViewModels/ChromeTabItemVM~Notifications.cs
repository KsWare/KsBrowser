using System.ComponentModel;

namespace KsWare.Presentation.ViewModels {

	public partial class ChromeTabItemVM : IHandleTabItemNotifications {

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyCreated() {
			OnCreated(null);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyCreated(ITabItemCreationOptions options) {
			OnCreated(options);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyAdded(IChromeTabsHostVM host) {
			OnAdded(host);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyRemoving(CancelEventArgs e) {
			OnRemoving(e);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyRemoved() {
			OnRemoved();
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyClosing(CancelEventArgs e) {
			OnClosing(e);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyClosed() {
			OnClosed();
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyMoved(IChromeTabsHostVM newHost, IChromeTabsHostVM oldHost) {
			OnMoved(newHost, oldHost);
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyActivated() {
			base.OnActivated(null);
			OnActivated();
		}

		/// <inheritdoc />
		void IHandleTabItemNotifications.NotifyDeactivated() {
			OnDeactivated();
		}

		// /// <summary>
		// /// Called on new <see cref="DockingWindow"/> created. EXPERIMENTAL. Not MVVM compliant.
		// /// </summary>
		// /// <param name="window">The window.</param>
		// public virtual void NotifyMoveToNewHost(IChromeTabHostVM host) {
		// }
	}

}