using KsWare.Presentation.Utilities;

namespace KsWare.KsBrowser {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {
		/// <inheritdoc />
		public App() {
			WinApi.SetProcessDpiAwarenessPerMonitor();
			CatchUnhandledExceptions = false;
			KsWare.KsBrowser.CefSpecific.AppInit.Init();
		}
	}
}
