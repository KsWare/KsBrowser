using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using KsWare.Presentation.Utilities;
using KsWare.Presentation.ViewFramework.Behaviors;
using NUnit.Framework;
using Point = System.Windows.Point;

namespace KsWare.Presentation.ChromeTabControl.Tests {

	public class WinApiTests {

		[SetUp]
		public void Setup() {
			WinApi.SetProcessDpiAwarenessPerMonitor();
		}

		[Test]
		public void GetCursorPos() {
			var p=WinApi.GetCursorPos();
			Console.WriteLine($"{p}");
			// TopRight:3759;-310
			// BottomRight: 3759;1609
		}
		[Test]
		public void GetCursorPosLoop() {
			Point o=new Point();
			while (true) {
				var p=WinApi.GetCursorPos();
				if (p != o) {
					o = p;
					Console.WriteLine($"{p}");
					Debug.WriteLine($"{p}");
				}
			}

			// TopRight:3759;-310
			// BottomRight: 3759;1609
		}

		[Test]
		public void Test1() {
			var m1=WinApi.GetMonitorRectFromPoint(new Point(0, 0));
			var dpi1 = WinApi.GetDpiForMonitorFromPoint(m1.TopLeft);
			Console.WriteLine($"1: {m1} {dpi1.DpiScaleX}");
			var m2=WinApi.GetMonitorRectFromPoint(new Point(m1.Right+1,m1.Top));
			Console.WriteLine($"2: {m2}");

			// STRANGE!!
			// net5    1: 0;0;2560;1440 1,25
			// net3.1: 1: 0;0;2048;1152 1
			// SOLUTION: SetProcessDpiAwarenessPerMonitor MUST be called 
			// 1: 0;0;2560;1440 1,25
			// 2: 2560;-310;1200;1920
		}

		[Test]
		public void Test2() {
			var fnc = new EnumMonitorsDelegate(
				(IntPtr hMonitor, IntPtr hdcMonitor, ref WinApi.RECT lprcMonitor, IntPtr data) => {
					var monitorInfo = new MONITORINFOEX();
					GetMonitorInfo(hMonitor, monitorInfo);
					var dpi=WinApi.GetDpiForMonitorFromPoint(new Point(lprcMonitor.Left, lprcMonitor.Top));
					Console.WriteLine($"{lprcMonitor.Left},{lprcMonitor.Top},{lprcMonitor.Right},{lprcMonitor.Bottom} {dpi.PixelsPerInchX}");
					return true;
				});
			EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, fnc, IntPtr.Zero);

			// STRANGE!! (net3.1/net5)
			// 0,0,2048,1152, 96
			// 2560,-310,3760,1610, 96
			// SOLUTION: SetProcessDpiAwarenessPerMonitor MUST be called 
			// 0,0,2560,1440 120
			// 2560,-310,3760,1610 96
		}

		[Test]
		public void Test3() {
			IntPtr lastMonitor = IntPtr.Zero;
			for (int x = -100; x < 5000; x+=1) {
				var hMonitor = MonitorFromPoint(new CustomWindowBehavior.POINT(x,0),0);
				if (hMonitor != IntPtr.Zero) {
					if (hMonitor != lastMonitor) {
						lastMonitor = hMonitor;
						Console.WriteLine($"Left:{x}");
					}
				}
				else {
					if (hMonitor != lastMonitor) {
						lastMonitor = hMonitor;
						Console.WriteLine($"Right{x-1}");
					}
				}
			}
		}

		[DllImport("User32.dll", ExactSpelling=true)]
		// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfrompoint
		private static extern IntPtr MonitorFromPoint(CustomWindowBehavior.POINT pt, int flags);	
		[DllImport("user32.dll")]
		static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
		delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref WinApi.RECT lprcMonitor, IntPtr dwData);
		[DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError = true)] 
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out]MONITORINFOEX info);
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		private class MONITORINFOEX {
			public int Size = Marshal.SizeOf(typeof(MONITORINFOEX));
			public WinApi.RECT MonitorArea = new WinApi.RECT();
			public WinApi.RECT WorkingArea = new WinApi.RECT();
			public int Flags = 0;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] Device = new char[32];
		}
		[DllImport("shcore.dll")]
		private static extern uint GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);
	}
}