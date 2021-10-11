using System;
using CefSharp;

namespace KsWare.KsBrowser.CefSpecific {

	public class MyLifeSpanHandler : ILifeSpanHandler {

		public MyLifeSpanHandler() {

		}

		public event EventHandler<NewWindowRequestedEventArgs> NewWindowRequested;
		public event EventHandler<AfterCreatedEventArgs> AfterCreated;
		public event EventHandler<BeforeCloseEventArgs> BeforeClose;

		public bool DoClose(IWebBrowser browserControl, IBrowser browser) {
			if (browser.IsDisposed || browser.IsPopup) return false;
			return true;
		}

		public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser) {
			AfterCreated?.Invoke(this, new AfterCreatedEventArgs(browserControl,browser));
		}

		public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser) {
			BeforeClose?.Invoke(this, new BeforeCloseEventArgs(browserControl,browser));
		}

		public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
			string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
			IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
			ref bool noJavascriptAccess, out IWebBrowser newBrowser)
		{
			var args = new NewWindowRequestedEventArgs(browserControl, browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, browserSettings, noJavascriptAccess);
			NewWindowRequested?.Invoke(this, args);
			noJavascriptAccess = args.NoJavascriptAccess;
			newBrowser = args.NewBrowser;
			return args.Handled; 
			// System.Exception: 'returning true cancels popup creation,
			// if you return true newBrowser should be set to null.Previously no exception was thrown in this instance,
			// this exception has been added to reduce the number of support requests from people returning true and
			// setting newBrowser and expecting popups to work.'
		}
	}

	public class BeforeCloseEventArgs {

		public BeforeCloseEventArgs(IWebBrowser browserControl, IBrowser browser) {
			BrowserControl = browserControl;
			Browser = browser;
		}

		public IWebBrowser BrowserControl { get; }
		public IBrowser Browser { get; }
	}

	public class AfterCreatedEventArgs {

		public AfterCreatedEventArgs(IWebBrowser browserControl, IBrowser browser) {
			BrowserControl = browserControl;
			Browser = browser;
		}
		public IWebBrowser BrowserControl { get; }
		public IBrowser Browser { get; }
	}

	public class NewWindowRequestedEventArgs {

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
