using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace KsWare.KsBrowser.WebView2Modules {

	public class WebViewCrashManagerVM : WebView2AdapterVM, ICoreWebView2AdapterVM {

		// handled by WebView2Controller

		/// <inheritdoc />
		public WebViewCrashManagerVM() {
		}

		/// <inheritdoc />
		public override void Init(WebView2 webView2, BrowserTabItemVM tab) {
			base.Init(webView2, tab);
		}

		/// <inheritdoc />
		public void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			// base.Init(coreWebView2, tab);
			// CoreWebView2.ProcessFailed += WebView2_ProcessFailed;
		}

		private CoreWebView2 CoreWebView2 => WebView2.CoreWebView2;


	}

}