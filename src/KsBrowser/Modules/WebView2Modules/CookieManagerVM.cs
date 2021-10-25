using System.Text;
using System.Windows;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class CookieManagerVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public CookieManagerVM() {
			RegisterChildren(() => this);

			// CoreWebView2.CookieManager.AddOrUpdateCookie();
			// CoreWebView2.CookieManager.CopyCookie();
			// CoreWebView2.CookieManager.CreateCookie();
			// CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie();
			// CoreWebView2.CookieManager.DeleteAllCookies();
			// CoreWebView2.CookieManager.DeleteCookie();
			// CoreWebView2.CookieManager.DeleteCookies();
			// CoreWebView2.CookieManager.DeleteCookiesWithDomainAndPath();
			// CoreWebView2.CookieManager.GetCookiesAsync()
		}

		public ActionVM GetCookiesAction { get; [UsedImplicitly] private set; }
		public ActionVM AddOrUpdateCookieAction { get; [UsedImplicitly] private set; }
		public ActionVM DeleteAllCookiesAction { get; [UsedImplicitly] private set; }
		public ActionVM DeleteCookiesAction { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);
		}

		private async void DoGetCookies() {
			var cookieList = await CoreWebView2.CookieManager.GetCookiesAsync("https://www.bing.com");
			var cookieResult = new StringBuilder(cookieList.Count + " cookie(s) received from https://www.bing.com\n");
			for (var i = 0; i < cookieList.Count; ++i) {
				var cookie = CoreWebView2.CookieManager.CreateCookieWithSystemNetCookie(cookieList[i].ToSystemNetCookie());
				cookieResult.Append($"\n{cookie.Name} {cookie.Value} {(cookie.IsSession ? "[session cookie]" : cookie.Expires.ToString("G"))}");
			}

			MessageBox.Show(cookieResult.ToString(), "GetCookiesAsync");
		}

		void DoAddOrUpdateCookie() {
			CoreWebView2Cookie cookie = CoreWebView2.CookieManager.CreateCookie("CookieName", "CookieValue", ".bing.com", "/");
			CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
		}

		void DoDeleteAllCookies() {
			CoreWebView2.CookieManager.DeleteAllCookies();
		}

		void DoDeleteCookies() {
			CoreWebView2.CookieManager.DeleteCookiesWithDomainAndPath("CookieName", ".bing.com", "/");
		}
	}

}
