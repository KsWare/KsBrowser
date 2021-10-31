using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;

namespace KsWare.KsBrowser.CefSpecific {

	public class MyLifeSpanHandler : ILifeSpanHandler {

		private Logging.Log Log;
		private SemaphoreSlim _browserCreatedWaitHandle = new SemaphoreSlim(0);

		public MyLifeSpanHandler() {
			Log = new Logging.Log(this);
		}

		public event EventHandler<NewWindowRequestedEventArgs> NewWindowRequested;
		public event EventHandler<AfterCreatedEventArgs> AfterCreated;
		public event EventHandler<BeforeCloseEventArgs> BeforeClose;

		public Task WaitForBrowserCreatedAsync() {
			// workaround for Exception: The browser has not been initialized. Load can only be called after the underlying CEF browser is initialized (CefLifeSpanHandler::OnAfterCreated).'
			return _browserCreatedWaitHandle.WaitAsync();
		}

		/// <inheritdoc />
		void ILifeSpanHandler.OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			Log.Method($"");
			_browserCreatedWaitHandle.Release();
		}

		/// <inheritdoc />
		bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser) {
			Log.Method($"");
			if (browser.IsDisposed || browser.IsPopup) return false;
			return true;
		}

		/// <inheritdoc />
		void ILifeSpanHandler.OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			throw new NotImplementedException();
		}

		bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
			string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
			IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
			ref bool noJavascriptAccess, out IWebBrowser newBrowser)
		{
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] MyLifeSpanHandler.OnBeforePopup");
			var args = new NewWindowRequestedEventArgs(browserControl, browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, browserSettings, noJavascriptAccess);
			NewWindowRequested?.Invoke(this, args); // => CefSharpControllerVM.LifeSpanHandler_NewWindowRequested
			noJavascriptAccess = args.NoJavascriptAccess;
		
			// == DISABLED as long not working
			// newBrowser = args.NewBrowser;
			// return args.Handled;
			// System.Exception: 'returning true cancels popup creation,
			// if you return true newBrowser should be set to null.Previously no exception was thrown in this instance,
			// this exception has been added to reduce the number of support requests from people returning true and
			// setting newBrowser and expecting popups to work.'
			// == WORKAROUND
			newBrowser = null;
			return true;
			// == END 
		}
	}

	public class BeforeCloseEventArgs : EventArgs {

		public BeforeCloseEventArgs(IWebBrowser browserControl, IBrowser browser) {
			BrowserControl = browserControl;
			Browser = browser;
		}

		public IWebBrowser BrowserControl { get; }
		public IBrowser Browser { get; }
	}

	public class AfterCreatedEventArgs : EventArgs {

		public AfterCreatedEventArgs(IWebBrowser browserControl, IBrowser browser) {
			BrowserControl = browserControl;
			Browser = browser;
		}
		public IWebBrowser BrowserControl { get; }
		public IBrowser Browser { get; }
	}

	public class NewWindowRequestedEventArgs : EventArgs {

		public NewWindowRequestedEventArgs(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
			string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
			IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
			bool noJavascriptAccess) 
		{
			BrowserControl = browserControl;
			Browser = browser;
			Frame = frame;
			TargetUrl = targetUrl;
			TargetFrameName = targetFrameName;
			TargetDisposition = targetDisposition;
			UserGesture = userGesture;
			PopupFeatures = popupFeatures;
			WindowInfo = windowInfo;
			BrowserSettings = browserSettings;
			NoJavascriptAccess = noJavascriptAccess;
		}

		public IWebBrowser BrowserControl { get; }
		public IBrowser Browser { get; }
		public IFrame Frame { get; }
		public string TargetUrl { get; }
		public string TargetFrameName { get; }
		public WindowOpenDisposition TargetDisposition { get; }
		public bool UserGesture { get; }
		public IPopupFeatures PopupFeatures { get; }
		public IWindowInfo WindowInfo { get; }
		public IBrowserSettings BrowserSettings { get; }

		public bool NoJavascriptAccess { get; set; }
		public IWebBrowser NewBrowser { get; set; }

		public bool Handled { get; set; }
	}
}
