using System;
using System.Windows;
using KsWare.KsBrowser.Extensions;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser {

	public class AppVM : ApplicationVM {

		/// <inheritdoc />
		public AppVM() {
			StartupUri = typeof(MainWindowVM);
		}

		/// <inheritdoc />
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			var mainWindow = (MainWindowVM)Windows.First;
			Dispatcher.TryBeginInvoke(() => mainWindow.OnStartup(new Uri[]{new Uri("http://www.microsoft.de/")}));
		}
	}
}
