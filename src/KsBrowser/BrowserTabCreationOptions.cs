using System;
using KsWare.Presentation.Controls;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser {

	public class BrowserTabCreationOptions : ITabCreationOptions {

		public BrowserTabCreationOptions(object newWindowRequest, TabItemVM referrer) {
			NewWindowRequest = newWindowRequest;
			Referrer = referrer;
		}

		public BrowserTabCreationOptions(Uri navigationUri, TabItemVM referrer = null) {
			NavigationUri = navigationUri;
			Referrer = referrer;
		}
		
		public TabItemVM Referrer { get; }

		public object NewWindowRequest { get; }

		public Uri NavigationUri { get; }
	}

}