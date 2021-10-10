using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using KsWare.KsBrowser.Extensions;
using KsWare.Presentation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace KsWare.KsBrowser {

	class OldInitialize {

		private WebView2 WebView2;
		private CoreWebView2 CoreWebView2;
		private IDispatcher Dispatcher;

		private void InitCreationProperties() {
			var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KsWare", "KsBrowser", "Default");
			Directory.CreateDirectory(cacheFolder);
			WebView2.CreationProperties = new CoreWebView2CreationProperties {
				UserDataFolder = cacheFolder,
			};
		}

		private async Task EnsureCoreWebView2Async(CoreWebView2Environment environment = null, string profileName = null) {
			if (CoreWebView2 != null) return; // already initialized

			if (environment == null && profileName == null) profileName = "Default";
			if (environment == null) {
				var options = new CoreWebView2EnvironmentOptions();
				var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KsWare", "KsBrowser", profileName);
				Directory.CreateDirectory(cacheFolder);
				environment = await CoreWebView2Environment.CreateAsync(null, cacheFolder, null).ConfigureAwait(true);
			}

			if (Dispatcher.IsInvokeRequired) {
				// await Application.Current.Dispatcher.BeginInvoke(() => WebView2.EnsureCoreWebView2Async(environment)).Task;
				await Dispatcher.RunAsync(() => WebView2.EnsureCoreWebView2Async(environment));
			}
			else {
				WebView2.VerifyAccess();
				await WebView2.EnsureCoreWebView2Async(environment);
			}
			CoreWebView2 = WebView2.CoreWebView2;
		}


	}
}
