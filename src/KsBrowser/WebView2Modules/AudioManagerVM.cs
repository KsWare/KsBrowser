using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class AudioManagerVM : CoreWebView2AdapterVM {

		public void Init(CoreWebView2 coreWebView2, WebView2ControllerVM tab) {
			base.Init(tab, coreWebView2);
			if (coreWebView2 != null) {
				// CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsMutedChanged; //v>=1.0.1018
				// CoreWebView2.IsMutedChanged += CoreWebView2_IsDocumentPlayingAudioChanged; //v>=1.0.1018
			}
			UpdateTab();
		}

		public bool IsPlayingAudio { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool IsMuted { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		void CoreWebView2_IsMutedChanged(object sender, object e) {
			UpdateTab();
		}

		void CoreWebView2_IsDocumentPlayingAudioChanged(object sender, object e) {
			UpdateTab();
		}

		void UpdateTab() {
			if (CoreWebView2 == null) {
				IsPlayingAudio = false;
				IsMuted = false;
				return;
			}
			// IsPlayingAudio = CoreWebView2.IsDocumentPlayingAudio;
			// IsMuted = CoreWebView2.IsMuted;
			//
			// if (_coreWebView2.IsDocumentPlayingAudio) {
			// 	if (_coreWebView2.IsMuted) {
			// 		_tab.Title = "🔇 " + CoreWebView2.DocumentTitle;
			// 	}
			// 	else {
			// 		_tab.Title = "🔊 " + CoreWebView2.DocumentTitle;
			// 	}
			// }
			// else {
			// 	_tab.Title = CoreWebView2.DocumentTitle;
			// }
		}
	}

}