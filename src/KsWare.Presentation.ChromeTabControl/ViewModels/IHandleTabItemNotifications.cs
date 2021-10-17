using System.ComponentModel;

namespace KsWare.Presentation.ViewModels {

	public interface IHandleTabItemNotifications {

		void NotifyCreated();
		void NotifyCreated(ITabItemCreationOptions options);
		void NotifyAdded(IChromeTabHostVM host);
		void NotifyRemoving(CancelEventArgs e);
		void NotifyRemoved();
		void NotifyClosing(CancelEventArgs e);
		void NotifyClosed();
		void NotifyMoved(IChromeTabHostVM newHost, IChromeTabHostVM oldHost);
		void NotifyActivated();
		void NotifyDeactivated();
	}
}
