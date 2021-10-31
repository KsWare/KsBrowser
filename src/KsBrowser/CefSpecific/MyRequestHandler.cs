using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CefSharp;
using CefSharp.Handler;
using KsWare.KsBrowser.Logging;
using KsWare.KsBrowser.Overlays;

namespace KsWare.KsBrowser.CefSpecific {

	public class MyRequestHandler : RequestHandler {

		private readonly Log Log;
		private readonly CefSharpControllerVM _webContentPresenter;

		/// <inheritdoc />
		public MyRequestHandler(CefSharpControllerVM webContentPresenter) {
			Log = new Log(this);
			_webContentPresenter = webContentPresenter;
		}

		public List<string> AllowCertificateErrorOneTime { get; } = new List<string>();
		public List<string> AllowCertificateErrorForHost { get; } = new List<string>();

		public string CertificateErrorTextTemplate =
			@"One or more of the certificates of this website are invalid, so we cannot guarantee their authenticity. This happens if the owner of the website has not updated the certificate in time or if it is a fake website created by fraudsters. If you visit such a website, the risk of attack increases.

Error Code: {ErrorCode}

Web address: {WebAddress}

I know the risk and would like to continue.";

		// https://github.com/cefsharp/CefSharp/issues/2887
		// https://stackoverflow.com/questions/35555754/how-to-bypass-ssl-error-cefsharp-winforms/35564187#35564187
		protected override bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser,
			CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) {
			Log.Method($"{errorCode}");
			//NOTE: We also suggest you wrap callback in a using statement or explicitly execute callback.Dispose as callback wraps an unmanaged resource.

			//Example #1
			//Return true and call IRequestCallback.Continue() at a later time to continue or cancel the request.
			//In this instance we'll use a Task, typically you'd invoke a call to the UI Thread and display a Dialog to the user
			Task.Run(async() => {
				//NOTE: When executing the callback in an async fashion need to check to see if it's disposed
				if (!callback.IsDisposed) {
					using (callback) {
						var uri = new Uri(requestUrl, UriKind.Absolute);

						if (AllowCertificateErrorOneTime.Contains(requestUrl)) {
							AllowCertificateErrorOneTime.Remove(requestUrl);
							callback.Continue(true);
							return;
						}
						if (AllowCertificateErrorForHost.Contains(uri.Host)) {
							callback.Continue(true);
							return;
						}

						var overlay = new MessagePresenterVM(_webContentPresenter.MessageOverlays);
						if (await overlay.Show(
							CertificateErrorTextTemplate.Replace("{WebAddress}",requestUrl).Replace("{ErrorCode}",$"{errorCode}"), 
							"Certificate Error", MessageBoxButton.YesNo,
							MessageBoxImage.Stop, MessageBoxResult.No) == MessageBoxResult.Yes) {
							callback.Continue(true);
							return;
						}

						//We'll allow the expired certificate from badssl.com
						if (requestUrl.ToLower().Contains("https://expired.badssl.com/")) {
							callback.Continue(true);
						}
						else {
							callback.Continue(false);
						}
					}
				}
			});

			return true;

			//Example #2
			//Execute the callback and return true to immediately allow the invalid certificate
			//callback.Continue(true); //Callback will Dispose it's self once exeucted
			//return true;

			//Example #3
			//Return false for the default behaviour (cancel request immediately)
			//callback.Dispose(); //Dispose of callback
			//return false;
		}

		/// <inheritdoc />
		protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
			Log.Method($"{(userGesture?"userGesture ":"")}{(isRedirect?"isRedirect ":"")}{request.ResourceType} {request.Method} {request.Url}");
			return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
		}

		/// <inheritdoc />
		protected override void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			Log.Method($"");
			base.OnDocumentAvailableInMainFrame(chromiumWebBrowser, browser);
		}

		/// <inheritdoc />
		protected override bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
			WindowOpenDisposition targetDisposition, bool userGesture) {
			Log.Method($"{targetDisposition}{(userGesture?" userGesture":"")} {targetUrl}");
			return base.OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture);
		}

		/// <inheritdoc />
		protected override void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath) {
			Log.Method($"{pluginPath}");
			base.OnPluginCrashed(chromiumWebBrowser, browser, pluginPath);
		}

		/// <inheritdoc />
		protected override bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) {
			Log.Method($"");
			return base.OnQuotaRequest(chromiumWebBrowser, browser, originUrl, newSize, callback);
		}

		/// <inheritdoc />
		protected override void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status) {
			Log.Method($"");
			base.OnRenderProcessTerminated(chromiumWebBrowser, browser, status);
		}

		/// <inheritdoc />
		protected override void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			Log.Method($"");
			base.OnRenderViewReady(chromiumWebBrowser, browser);
		}

		/// <inheritdoc />
		protected override bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port,
			X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) {
			Log.Method($"");
			return base.OnSelectClientCertificate(chromiumWebBrowser, browser, isProxy, host, port, certificates, callback);
		}
	}

}