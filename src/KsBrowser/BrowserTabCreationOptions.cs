using System;
using KsWare.Presentation.ViewModels;

namespace KsWare.KsBrowser {

	public class BrowserTabCreationOptions : ITabItemCreationOptions {

		public BrowserTabCreationOptions(object newWindowRequest, ChromeTabItemVM referrer) {
			NewWindowRequest = newWindowRequest;
			Referrer = referrer;
		}

		public BrowserTabCreationOptions(Uri navigationUri, ChromeTabItemVM referrer = null) {
			NavigationUri = navigationUri;
			Referrer = referrer;
		}
		
		public ChromeTabItemVM Referrer { get; }

		public object NewWindowRequest { get; }

		public Uri NavigationUri { get; }

		/// <inheritdoc />
		public ChromeTabItemVM GetInstance() => throw new NotSupportedException();
		
	}

}