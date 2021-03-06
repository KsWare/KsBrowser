using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace KsWare.Presentation.Utilities {

	public static class WinApi {

		#region Managed

		public static Point GetCursorPos() {
			var pt = new POINT();
			if (!WinApi.GetCursorPos(ref pt)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			var p = new Point(pt.X , pt.Y);
			return p;
		}

		public static Rect GetMonitorRectFromCursor(bool workingArea=false) {
			var pt = new POINT();
			if (!WinApi.GetCursorPos(ref pt)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			var hMonitor = MonitorFromPoint(pt,MONITOR_DEFAULTTONULL);
			if (hMonitor == IntPtr.Zero) return new Rect();
			var monitorInfo = new MONITORINFOEX();
			if(!GetMonitorInfo(hMonitor, monitorInfo)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return CreateRect(workingArea ? monitorInfo.WorkingArea : monitorInfo.MonitorArea);
		}

		public static Rect GetMonitorRectFromPoint(Point p, bool workingArea = false) {
			var pt = new POINT((int)p.X, (int)p.Y);
			var hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONULL);
			if (hMonitor == IntPtr.Zero) return new Rect();
			var monitorInfo = new MONITORINFOEX();
			if(!GetMonitorInfo(hMonitor, monitorInfo)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return CreateRect(workingArea ? monitorInfo.WorkingArea : monitorInfo.MonitorArea);
		}

		public static Rect GetMonitorRectFromWindow(Window window, bool workingArea = false) {
			var hWnd = new WindowInteropHelper(window).Handle;
			var hMonitor = MonitorFromWindow(hWnd, MonitorOpts.MONITOR_DEFAULTTONEAREST);
			var monitorInfo = new MONITORINFOEX();
			if(!GetMonitorInfo(hMonitor, monitorInfo)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return CreateRect(workingArea ? monitorInfo.WorkingArea : monitorInfo.MonitorArea);
		}

		public static DpiScale GetDpiForMonitorFromPoint(Point p) {
			var pt = new POINT((int)p.X, (int)p.Y);
			var hMonitor = MonitorFromPoint(pt,MONITOR_DEFAULTTONULL);
			if (hMonitor == IntPtr.Zero) throw new ArgumentOutOfRangeException();
			GetDpiForMonitor(hMonitor, MDT.EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
			return new DpiScale((double)dpiX / 96, (double)dpiY / 96);
		}

		/// <summary>
		/// Sets the dpi awareness for the current process to: per monitor dpi aware.
		/// </summary>
		public static void SetProcessDpiAwarenessPerMonitor() {
			var result = SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
			switch (result) {
				case 0: return;
				case 0x80070005: return; // The DPI awareness is already set, either by calling this API previously or through the application (.exe) manifest.
				default: Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); return;
			}
			// Return code	Description
			// 	S_OK			The DPI awareness for the app was set successfully.
			// 	E_INVALIDARG	The value passed in is not valid.
			// 	E_ACCESSDENIED	The DPI awareness is already set, either by calling this API previously or through the application (.exe) manifest.
		}

		/// <summary>
		/// Gets the window rect in device units.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>Rect.</returns>
		public static Rect GetWindowRect(Window window) {
			var hwnd = new WindowInteropHelper(window).Handle;
			if(!GetWindowRect(hwnd, out var r)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
		}

		public static void ForceWindowOnScreen(Window window) {
			var hWnd = new WindowInteropHelper(window).Handle;
			var hMonitor = MonitorFromWindow(hWnd, MonitorOpts.MONITOR_DEFAULTTONEAREST);
			var monitorInfo = new MONITORINFOEX();
			if(!GetMonitorInfo(hMonitor, monitorInfo)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			var monRect = CreateRect(monitorInfo.WorkingArea);
			if(!GetWindowRect(hWnd, out var r)) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			var winRect = new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);

			var x = Math.Min(monRect.Right - winRect.Right, 0);
			var y = Math.Min(monRect.Bottom - winRect.Bottom, 0);
			winRect = new Rect(new Point(winRect.X + x, winRect.Y + y), winRect.Size);
			x = Math.Max(monRect.Left - winRect.Left, 0);
			y = Math.Max(monRect.Top - winRect.Top, 0);
			winRect = new Rect(new Point(winRect.X + x, winRect.Y + y), winRect.Size);
			var move = SetWindowPosFlags.DoNotChangeOwnerZOrder | SetWindowPosFlags.IgnoreResize;
			SetWindowPos(hWnd, IntPtr.Zero, (int)winRect.X, (int)winRect.Y, 0, 0, move);
		}

		// Mouse.LeftButton==MouseButtonState.Pressed works only in window client area (WPF issue)
		public static bool IsMouseLeftButtonPressed => (GetKeyState(VK_LBUTTON) & KEY_PRESSED) != 0;

		#endregion 

		#region private

		private const ushort KEY_TOGGLED = 0x1;
		private const ushort KEY_PRESSED = 0x8000;
		private const int VK_LBUTTON = 0x01;

		[DllImport("USER32.dll")]
		static extern ushort GetKeyState(int nVirtKey);

		private static Rect CreateRect(RECT rect) {
			return new Rect(
				rect.Left,
				rect.Top,
				rect.Right - rect.Left,
				rect.Bottom - rect.Top
			);
		}

		#endregion 

		#region Native

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPos(ref POINT pt);

		/// <summary> Returns a handle to the display monitor that is nearest to the point. </summary>
		private const int MONITOR_DEFAULTTONEAREST = 2;
		/// <summary> Returns NULL. </summary>
		private const int MONITOR_DEFAULTTONULL = 0;
		/// <summary> Returns a handle to the primary display monitor. </summary>
		private const int MONITOR_DEFAULTTOPRIMARY = 1;
		

		[DllImport("User32.dll", ExactSpelling=true)]
		// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfrompoint
		private static extern IntPtr MonitorFromPoint(POINT pt, int flags);	

		[DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError = true)] 
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out]MONITORINFOEX info);

		public const uint GW_HWNDNEXT = 2;

		[DllImport("User32")]
		internal static extern IntPtr GetTopWindow(IntPtr hWnd);

		[DllImport("User32")]
		internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

		[DllImport("shcore.dll")]
		//https://docs.microsoft.com/de-de/windows/win32/api/shellscalingapi/nf-shellscalingapi-getdpiformonitor
		private static extern uint GetDpiForMonitor(IntPtr hmonitor, MDT dpiType, out uint dpiX, out uint dpiY);

		[DllImport("shcore.dll",SetLastError = true)]
		private static extern uint SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

		[DllImport("gdi32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);

		[DllImport("user32.dll")]
		private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOpts dwFlags);

		[DllImport("user32.dll", SetLastError=true)]
		private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError=true)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags flags);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		private class MONITORINFOEX {
			public int Size = Marshal.SizeOf(typeof(MONITORINFOEX));
			public RECT MonitorArea = new RECT();
			public RECT WorkingArea = new RECT();
			public int Flags = 0;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] Device = new char[32];
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT { 
			public int X;
			public int Y;
			public POINT(int x, int y) {
				X = x; 
				Y = y;
			} 
		} 

		[StructLayout(LayoutKind.Sequential)] 
		public struct RECT {
			public int Left; 
			public int Top; 
			public int Right;
			public int Bottom; 
		}

		/// <summary> MonitorDpiType </summary>
		private enum MDT {
			EFFECTIVE_DPI = 0,
			ANGULAR_DPI = 1,
			RAW_DPI = 2,
		}

		enum PROCESS_DPI_AWARENESS {
			PROCESS_DPI_UNAWARE = 0,
			PROCESS_SYSTEM_DPI_AWARE = 1,
			PROCESS_PER_MONITOR_DPI_AWARE = 2
		}

		enum DeviceCaps {
			VERTRES = 10,
			DESKTOPVERTRES = 117,
		}

		enum MonitorOpts : uint {
			MONITOR_DEFAULTTONULL = 0x00000000,
			MONITOR_DEFAULTTOPRIMARY = 0x00000001,
			MONITOR_DEFAULTTONEAREST = 0x00000002,
		}

		[Flags]
		private enum SetWindowPosFlags : uint {
			AsynchronousWindowPosition = 0x4000,
			DeferErase = 0x2000,
			DrawFrame = 0x0020,
			FrameChanged = 0x0020,
			HideWindow = 0x0080,
			DoNotActivate = 0x0010,
			DoNotCopyBits = 0x0100,
			IgnoreMove = 0x0002,
			DoNotChangeOwnerZOrder = 0x0200,
			DoNotRedraw = 0x0008,
			DoNotReposition = 0x0200,
			DoNotSendChangingEvent = 0x0400,
			IgnoreResize = 0x0001,
			IgnoreZOrder = 0x0004,
			ShowWindow = 0x0040,
		}

		#endregion
	}

}
// https://github.com/anaisbetts/PerMonitorDpi/blob/master/SafeNativeMethods.cs