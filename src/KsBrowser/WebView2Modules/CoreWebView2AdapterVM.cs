using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class CoreWebView2AdapterVM : ObjectVM, ICoreWebView2AdapterVM {

		public virtual void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			CoreWebView2 = coreWebView2;
			Tab = tab;
		}

		protected CoreWebView2 CoreWebView2 { get; private set; }

		[Hierarchy(HierarchyType.Reference)]
		protected WebView2ControllerVM Tab { get; private set; }
	}

	public interface ICoreWebView2AdapterVM : IObjectVM {
		void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2);
	}

}