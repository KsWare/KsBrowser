using System;
using System.Collections.Generic;
using System.Text;
using Demo.ViewModels;
using KsWare.Presentation.Utilities;
using KsWare.Presentation.ViewModelFramework;

namespace Demo {

	public class AppVM : ApplicationVM {

		/// <inheritdoc />
		public AppVM() {
			WinApi.SetProcessDpiAwarenessPerMonitor();
			StartupUri = typeof(MainWindowVM);
		}
	}

}
