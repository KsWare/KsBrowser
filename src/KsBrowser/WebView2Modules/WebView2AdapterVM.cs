using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Wpf;

namespace KsWare.KsBrowser.WebView2Modules {

	public class WebView2AdapterVM : ObjectVM {

		public virtual void Init(WebView2 webView2, BrowserTabItemVM tab) {
			WebView2 = webView2;
			Tab = tab;
		}

		protected WebView2 WebView2 { get; private set; }

		[Hierarchy(HierarchyType.Reference)]
		protected BrowserTabItemVM Tab { get; private set; }
	}

	public interface IWebView2AdapterVM:IObjectVM {
		void Init(WebView2ControllerVM tab, WebView2 webView2);
	}

}