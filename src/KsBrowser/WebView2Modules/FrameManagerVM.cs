using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class FrameManagerVM : CoreWebView2AdapterVM {

		private readonly List<CoreWebView2Frame> _webViewFrames = new List<CoreWebView2Frame>();

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);

			if (CoreWebView2 != null) {
				CoreWebView2.FrameCreated+=CoreWebView2_FrameCreated;
			}
			else {
				_webViewFrames.Clear();
			}
		}

		private void CoreWebView2_FrameCreated(object? sender, CoreWebView2FrameCreatedEventArgs e) {
			Debug.WriteLine($"    FrameCreated {e.Frame.Name}");
			_webViewFrames.Add(e.Frame);
			e.Frame.Destroyed += (frameDestroyedSender, frameDestroyedArgs) => {
				var frameToRemove = _webViewFrames.SingleOrDefault(r => r.IsDestroyed() == 1);
				if (frameToRemove != null) _webViewFrames.Remove(frameToRemove);
			};
		}

		public string ToString() {
			string result = "";
			for (var i = 0; i < _webViewFrames.Count; i++) {
				if (i > 0) result += "; ";
				result += i.ToString() + " " + (string.IsNullOrEmpty(_webViewFrames[i].Name) ? "<empty_name>" : _webViewFrames[i].Name);
			}

			return string.IsNullOrEmpty(result) ? "no iframes available." : result;
		}
	}

}