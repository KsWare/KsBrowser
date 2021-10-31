using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf;
using JetBrains.Annotations;
using KsWare.KsBrowser.Base;
using KsWare.KsBrowser.CefSpecific;
using KsWare.KsBrowser.Logging;
using KsWare.KsBrowser.Modules;
using KsWare.KsBrowser.Modules.CefModules;
using KsWare.KsBrowser.Overlays;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModelFramework;
using EventManager = KsWare.Presentation.EventManager;

namespace KsWare.KsBrowser {

	public class CefSharpControllerVM : WebContentPresenterVM, IViewControllerVM<ChromiumWebBrowser> {

		private readonly Log Log;
		

		/// <inheritdoc />
		public CefSharpControllerVM() {
			Log = new Log(this);
			Log.Method($".ctor");
			RegisterChildren(() => this);

			// NavigateBackCommand = initialize later
			// NavigateForwardCommand = initialize later

			// Modules.Add("Test", new TestModuleVM());
			Modules.Add("Audio", new AudioManagerVM());
			LifeSpanHandler.NewWindowRequested += LifeSpanHandler_NewWindowRequested;
		}

		public ChromiumWebBrowser ChromiumWebBrowser { get => Fields.GetValue<ChromiumWebBrowser>(); set => Fields.SetValue(value); }
		public ListVM<BaseMessageOverlayVM> MessageOverlays { get; [UsedImplicitly] private set; }
		public ErrorPresenterVM ErrorPresenter { get; [UsedImplicitly] private set; }
		public IAudioManager AudioManager => (IAudioManager)Modules["Audio"];
		public DownloadManagerVM DownloadManager { get; [UsedImplicitly] private set; }
		public MyLifeSpanHandler LifeSpanHandler { get; private set; } = new MyLifeSpanHandler();
		public ContextMenuHandlerVM ContextMenuHandler { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		void IViewControllerVM<ChromiumWebBrowser>.NotifyViewChanged(object sender, ValueChangedEventArgs<ChromiumWebBrowser> e) {
			Log.Method($"");
			if (ChromiumWebBrowser != null && e.NewValue != ChromiumWebBrowser) {
				throw new NotSupportedException();
				// clean old
				ChromiumWebBrowser = null;
			}

			if (ChromiumWebBrowser == null && e.NewValue != null) {
				Log.Message("initialize ChromiumWebBrowser");
				ChromiumWebBrowser = e.NewValue;

				NavigateBackCommand = ChromiumWebBrowser.BackCommand; OnPropertyChanged(nameof(NavigateBackCommand));
				NavigateForwardCommand = ChromiumWebBrowser.ForwardCommand; OnPropertyChanged(nameof(NavigateForwardCommand));
				RefreshCommand = ChromiumWebBrowser.ReloadCommand; OnPropertyChanged(nameof(RefreshCommand));

				ChromiumWebBrowser.AddressChanged += ChromiumWebBrowser_AddressChanged;
				ChromiumWebBrowser.TitleChanged += ChromiumWebBrowser_TitleChanged;
				ChromiumWebBrowser.IsBrowserInitializedChanged += ChromiumWebBrowser_IsBrowserInitializedChanged;
				ChromiumWebBrowser.LoadError += ChromiumWebBrowser_LoadError;
				ChromiumWebBrowser.LoadingStateChanged += ChromiumWebBrowser_LoadingStateChanged;

				// ChromiumWebBrowser.ConsoleMessage;
				// ChromiumWebBrowser.StatusMessage;
				// ChromiumWebBrowser.JavascriptMessageReceived;
				// ChromiumWebBrowser.FrameLoadStart;
				// ChromiumWebBrowser.FrameLoadEnd;
				// ChromiumWebBrowser.VirtualKeyboardRequested;

				ChromiumWebBrowser.LifeSpanHandler = LifeSpanHandler;
				ChromiumWebBrowser.RequestHandler = new MyRequestHandler(this);
				ChromiumWebBrowser.MenuHandler = ContextMenuHandler;
				
				// ChromiumWebBrowser.AccessibilityHandler;
				// ChromiumWebBrowser.BrowserSettings.Plugins;
				// ChromiumWebBrowser.AudioHandler.;		OK
				// ChromiumWebBrowser.DialogHandler;
				// ChromiumWebBrowser.DisplayHandler;
				// ChromiumWebBrowser.DownloadHandler = DownloadManager;
				// ChromiumWebBrowser.DragHandler;
				// ChromiumWebBrowser.FindHandler;
				// ChromiumWebBrowser.FocusHandler;
				// ChromiumWebBrowser.FrameHandler;
				// ChromiumWebBrowser.JavascriptObjectRepository;
				// ChromiumWebBrowser.JsDialogHandler;
				// ChromiumWebBrowser.JavascriptMessageReceived;
				// ChromiumWebBrowser.KeyboardHandler;
				// ChromiumWebBrowser.WpfKeyboardHandler;
				// ChromiumWebBrowser.LoadHandler;
				// ChromiumWebBrowser.MenuHandler;
				// ChromiumWebBrowser.RenderHandler;
				// ChromiumWebBrowser.RenderProcessMessageHandler;
				// ChromiumWebBrowser.RequestHandler;
				// ChromiumWebBrowser.ResourceRequestHandlerFactory;
				// ChromiumWebBrowser.WebBrowser;

				InitModules();
			}
		}

		private void InitModules() {
			// ChromiumWebBrowser can be null, this is intended
			foreach (var adapter in Children.OfType<CefModuleBaseVM>()) {
				adapter.Init(this, ChromiumWebBrowser);
			}
			foreach (var adapter in Modules.Values.OfType<CefModuleBaseVM>()) {
				adapter.Init(this, ChromiumWebBrowser);
			}
		}

		private void ChromiumWebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e) {
			Log.Method($"{e.IsLoading}");
		}

		private void ChromiumWebBrowser_LoadError(object sender, CefSharp.LoadErrorEventArgs e) {
			Log.Method($"{e.ErrorCode} {e.ErrorText} {e.Frame.Name} {e.FailedUrl}");
			//TODO ChromiumWebBrowser.LoadHtml("<html>" + e.ErrorText + "</html>", e.FailedUrl, Encoding.UTF8, true);
		}

		/// <remarks>
		/// --> <see cref="BrowserTabItemVM.WebContentPresenter_NewWindowRequested"/>
		/// ==> <see cref="Initialize">Initialize(PrivateNewWindowRequestedEventArgs)</see>
		/// </remarks>
		private void LifeSpanHandler_NewWindowRequested(object sender, CefSpecific.NewWindowRequestedEventArgs e) {
			Dispatcher.Invoke(DispatcherPriority.Normal, () =>
				EventManager.Raise<EventHandler<NewWindowRequestedEventArgs>, NewWindowRequestedEventArgs>(
					LazyWeakEventStore, nameof(NewWindowRequested),
					new Lazy<NewWindowRequestedEventArgs>(() =>
						new NewWindowRequestedEventArgs(new PrivateNewWindowRequestedEventArgs(e, this)))));
		}

		private void ChromiumWebBrowser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Log.Method($"{e.NewValue}");
		}

		private void ChromiumWebBrowser_TitleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Log.Method($"{e.NewValue}");
			DocumentTitle = e.NewValue as string;
		}

		private void ChromiumWebBrowser_AddressChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Log.Method($"{e.NewValue}");
			Address = e.NewValue as string;
			Source =  Uri.TryCreate(Address,UriKind.RelativeOrAbsolute, out var uri) ? uri : new Uri("about:unknown");
		}

		/// <inheritdoc />
		public override async void Initialize(object parameter) {
			Log.Method();
			switch (parameter) {
				case null:
					throw new ArgumentNullException(nameof(parameter));
				case PrivateNewWindowRequestedEventArgs args: {
					Log.Message($"NewWindowRequested");
					if (args.Referrer == null) throw new ArgumentNullException(nameof(PrivateNewWindowRequestedEventArgs.Referrer));

					// == BUG CefBrowser can not use the new ChromiumWebBrowser
					args.CoreArguments.NewBrowser = ChromiumWebBrowser;
					args.CoreArguments.Handled = true;
					// == WORKAROUND
					// await Task.Run(async () => { while (ChromiumWebBrowser.IsBrowserInitialized == false) { await Task.Delay(25).ConfigureAwait(false); } });
					await Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, () => { });
					await NavigateToUriAsync(new Uri(args.CoreArguments.TargetUrl, UriKind.Absolute)); 
					// == END
					// System.Exception: 'The browser has not been initialized. Load can only be called after the underlying CEF browser is initialized (CefLifeSpanHandler::OnAfterCreated).'
					// WORKAROUND: wait for OnAfterCreated

					break;
				}
				case Uri uri:
					Log.Message($"Uri");
					await LifeSpanHandler.WaitForBrowserCreatedAsync();
					await NavigateToUriAsync(uri);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(parameter), $"Type not supported. Type:'{parameter.GetType().FullName}'");
			}
		}

		/// <inheritdoc />
		protected override async void DoNavigate(object parameter) {
			var uri = await CommonTools.EnsureUrlAsync($"{parameter}");
			await NavigateToUriAsync(uri);
		}

		private async Task NavigateToUriAsync(Uri uri) {
			Log.Method($"{uri}");
			var result = await ChromiumWebBrowser.LoadUrlAsync(uri.AbsoluteUri);
			if (result.Success) {

			}
			else {
				Debug.WriteLine($"{result.HttpStatusCode} {result.ErrorCode}");
			}
		}

		public void MoveToNewWindow(DockingWindow window) {
			var parent = (UserControl)ChromiumWebBrowser.Parent;
			parent.Content = null;
			window.Content = ChromiumWebBrowser;
		}

		private class PrivateNewWindowRequestedEventArgs : EventArgs {

			public PrivateNewWindowRequestedEventArgs(CefSpecific.NewWindowRequestedEventArgs coreArguments, CefSharpControllerVM referrer) {
				CoreArguments = coreArguments;
				Referrer = referrer;
			}
			internal CefSpecific.NewWindowRequestedEventArgs CoreArguments { get; }
			internal CefSharpControllerVM Referrer { get; }
		}

	}

}
