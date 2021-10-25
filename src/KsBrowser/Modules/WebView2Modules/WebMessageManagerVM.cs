using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class WebMessageManagerVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public WebMessageManagerVM() {
			RegisterChildren(() => this);
		}

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);

			if (CoreWebView2 != null) {
				CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
			}
		}

		private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e) {
			// e.Source						string
			// e.WebMessageAsJson			string
			// e.TryGetWebMessageAsString() string
			
			var message = e.TryGetWebMessageAsString();
			Debug.WriteLine($"    WebMessageReceived {message}");

			CoreWebView2.PostWebMessageAsString("PostWebMessageAsString"); //TEST
		}
	}
}
