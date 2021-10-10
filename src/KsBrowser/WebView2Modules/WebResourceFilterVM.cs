using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class WebResourceFilterVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);

			if (CoreWebView2 != null) {

				CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

				string uriPattern = "*"; // https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.addwebresourcerequestedfilter?view=webview2-dotnet-1.0.961.33
				var resourceContext = CoreWebView2WebResourceContext.All; // https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2webresourcecontext?view=webview2-dotnet-1.0.961.33
				// All				 0 Specifies all resources.
				// CspViolationReport	15 Specifies a CSP Violation Report.
				// Document			 1 Specifies a document resources.
				// EventSource		10 Specifies an EventSource API communication.
				// Fetch			 8 Specifies a Fetch API communication.
				// Font				 5 Specifies a font resource.
				// Image			 3 Specifies an image resources.
				// Manifest			12 Specifies a Web App Manifest.
				// Media			 4 Specifies another media resource such as a video.
				// Other			16 Specifies an other resource.
				// Ping				14 Specifies a Ping request.
				// Script			 6 Specifies a script resource.
				// SignedExchange	13 Specifies a Signed HTTP Exchange.
				// Stylesheet		 2 Specifies a CSS resources.
				// TextTrack		 9 Specifies a TextTrack resource.
				// Websocket		11 Specifies a WebSocket API communication.
				// XmlHttpRequest	 7 Specifies an XML HTTP request.

				CoreWebView2.AddWebResourceRequestedFilter(uriPattern, resourceContext);
			}
		}

		private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e) {
			// e.ResourceContext;
			// e.Request;
			// e.Response;
			// e.GetDeferral()
			
			Debug.WriteLine($"    WebResourceRequested {e.ResourceContext} {e.Request.Method} {e.Request.Uri}");
		}
	}

}