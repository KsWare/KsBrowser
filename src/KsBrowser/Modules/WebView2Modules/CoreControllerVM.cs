using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;
using Point = System.Drawing.Point;

namespace KsWare.KsBrowser.WebView2Modules {

	public class CoreControllerVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public CoreControllerVM() {
			RegisterChildren(() => this);
		}

		public sealed override async void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);
			if (CoreWebView2 == null) return;

			// must be run in UI-thread
			//TODO seems not to be the correct hwnd
			var hWindow = new WindowInteropHelper(Application.Current.MainWindow).Handle;
			CompositionController = await CoreWebView2.Environment.CreateCoreWebView2CompositionControllerAsync(hWindow);
			Controller = await CoreWebView2.Environment.CreateCoreWebView2ControllerAsync(hWindow);

			CompositionController.CursorChanged += CompositionController_CursorChanged;

			Controller.AcceleratorKeyPressed += Controller_AcceleratorKeyPressed;
			Controller.GotFocus += Controller_GotFocus;
			Controller.LostFocus += Controller_LostFocus;
			Controller.RasterizationScaleChanged += Controller_RasterizationScaleChanged;
			Controller.ZoomFactorChanged += Controller_ZoomFactorChanged;
			Controller.MoveFocusRequested += Controller_MoveFocusRequested;
		}

		public CoreWebView2Controller Controller { get; private set; }

		public CoreWebView2CompositionController CompositionController { get; private set; }

		public ActionVM ClickAction { get; [UsedImplicitly] private set; }

		private void CompositionController_CursorChanged(object sender, object e) {
			Debug.WriteLine($"CompositionController_CursorChanged {CompositionController.SystemCursorId} {CompositionController.Cursor:X8}");
		}

		private void Controller_MoveFocusRequested(object sender, CoreWebView2MoveFocusRequestedEventArgs e) {
			Debug.WriteLine($"Controller_MoveFocusRequested {e.Reason}");
		}

		private void Controller_ZoomFactorChanged(object sender, object e) {
			Debug.WriteLine($"Controller_ZoomFactorChanged {Controller.ZoomFactor}");
		}

		private void Controller_RasterizationScaleChanged(object sender, object e) {
			Debug.WriteLine($"Controller_RasterizationScaleChanged {Controller.RasterizationScale}");
		}

		private void Controller_LostFocus(object sender, object e) {
			Debug.WriteLine($"Controller_LostFocus");
		}

		private void Controller_GotFocus(object sender, object e) {
			Debug.WriteLine($"Controller_GotFocus");
		}

		private void Controller_AcceleratorKeyPressed(object sender, CoreWebView2AcceleratorKeyPressedEventArgs e) {
			Debug.WriteLine($"Controller_AcceleratorKeyPressed Kind:{e.KeyEventKind} LParam:{e.KeyEventLParam} VKey:{e.VirtualKey}");
			// e.PhysicalKeyStatus.IsExtendedKey;
			// e.PhysicalKeyStatus.IsKeyReleased;
			// e.PhysicalKeyStatus.IsMenuKeyDown;
			// e.PhysicalKeyStatus.RepeatCount;
			// e.PhysicalKeyStatus.ScanCode;
			// e.PhysicalKeyStatus.WasKeyDown;
		}


		private void DoClick() {
			// CompositionController.SendMouseInput(CoreWebView2MouseEventKind.Wheel, CoreWebView2MouseEventVirtualKeys.Control, 180, new Point(300, 200));
			CompositionController.SendMouseInput(CoreWebView2MouseEventKind.LeftButtonDown, CoreWebView2MouseEventVirtualKeys.Control, 0, new Point(200, 200));
			CompositionController.SendMouseInput(CoreWebView2MouseEventKind.LeftButtonUp, CoreWebView2MouseEventVirtualKeys.Control, 0, new Point(200, 200));
		}
	}
}
