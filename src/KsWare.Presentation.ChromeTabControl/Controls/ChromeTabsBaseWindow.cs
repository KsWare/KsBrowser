// ORIGINAL ChromeTabsDemo\WindowBase.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ChromeTabs;
using KsWare.Presentation.Utilities;
using KsWare.Presentation.ViewModels;
using static System.Windows.PresentationSource;
using static KsWare.Presentation.Utilities.Tools;

namespace KsWare.Presentation.Controls {

	// TODO Revise old docking logic and use MVVM

	public abstract class ChromeTabsBaseWindow : Window {

		//We use this collection to keep track of what windows we have open
		protected static readonly List<DockingWindow> DockingWindows = new List<DockingWindow>();
		protected static readonly List<ChromeTabsBaseWindow> ChromeTabsWindows = new List<ChromeTabsBaseWindow>();

		private DragData _dragData;

		protected ChromeTabsBaseWindow() {
			ChromeTabsWindows.Add(this);
			Closed += OnClosed;
			Loaded += OnLoaded;
			LocationChanged += OnLocationChanged;
		}

		public new ChromeTabsBaseWindowVM DataContext { get => (ChromeTabsBaseWindowVM)base.DataContext; set => base.DataContext = value; }
		
		/// <summary>
		/// This event triggers when a tab is dragged outside the bonds of the tab control panel.
		/// We can use it to create a docking tab control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e) {
			if (TryDragTabToWindow(e.CursorPosition, (ChromeTabItemVM)e.Tab)) {
				//Set Handled to true to tell the tab control that we have dragged the tab to a window, and the tab should be closed.
				e.Handled = true;
			}
		}

		protected void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e) {
			e.Handled = true;
			if (e.TabItem != null && e.Model is ChromeTabItemVM viewModel) {
				e.TabItem.IsPinned = viewModel.IsPinned;
			}
		}

		protected void TryDockTo(ChromeTabsBaseWindow window, Point absoluteScreenPos, ChromeTabItemVM tabItemViewModel) {
			// var scale = WinApi.GetDpiForMonitorFromPoint(absoluteScreenPos);
			// absoluteScreenPos = new Point(absoluteScreenPos.X * scale.DpiScaleX, absoluteScreenPos.Y * scale.DpiScaleY);
			var newTabControl = FindTabControl(window);
			var position = newTabControl.PointFromScreen(absoluteScreenPos);
			//Hit test against the tab control
			if (!(newTabControl.InputHitTest(position) is FrameworkElement element)) return;
			////test if the mouse is over the tab panel or a tab item.
			if (!CanInsertTabItem(element)) return;

			var myItemsHolder = FindItemsHolder(this);
			var itemPresenter = FindContentPresenter(myItemsHolder, tabItemViewModel);
			myItemsHolder.Children.Remove(itemPresenter);
			var newItemsHolder=FindItemsHolder(window);
			newItemsHolder.Children.Add(itemPresenter);
			tabItemViewModel.TabHost.RemoveTabItem(tabItemViewModel);
			((IProvideTabHostVM)window.DataContext).TabHost.AddTabItem(tabItemViewModel, -1, null);
			window.DataContext.CurrentTabItem = tabItemViewModel;
			Close();
			//We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
			newTabControl.GrabTab(tabItemViewModel);
		}

		protected void TryDockWindow(Point absoluteScreenPos, DockingWindow dockingWindow) {
			//DockingWindow
			var tabItemViewModel = dockingWindow.DragData.TabItemViewModel;
			var tabControl = FindTabControl(this);
			var position = tabControl.PointFromScreen(absoluteScreenPos);
			//Hit test against the tab control
			if (!(tabControl.InputHitTest(position) is FrameworkElement element)) return;
			////test if the mouse is over the tab panel or a tab item.
			if (!CanInsertTabItem(element)) return;
			//TabBase dockedWindowVM = (TabBase)win.DataContext;
			DataContext.TabItems.Add(tabItemViewModel); //TODO TryDockWindow => VM 
			DataContext.CurrentTabItem = tabItemViewModel;
			//We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
			tabControl.GrabTab(tabItemViewModel);
			dockingWindow.Close();
		}

		protected bool TryDragTabToWindow(Point position, ChromeTabItemVM draggedTab) {
			// TODO add property 'AllowChangeWindow' or similar and use that instead checking type
			// if (draggedTab is Class3TabItemVM)
			// 	return false; //As an example, we don't want TabClass3 to form new windows, so we stop it here.
			if (draggedTab.IsPinned)
				return false;  //We don't want pinned tabs to be draggable either.

			// DragToNewDockingWindow(position, draggedTab);
			DragToNewChromeTabsWindow(position, draggedTab);
			return true;
		}

		private void DragToNewChromeTabsWindow(Point screenPosition, ChromeTabItemVM draggedTabViewModel) {
			var viewType = this.GetType();
			var vmType = this.DataContext.GetType();

			var myItemsHolder = FindItemsHolder(this);
			var dragData = new DragData {
				TabItemViewModel = draggedTabViewModel,
				Position = screenPosition,
				ItemPresenter = FindContentPresenter(myItemsHolder, draggedTabViewModel) //will be moved in OnLoaded
			};

			var newTabsWindow = (ChromeTabsBaseWindow)Activator.CreateInstance(viewType);
			newTabsWindow.DataContext = (ChromeTabsBaseWindowVM)Activator.CreateInstance(vmType);
			newTabsWindow._dragData = dragData;

			myItemsHolder.Children.Remove(dragData.ItemPresenter);
			draggedTabViewModel.TabHost.RemoveTabItem(draggedTabViewModel);
			var scale = VisualTreeHelper.GetDpi(this);
			newTabsWindow.Left = screenPosition.X / scale.DpiScaleX - newTabsWindow.Width / 2;
			newTabsWindow.Top = screenPosition.Y / scale.DpiScaleY - 10;
	
			newTabsWindow.Show();
			// finish drop on ==> OnLoaded()
		}

		private void DragToNewDockingWindow(Point position, ChromeTabItemVM draggedTabViewModel) {
			const double width = 600;
			var scale = VisualTreeHelper.GetDpi(this);
			var dockingWindow = DockingWindows.FirstOrDefault(x => x.DragData?.TabItemViewModel == draggedTabViewModel); //check if it's already open
			if (dockingWindow == null) { //If not, create a new one
				dockingWindow = new DockingWindow {
					Title = draggedTabViewModel?.Title ?? "", // Title MUST NOT be null
					DragData = new DragData {
						TabItemViewModel = draggedTabViewModel,
						Position = position
					},
					Left = position.X / scale.DpiScaleX - width / 2,
					Top = position.Y / scale.DpiScaleY - 10,
					Width = width,
					Height = 400
				};
				draggedTabViewModel.TabHost.RemoveTabItem(draggedTabViewModel);
				
				dockingWindow.Closed += DockingWindow_Closed;
				dockingWindow.Loaded += DockingWindow_Loaded;
				dockingWindow.LocationChanged += DockingWindow_LocationChanged;
				dockingWindow.Show();
			}
			else{
				Debug.WriteLine(DateTime.Now.ToShortTimeString() + " got window");
				MoveWindow(dockingWindow, position);
			}

			DockingWindows.Add(dockingWindow);
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			Loaded -= OnLoaded;
			if(_dragData==null) return;

			var itemHolder = FindItemsHolder(this);
			itemHolder.Children.Add(_dragData.ItemPresenter);
			DataContext.TabHost.AddTabItem(_dragData.TabItemViewModel, 0, null);
			
			var cursorPosition = _dragData.Position;
			MoveWindow(this, cursorPosition);
		}

		private void MoveWindow(ChromeTabsBaseWindow tabsWindow, Point pt) {
			//Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
			Dispatcher.BeginInvoke(new Action(() => {
				tabsWindow.Topmost = true;
				//We position the window at the mouse position
				var scale = VisualTreeHelper.GetDpi(this);
				tabsWindow.Left = pt.X / scale.DpiScaleX - tabsWindow.Width / 2;
				tabsWindow.Top = pt.Y / scale.DpiScaleY - 10;

				if (Mouse.LeftButton == MouseButtonState.Pressed) {
					tabsWindow._dragData.IsDragMove = true;
					tabsWindow.DragMove(); //capture the movement to the mouse, so it can be dragged around
				}

				tabsWindow.Topmost = false;
			}));
		}

		private void OnLocationChanged(object sender, EventArgs e) {
			//We use this to keep track of where the window is on the screen, so we can try to dock it
			if(!WinApi.IsMouseLeftButtonPressed)
				return;
			if(!IsLoaded) 
				return;
			if(DataContext.TabHost.CountTabItems != 1) 
				return;
			var absoluteScreenPos = WinApi.GetCursorPos();
			if (!(FindWindowUnderThisAt(this, absoluteScreenPos) is ChromeTabsBaseWindow windowUnder)) 
				return;
			TryDockTo(windowUnder, absoluteScreenPos, _dragData.TabItemViewModel);
		}

		private void OnClosed(object sender, EventArgs e) {
			Closed -= OnClosed;
			ChromeTabsWindows.Remove(this);
		}

		private void DockingWindow_Loaded(object sender, RoutedEventArgs e) {
			var dockingWindow = (DockingWindow)sender;
			dockingWindow.Loaded -= DockingWindow_Loaded;
			var cursorPosition = dockingWindow.DragData.Position;
			MoveWindow(dockingWindow, cursorPosition);
		}

		private void MoveWindow(DockingWindow dockingWindow, Point pt) {
			//Use a BeginInvoke to delay the execution slightly, else we can have problems grabbing the newly opened window.
			Dispatcher.BeginInvoke(new Action(() => {
				dockingWindow.Topmost = true;
				//We position the window at the mouse position
				var scale = VisualTreeHelper.GetDpi(this);
				dockingWindow.Left = pt.X / scale.DpiScaleX - dockingWindow.Width / 2;
				dockingWindow.Top = pt.Y / scale.DpiScaleY - 10;
				Debug.WriteLine(DateTime.Now.ToShortTimeString() + " dragging window");

				if (Mouse.LeftButton == MouseButtonState.Pressed) {
					Mouse.AddMouseUpHandler(dockingWindow, DockingWindow_MouseUp);
					dockingWindow.DragData.IsDragMove = true;
					dockingWindow.DragMove(); //capture the movement to the mouse, so it can be dragged around
				}

				dockingWindow.Topmost = false;
			}));
		}

		private void DockingWindow_MouseUp(object sender, MouseButtonEventArgs e) {
			var dockingWindow = (DockingWindow)sender;
			if (dockingWindow.DragData.IsDragMove) {
				Mouse.RemoveMouseUpHandler(dockingWindow, DockingWindow_MouseUp);
				dockingWindow.DragData.IsDragMove = false;
				DockingWindow_DragEnd(sender, EventArgs.Empty);
			}
		}

		private void DockingWindow_DragEnd(object sender, EventArgs e) {
			var dockingWindow = (DockingWindow)sender;
			var newWindow = CreateWindow(dockingWindow.DragData.TabItemViewModel);
			if(newWindow != null){
				dockingWindow.Close();
				ChromeTabsWindows.Add(newWindow);
			}
		}

		protected virtual ChromeTabsBaseWindow CreateWindow(ChromeTabItemVM tabItem) {
			var p = WinApi.GetCursorPos();
			var scale = WinApi.GetDpiForMonitorFromPoint(p);
			p = new Point(p.X / scale.DpiScaleX, p.Y / scale.DpiScaleY);
			var win = new ChromeTabsWindow {
				DataContext = new ChromeTabsWindowVM(),
				WindowStartupLocation = WindowStartupLocation.Manual,
				Left = p.X, Top = p.Y,
				Width = 600, Height = 400
			};
			win.Loaded += ChromeTabsWindow_Loaded;
			win.Show();
			if (win.DataContext is IProvideTabHostVM provider) {
				provider.TabHost.AddTabItem(tabItem, 0, null);
			}
			Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
			
			return win;
		}

		private void ChromeTabsWindow_Loaded(object sender, RoutedEventArgs e) {
			var window = (ChromeTabsBaseWindow)sender;
			window.Loaded -= ChromeTabsWindow_Loaded;
			var tabControl = FindTabControl(window);
			var p = tabControl.TranslatePoint(new Point(100, 16), window);  // estimated center of a tab item
			window.Left -= p.X;
			window.Top -= p.Y;

			WinApi.ForceWindowOnScreen(window);
		}

		private void DockingWindow_Closed(object sender, EventArgs e) {
			//remove the window from the open windows collection when it is closed.
			var dockingWindow = (DockingWindow)sender;
			if (dockingWindow.DragData.IsDragMove) {
				Mouse.RemoveMouseUpHandler(dockingWindow, DockingWindow_MouseUp);
			}
			DockingWindows.Remove(sender as DockingWindow);
			Debug.WriteLine(DateTime.Now.ToShortTimeString() + " closed window");
		}
		
		private void DockingWindow_LocationChanged(object sender, EventArgs e) {
			//We use this to keep track of where the window is on the screen, so we can dock it later
			if(Mouse.LeftButton==MouseButtonState.Released) return;
			var dockingWindow = (DockingWindow)sender;
			if (!dockingWindow.IsLoaded) return;
			var absoluteScreenPos = WinApi.GetCursorPos();
			var windowUnder = FindWindowUnderThisAt(dockingWindow, absoluteScreenPos);
			if (windowUnder == null) return;
			TryDockWindow(absoluteScreenPos, dockingWindow);
		}

		protected bool CanInsertTabItem(FrameworkElement element) {
			if (element is ChromeTabItem) return true;
			if (element is ChromeTabPanel) return true;
			var child = LogicalTreeHelper.GetChildren(element).Cast<object>().FirstOrDefault(x => x is ChromeTabPanel);
			if (child != null) return true;
			var localElement = element;
			while (true) {
				var obj = localElement?.TemplatedParent;
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
		private static Window FindWindowUnderThisAt(Window source, Point devicePoint) {
			var excludeTypeNames = new [] {
				//prevent "UI debugging tools for XAML" from interfering when debugging.
				"Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerWindow", 
				"Microsoft.VisualStudio.DesignTools.WpfTap.WpfVisualTreeService.Adorners.AdornerLayerWindow"
			};

			var appWindows = Application.Current.Windows.OfType<Window>().ToArray();
			var excludeWindows = appWindows.Where(w => w == source || excludeTypeNames.Contains(w.GetType().FullName));
			var sortedWindows = SortWindowsTopToBottom(appWindows.Except(excludeWindows));
			var windowsUnderCurrent = sortedWindows.Where(w => WinApi.GetWindowRect(w).Contains(devicePoint));
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
