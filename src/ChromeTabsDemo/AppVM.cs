using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Demo.ViewModels;
using KsWare.Presentation.Utilities;
using KsWare.Presentation.ViewModelFramework;

namespace Demo {

	public class AppVM : ApplicationVM {

		/// <inheritdoc />
		public AppVM() {
			WinApi.SetProcessDpiAwarenessPerMonitor();
			
		}

		/// <inheritdoc />
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);
			var vm = new MainWindowVM();
			vm.OnStartup();
			vm.Show();
		}
	}

}
