using System.Windows;

namespace KsWare.KsBrowser.Overlays {

	public class MessageOverlayButton {
		public bool IsDefault { get; set; }
		public bool IsCancel { get; set; }
		public string Title { get; set; }
		public MessageBoxResult Result { get; set; }
	}

}