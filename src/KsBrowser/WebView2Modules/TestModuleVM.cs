using System.Diagnostics;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class TestModuleVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public TestModuleVM() {
			RegisterChildren(() => this);
			CrashBrowserAction.SetCanExecute("CoreWebView2NotAvailable", false);
		}

		public ActionVM CrashBrowserAction { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);
			CrashBrowserAction.SetCanExecute("CoreWebView2NotAvailable", CoreWebView2 != null);
		}

		private void DoCrashBrowser() {
			// WebView2.Dispose();
			var process = Process.GetProcessById((int)CoreWebView2.BrowserProcessId);
			// process.Kill(true); freezes
			process.Kill();
		}
	}
}
