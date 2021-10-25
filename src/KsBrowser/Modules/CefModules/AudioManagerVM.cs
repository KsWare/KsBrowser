using System;
using CefSharp;
using CefSharp.Structs;
using CefSharp.Wpf;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Modules.CefModules {

	public class AudioManagerVM : CefModuleBaseVM, IAudioManager, IAudioHandler {

		//TODO AudioManagerVM.IsMuted

		/// <inheritdoc />
		public override void Init(CefSharpControllerVM presenter, ChromiumWebBrowser browserControl) {
			base.Init(presenter, browserControl);
			if(presenter == null || browserControl.AudioHandler != null) return;
			ChromiumWebBrowser.AudioHandler = this;
		}

		public bool IsPlayingAudio { get => Fields.GetValue<bool>(); private set => Fields.SetValue(value); }

		public bool IsMuted { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); } 

		// IAudioHandler
		// https://cefsharp.github.io/api/94.4.x/html/T_CefSharp_IAudioHandler.htm

		/// <inheritdoc />
		bool IAudioHandler.GetAudioParameters(IWebBrowser chromiumWebBrowser, IBrowser browser, ref AudioParameters parameters) {
			// parameters.ChannelLayout;
			// parameters.FramesPerBuffer;
			// parameters.SampleRate;
			return true; // Return true to proceed with audio stream capture, or false to cancel it
			//if we return false no OnAudioStreamStarted..OnAudioStreamStopped event will occur
		}

		/// <inheritdoc />
		void IAudioHandler.OnAudioStreamStarted(IWebBrowser chromiumWebBrowser, IBrowser browser, AudioParameters parameters, int channels) {
			IsPlayingAudio = true;
		}

		/// <inheritdoc />
		void IAudioHandler.OnAudioStreamPacket(IWebBrowser chromiumWebBrowser, IBrowser browser, IntPtr data, int noOfFrames, long pts) {
		}

		/// <inheritdoc />
		void IAudioHandler.OnAudioStreamStopped(IWebBrowser chromiumWebBrowser, IBrowser browser) {
			IsPlayingAudio = false;
		}

		/// <inheritdoc />
		void IAudioHandler.OnAudioStreamError(IWebBrowser chromiumWebBrowser, IBrowser browser, string errorMessage) {
		}
	}

	public class CefModuleBaseVM : ObjectVM, ICefAdapterVM {
		// CoreWebView2AdapterVM

		public virtual void Init(CefSharpControllerVM presenter, ChromiumWebBrowser browserControl){
			ChromiumWebBrowser = browserControl;
			Presenter = presenter;
		}

		protected ChromiumWebBrowser ChromiumWebBrowser { get; private set; }

		[Hierarchy(HierarchyType.Reference)]
		protected CefSharpControllerVM Presenter { get; private set; }
	}

	public interface ICefAdapterVM : IObjectVM {
		// ICoreWebView2AdapterVM
		void Init(CefSharpControllerVM presenter, ChromiumWebBrowser browserControl);
	}

}
