using System;
using System.Diagnostics;
using System.Windows;
using ChromeTabs;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace KsWare.KsBrowser {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ChromeTabsBaseWindow {

		public MainWindow() {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] new MainWindow");
			InitializeComponent();

			TabControl.TabDraggedOutsideBonds += TabControl_TabDraggedOutsideBonds;
		}
	}
}
