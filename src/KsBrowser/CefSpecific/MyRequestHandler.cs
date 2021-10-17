using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CefSharp;
using CefSharp.Handler;

namespace KsWare.KsBrowser.CefSpecific {

	public class MyRequestHandler : RequestHandler {

		public List<string> AllowCertificateErrorOneTime { get; } = new List<string>();
		public List<string> AllowCertificateErrorForHost { get; } = new List<string>(){"ksware.de"};

		public string CertificateErrorTextTemplate =
			@"One or more of the certificates of this website are invalid, so we cannot guarantee their authenticity. This happens if the owner of the website has not updated the certificate in time or if it is a fake website created by fraudsters. If you visit such a website, the risk of attack increases.

Error Code: {ErrorCode}

Web address: {WebAddress}

I know the risk and would like to continue.";

		// https://github.com/cefsharp/CefSharp/issues/2887
		// https://stackoverflow.com/questions/35555754/how-to-bypass-ssl-error-cefsharp-winforms/35564187#35564187
		protected override bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser,
			CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] RequestHandler.OnCertificateError {errorCode}");
			//NOTE: We also suggest you wrap callback in a using statement or explicitly execute callback.Dispose as callback wraps an unmanaged resource.

			//Example #1
			//Return true and call IRequestCallback.Continue() at a later time to continue or cancel the request.
			//In this instance we'll use a Task, typically you'd invoke a call to the UI Thread and display a Dialog to the user
			Task.Run(() => {
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
						
						if (MessageBox.Show(CertificateErrorTextTemplate.Replace("{WebAddress}",requestUrl).Replace("{ErrorCode}",$"{errorCode}"), "Certificate Error", MessageBoxButton.YesNo,
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
	}

}