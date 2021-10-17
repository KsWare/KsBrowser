// ORIGINAL ChromeTabsDemo\WindowBase.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ChromeTabs;
using KsWare.Presentation.Utilities;
using KsWare.Presentation.ViewModels;
using static System.Windows.PresentationSource;

namespace KsWare.Presentation.Controls {

	// TODO Revise old docking logic and use MVVM

	public abstract class ChromeTabsBaseWindow : Window {

		private static readonly List<ChromeTabsBaseWindow> ChromeTabsWindows = new List<ChromeTabsBaseWindow>();

		private ChromeTabControl _tabControl;

		//We use this collection to keep track of what windows we have open
		protected readonly List<DockingWindow> DockingWindows = new List<DockingWindow>();

		public ChromeTabsBaseWindow() {
			
		}

		public new ChromeTabsBaseWindowVM DataContext { get => (ChromeTabsBaseWindowVM)base.DataContext; set => base.DataContext = value; }

		/// <summary>
		/// This event triggers when a tab is dragged outside the bonds of the tab control panel.
		/// We can use it to create a docking tab control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e) {
			_tabControl = (ChromeTabControl)sender;
			var draggedTab = e.Tab as ChromeTabItemVM;
			if (TryDragTabToWindow(e.CursorPosition, draggedTab)) {
				//Set Handled to true to tell the tab control that we have dragged the tab to a window, and the tab should be closed.
				e.Handled = true;
			}
		}

		protected void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e) {
			_tabControl = (ChromeTabControl)sender;
			e.Handled = true;
			if (e.TabItem != null && e.Model is ChromeTabItemVM viewModel) {
				e.TabItem.IsPinned = viewModel.IsPinned;
			}
		}

		protected bool TryDockWindow(Point absoluteScreenPos, ChromeTabItemVM dockedWindowVM) {
			var position = _tabControl.PointFromScreen(absoluteScreenPos);
			Debug.WriteLine($"    {position}");
			//Hit test against the tab control
			if (_tabControl.InputHitTest(position) is FrameworkElement element) {
				Debug.WriteLine($"{element.GetType().Name} {element.Name}");
				////test if the mouse is over the tab panel or a tab item.
				if (CanInsertTabItem(element)) {
					//TabBase dockedWindowVM = (TabBase)win.DataContext;
					DataContext.TabItems.Add(dockedWindowVM); //TODO TryDockWindow => VM 
					DataContext.CurrentTabItem = dockedWindowVM;
					//We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
					_tabControl.GrabTab(dockedWindowVM);
					return true;
				}
			}
			return false;
		}

		protected bool TryDragTabToWindow(Point position, ChromeTabItemVM draggedTab) {
			// TODO add property 'AllowChangeWindow' or similar and use that instead checking type
			// if (draggedTab is Class3TabItemVM)
			// 	return false; //As an example, we don't want TabClass3 to form new windows, so we stop it here.
			if (draggedTab.IsPinned)
				return false;  //We don't want pinned tabs to be draggable either.

			return UseDockingWindow(position, draggedTab);
			// return UseChromeTabsWindow(position, draggedTab);
		}

		// private bool UseChromeTabsWindow(Point position, ChromeTabItemVM draggedTab) {
		// 	var win = ChromeTabsWindows.FirstOrDefault(x => x.DataContext == draggedTab); //check if it's already open
		// 	if (win == null) { //If not, create a new one
		// 		win = new ChromeTabsWindow() {
		// 			Title = draggedTab?.Title ?? "", // Title MUST NOT be null
		// 			DataContext = new ChromeTabsWindowVM()
		// 		};
		// 		//TODO draggedTab.NotifyMoveToNewHost(win);
		// 		
		// 		win.Closed += DockingWindow_Closed;
		// 		win.Loaded += DockingWindow_Loaded;
		// 		win.LocationChanged += DockingWindow_LocationChanged;
		// 		win.Tag = position;
		// 		var scale = VisualTreeHelper.GetDpi(this);
		// 		win.Left = position.X / scale.DpiScaleX - win.Width / 2;
		// 		win.Top = position.Y / scale.DpiScaleY - 10;
		//
		// 		win.Show();
		// 	}
		// 	else{
		// 		Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
		// 		MoveWindow(win, position);
		// 	}
		//
		// 	ChromeTabsWindows.Add(win);
		// 	return true;
		// }

		private bool UseDockingWindow(Point position, ChromeTabItemVM draggedTab) {
			var win = DockingWindows.FirstOrDefault(x => x.DragContent == draggedTab); //check if it's already open
			if (win == null) { //If not, create a new one
				win = new DockingWindow {
					Title = draggedTab?.Title ?? "", // Title MUST NOT be null
					DragContent = draggedTab
				};
				draggedTab.TabHost.MoveTabItem(draggedTab, null);
				
				win.Closed += DockingWindow_Closed;
				win.Loaded += DockingWindow_Loaded;
				win.LocationChanged += DockingWindow_LocationChanged;
				win.Tag = position;
				var scale = VisualTreeHelper.GetDpi(this);
				win.Left = position.X / scale.DpiScaleX - win.Width / 2;
				win.Top = position.Y / scale.DpiScaleY - 10;
				win.Width = 600;
				win.Height = 400;
				win.Show();
			}
			else{
				Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
				MoveWindow(win, position);
			}

			DockingWindows.Add(win);
			return true;
		}

		private void DockingWindow_Loaded(object sender, RoutedEventArgs e) {
			var win = (Window)sender;
			win.Loaded -= DockingWindow_Loaded;
			var cursorPosition = (Point)win.Tag;
			MoveWindow(win, cursorPosition);
		}

		private void MoveWindow(Window win, Point pt) {
			//Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
			Dispatcher.BeginInvoke(new Action(() => {
				win.Topmost = true;
				//We position the window at the mouse position
				var scale = VisualTreeHelper.GetDpi(this);
				win.Left = pt.X / scale.DpiScaleX - win.Width / 2;
				win.Top = pt.Y / scale.DpiScaleY - 10;
				Debug.WriteLine(DateTime.Now.ToShortTimeString() + " dragging window");

				if (Mouse.LeftButton == MouseButtonState.Pressed) {
					win.DragMove(); //capture the movement to the mouse, so it can be dragged around
				}

				win.Topmost = false;
			}));
		}

		//remove the window from the open windows collection when it is closed.
		private void DockingWindow_Closed(object sender, EventArgs e) {
			DockingWindows.Remove(sender as DockingWindow);
			Debug.WriteLine(DateTime.Now.ToShortTimeString() + " closed window");
		}

		//We use this to keep track of where the window is on the screen, so we can dock it later
		private void DockingWindow_LocationChanged(object sender, EventArgs e) {
			var win = (DockingWindow)sender;
			if (!win.IsLoaded) return;

			var absoluteScreenPos = WinApi.GetCursorPos();
			var windowUnder = FindWindowUnderThisAt(win, absoluteScreenPos);
			var rect = windowUnder != null ? new Rect(windowUnder.Left, windowUnder.Top, windowUnder.Width, windowUnder.Height) : new Rect();
			if(windowUnder==null) Debug.WriteLine($"None  C:{absoluteScreenPos}");
			else Debug.WriteLine($"Found C:{absoluteScreenPos} W:{rect}");

			if (windowUnder != null && windowUnder.Equals(this)) {
				if (TryDockWindow(absoluteScreenPos, (ChromeTabItemVM)win.DragContent)) {
					win.Close();
				}
			}
		}

		protected bool CanInsertTabItem(FrameworkElement element) {
			if (element is ChromeTabItem) return true;
			if (element is ChromeTabPanel) return true;
			var child = LogicalTreeHelper.GetChildren(element).Cast<object>().FirstOrDefault(x => x is ChromeTabPanel);
			if (child != null) return true;
			var localElement = element;
			while (true) {
				Object obj = localElement?.TemplatedParent;
				if (obj == null) break;
				if (obj is ChromeTabItem) return true;
				localElement = localElement.TemplatedParent as FrameworkElement;
			}
			return false;
		}

		/// <summary>
		/// Used P/Invoke to find and return the top window under the cursor position
		/// </summary>
		/// <param name="source">The source window.</param>
		/// <param name="devicePoint">Point in device units. NOT WPF units (96dpi)</param>
		/// <returns></returns>
		private Window FindWindowUnderThisAt(Window source, Point devicePoint) {
			var excludeTypeNames = new [] {
				//prevent "UI debugging tools for XAML" from interfering when debugging.
				"Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerWindow", 
				"Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerLayerWindow"
			};

			var appWindows = Application.Current.Windows.OfType<Window>().ToArray();
			var excludeWindows = appWindows.Where(w => w == source || excludeTypeNames.Contains(w.GetType().FullName));
			var sortedWindows = SortWindowsTopToBottom(appWindows.Except(excludeWindows));
			var windowsUnderCurrent = sortedWindows
				//.Where(w => w.WindowState != WindowState.Maximized)
				.Where(w=>WinApi.GetWindowRect(w).Contains(devicePoint));
			return windowsUnderCurrent.FirstOrDefault();
		}

		/// <summary>
		/// We need to do some P/Invoke magic to get the windows on screen
		/// </summary>
		/// <param name="windows"></param>
		/// <returns></returns>
		private static IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> windows) {
			var byHandle = windows.ToDictionary(w => ((HwndSource)FromVisual(w)).Handle);
			for (var hWnd = WinApi.GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = WinApi.GetWindow(hWnd, WinApi.GW_HWNDNEXT)) {
				if (byHandle.ContainsKey(hWnd)) yield return byHandle[hWnd];
			}
		}

		
	}

}
