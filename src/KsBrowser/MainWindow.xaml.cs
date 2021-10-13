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
		}

		// TODO try not to use code behind

		/// <summary>
		/// This event triggers when a tab is dragged outside the bonds of the tab control panel.
		/// We can use it to create a docking tab control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e) {
			ChromeTabItemVM draggedTab = e.Tab as ChromeTabItemVM;
			if (TryDragTabToWindow(e.CursorPosition, draggedTab)) {
				//Set Handled to true to tell the tab control that we have dragged the tab to a window, and the tab should be closed.
				e.Handled = true;
			}
		}

		protected override bool TryDockWindow(Point position, ChromeTabItemVM dockedWindowVM) {
			//Hit test against the tab control
			if (TabControl.InputHitTest(position) is FrameworkElement element) {
				////test if the mouse is over the tab panel or a tab item.
				if (CanInsertTabItem(element)) {
					//TabBase dockedWindowVM = (TabBase)win.DataContext;
					var vm = (MainWindowVM)DataContext;
					vm.Tabs.Add(dockedWindowVM);
					vm.CurrentTab = (BrowserTabItemVM)dockedWindowVM;
					//We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
					TabControl.GrabTab(dockedWindowVM);
					return true;
				}
			}

			return false;
		}
	}
}
