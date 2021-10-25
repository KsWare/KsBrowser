using System.Windows;
using KsWare.Presentation.Utilities;

namespace MonitorTestApp {

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		/// <inheritdoc />
		public App() {
			WinApi.SetProcessDpiAwarenessPerMonitor();
		}
	}

}
