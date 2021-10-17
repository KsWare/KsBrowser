using System;
using System.Diagnostics;
using KsWare.Presentation.Controls;

namespace KsWare.KsBrowser {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ChromeTabsBaseWindow {

		public MainWindow() {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] new MainWindow");
			InitializeComponent();
		}
	}
}
