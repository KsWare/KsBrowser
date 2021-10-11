using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Threading;
using KsWare.Presentation;

namespace KsWare.KsBrowser.Tools {

	public static class CommonTools {

		public static bool EnsureUrl(ref string url) {
			if (Uri.TryCreate(url, UriKind.Absolute, out _)) return true;
			url = "https://" + url;
			if (Uri.TryCreate(url, UriKind.Absolute, out _)) return true;
			return false;
		}
		
		public static async Task<Uri> EnsureUrlAsync(string s) {
			var url = s;
			Uri uri = null;

			if (string.IsNullOrEmpty(s)) {
				uri = new Uri("about:blank");
			}

			if (uri == null) {
				if (s.StartsWith("http:") || s.StartsWith("https:") || s.StartsWith("file:")) {
					if (EnsureUrl(ref url)) uri = new Uri(url, UriKind.Absolute);
				}
			}

			if (uri == null) {
				var segments = s.Split('\\', '/');
				//www.ksware.de/sub
				try {
					var hostEntry = await Dns.GetHostEntryAsync(segments[0]);
					url = "https://" + s;
					if (EnsureUrl(ref url)) uri = new Uri(url, UriKind.Absolute);
				}
				catch { }
			}

			if (uri == null) {
				url = "https://www.google.com/search?q=" + HttpUtility.UrlEncode(s);
				if (EnsureUrl(ref url)) uri = new Uri(url, UriKind.Absolute);
			}

			return uri;
		}

		/// <summary>
		/// Wait for render tasks done.
		/// </summary>
		/// <remarks>
		/// This lets create the UI controls for data bound templates.
		/// </remarks>
		public static void WaitForRender() {
			ApplicationDispatcher.Invoke(DispatcherPriority.Render, () => {});
		}
	}
}
