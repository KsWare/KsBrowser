using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.KsBrowser.Base;
using KsWare.KsBrowser.Tools;
using KsWare.KsBrowser.WebView2Modules;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using EventManager = KsWare.Presentation.EventManager;

namespace KsWare.KsBrowser {

	public class WebView2ControllerVM : WebContentPresenterVM, IViewControllerVM<WebView2> {
		private bool _resetWebView2;

		/// <inheritdoc />
		public WebView2ControllerVM() {
			Debug.WriteLine($"new WebView2ControllerVM");
			RegisterChildren(() => this);

			NavigateBackCommand = new RelayCommand(() => WebView2?.GoBack(), () => WebView2?.CanGoBack??false);
			NavigateForwardCommand = new RelayCommand(() => WebView2?.GoForward(), () => WebView2?.CanGoForward??false);

			Modules.Add("Test", new TestModuleVM());
		}

		public WebView2 WebView2 { get => Fields.GetValue<WebView2>(); private set => Fields.SetValue(value); }
		private CoreWebView2 CoreWebView2 { get; set; }

		public EventLoggerVM EventLogger { get; [UsedImplicitly] private set; }
		public WebViewCrashManagerVM WebViewCrashManager { get; [UsedImplicitly] private set; }
		public WebResourceFilterVM WebResourceFilter { get; [UsedImplicitly] private set; }
		public FrameManagerVM FrameManager { get; [UsedImplicitly] private set; }
		public AudioManagerVM AudioManager { get; [UsedImplicitly] private set; }
		public DownloadManagerVM DownloadManager { get; [UsedImplicitly] private set; }
		public CookieManagerVM CookieManager { get; [UsedImplicitly] private set; }
		public CoreControllerVM Controller { get; [UsedImplicitly] private set; }
		
		public Uri Source { get => Fields.GetValue<Uri>(); set => Fields.SetValue(value); }

		public Dictionary<string, IObjectVM> Modules { get; }= new Dictionary<string, IObjectVM>();

		protected override async void DoNavigate(object parameter) {
			var uri = await CommonTools.EnsureUrlAsync($"{parameter}");
			await NavigateToUriAsync(uri);
		}

		/// <inheritdoc />
		// implements IWebView2ControllerVM.NotifyViewChanged
		public void NotifyViewChanged(object sender, ValueChangedEventArgs<WebView2> e) {
			Debug.WriteLine($"===== NotifyViewChanged {(e.NewValue != null ? "WebView2" : "null")} =====");
			var oldValue = WebView2;

			if (oldValue != null) {
				_resetWebView2 = true;
				// The old WebView2 and CoreWebView2 will be invalid after this call
				// if required backup all relevant values
				// WebView2.Source
				// WebView2.CreationProperties
				// WebView2.ZoomFactor
				// WebView2.CoreWebView2.Environment;
				// WebView2.CoreWebView2.Settings
			}

			WebView2 = e.NewValue;
			CoreWebView2 = WebView2?.CoreWebView2;
			if (CoreWebView2 != null) throw new NotSupportedException(); // should not be! who has already initialized this?
			InitWebView2Modules();

			if (WebView2 != null) {
				Debug.WriteLine($"View assigned.");
			}
		}

		private void InitWebView2Modules() {
			// WebView2 can be null, this is intended
			foreach (var adapter in Children.OfType<IWebView2AdapterVM>()) {
				adapter.Init(this, WebView2);
			}
			foreach (var adapter in Modules.Values.OfType<IWebView2AdapterVM>()) {
				adapter.Init(this, WebView2);
			}

			// CoreWebView2 is dependent on WebView2, if WebView2 changes, CoreWebView2 changes too (null at this moment)
			InitCoreWebView2Modules();
		}

		private void InitCoreWebView2Modules() {
			// CoreWebView2 can be null, this is intended
			foreach (var adapter in Children.OfType<ICoreWebView2AdapterVM>()) {
				adapter.Init(this, WebView2?.CoreWebView2);
			}
			foreach (var adapter in Modules.Values.OfType<ICoreWebView2AdapterVM>()) {
				adapter.Init(this, WebView2?.CoreWebView2);
			}
		}

		/// <summary>
		/// Called after awaiting WebView2.EnsureCoreWebView2Async()
		/// </summary>
		private async Task OnCoreWebView2CreatedAsync() {
			CoreWebView2 = WebView2.CoreWebView2;

			// CoreWebView2.CookieManager				=> CookieManager
			// CoreWebView2.DocumentTitle				managed
			// CoreWebView2.Source						managed
			// CoreWebView2.BrowserProcessId
			// CoreWebView2.CanGoBack					managed
			// CoreWebView2.CanGoForward				managed
			// CoreWebView2.ContainsFullScreenElement
			// CoreWebView2.Environment
			// CoreWebView2.IsSuspended
			// CoreWebView2.Settings					managed

			SyncSettings(Settings, CoreWebView2.Settings);
			RegisterCoreWebView2Events(CoreWebView2);
			((RelayCommand)RefreshCommand).OnCanExecuteChanged(); // TODO with no url we can not refresh!, or?

			InitCoreWebView2Modules();

			// await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("alert('OnDocumentCreate')");
			// CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
			// await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);").ConfigureAwait(true);
			//await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));").ConfigureAwait(true);
		}

		private void SyncSettings(SettingVM settings, CoreWebView2Settings coreWebView2Settings) {
			// UserAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36 Edg/94.0.992.38"
			// all true, except IsPasswordAutosaveEnabled
		}

		public SettingVM Settings { get; private set; }

		private void RegisterCoreWebView2Events(CoreWebView2 coreWebView2) {
			coreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
		 //	coreWebView2.ProcessFailed
		 //	coreWebView2.NavigationStarting
		 //	coreWebView2.NavigationCompleted;
			coreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
			coreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
			coreWebView2.SourceChanged += CoreWebView2_SourceChanged;
		 //	coreWebView2.ContentLoading
		 //	coreWebView2.DOMContentLoaded
		 // coreWebView2.DownloadStarting					=> DownloadManager
		 //	coreWebView2.WebMessageReceived					=> WebMessageManager
		 // coreWebView2.WebResourceRequested				=> WebResourceFilter
		 //	coreWebView2.WebResourceResponseReceived 
		 // coreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged; //TODO handle fullscreen request
		 // coreWebView2.ClientCertificateRequested += CoreWebView2_ClientCertificateRequested;
		 //	coreWebView2.FrameNavigationStarting 
		 //	coreWebView2.FrameNavigationCompleted
		 // coreWebView2.FrameCreated						=> FrameManager
		 //	coreWebView2.ScriptDialogOpening
			coreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
		 // coreWebView2.IsDocumentPlayingAudioChanged => AudioManager	//v>=1.0.1018
		 // coreWebView2.IsMutedChanged => AudioManager					//v>=1.0.1018

		 //	coreWebView2.Environment.BrowserProcessExited;
		 //	coreWebView2.Environment.NewBrowserVersionAvailable
		}

		/// <summary> SourceChanged is raised when the Source property changes. </summary>
		private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e) {
			// SourceChanged is raised when navigating to a different site or fragment navigations.
			// It is not raised for other types of navigations such as page refreshes
			// or history.pushState with the same URL as the current page.
			// This event is raised before ContentLoading for navigation to a new document.

			// e.IsNewDocument

			var source = CoreWebView2.Source;
			// Debug.WriteLine($"    SourceChanged {(e.IsNewDocument ? "NewDocument" : "")} {source}");
			Address = source;
			Uri.TryCreate(source, UriKind.Absolute, out var sourceUri); //TODO handle error
			Source = sourceUri;
		}

		protected override bool CanRefresh() {
			return CoreWebView2 != null;
		}

		protected override void DoRefresh() {
			WebView2?.CoreWebView2?.Reload();
		}

		private void CoreWebView2_HistoryChanged(object sender, object e) {
			((RelayCommand)NavigateBackCommand).OnCanExecuteChanged();
			((RelayCommand)NavigateForwardCommand).OnCanExecuteChanged();
		}

		public async Task NavigateToUriAsync(Uri uri) {
			if (WebView2.CoreWebView2 == null) {
				uri ??= new Uri("about:blank");
				var environment = await CreateCoreWebView2EnvironmentAsync("Default").ConfigureAwait(true);
				await WebView2.EnsureCoreWebView2Async(environment).ConfigureAwait(true);
				await OnCoreWebView2CreatedAsync();
			}
			WebView2.CoreWebView2.Navigate(uri.AbsoluteUri);
			Address = uri.ToString();
		}

		private async Task<CoreWebView2Environment> CreateCoreWebView2EnvironmentAsync(string profileName) {
			if (CoreWebView2 != null) throw new InvalidOperationException();
			var options = new CoreWebView2EnvironmentOptions {
				// AdditionalBrowserArguments =,
				// AllowSingleSignOnUsingOSPrimaryAccount = ,
				// Language = ,
				// TargetCompatibleBrowserVersion = 
			};
			var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KsWare", "KsBrowser", profileName);
			Directory.CreateDirectory(cacheFolder);

			var environment = await CoreWebView2Environment.CreateAsync(null, cacheFolder, null);
			// var hControl = ((HwndSource)PresentationSource.FromVisual(WebView2))?.Handle;
			// var hWindow = new WindowInteropHelper(App.Current.MainWindow).Handle;

			Debug.WriteLine($"    Environment created");
			Debug.WriteLine($"       Version: {environment.BrowserVersionString}");

			// environment.BrowserProcessExited += ; 
			// environment.NewBrowserVersionAvailable += Environment_NewBrowserVersionAvailable;
			// var compositionController = await environment.CreateCoreWebView2CompositionControllerAsync(hWindow);
			// var controller = await environment.CreateCoreWebView2ControllerAsync(hWindow);
			// var pointerInfo = environment.CreateCoreWebView2PointerInfo();
			// pointerInfo.PixelLocation = new Point(10, 10);
			// compositionController.SendPointerInput(CoreWebView2PointerEventKind.Down, pointerInfo);
			// compositionController.SendPointerInput(CoreWebView2PointerEventKind.Up, pointerInfo);
			// compositionController.SendMouseInput(CoreWebView2MouseEventKind.Wheel, CoreWebView2MouseEventVirtualKeys.Control, 1, new Point(10,10));
			// compositionController.Cursor;
			// compositionController.RootVisualTarget;
			// compositionController.SystemCursorId;
			// compositionController.CursorChanged +=
			// controller.Bounds;
			// controller.BoundsMode;
			// controller.CoreWebView2;
			// controller.DefaultBackgroundColor;
			// controller.IsVisible;
			// controller.ParentWindow;
			// controller.RasterizationScale;
			// controller.ShouldDetectMonitorScaleChanges;
			// controller.ZoomFactor;
			// controller.Close();
			// controller.MoveFocus(CoreWebView2MoveFocusReason.Next);
			// controller.NotifyParentWindowPositionChanged();
			// controller.SetBoundsAndZoomFactor(new Rectangle(0,0,100,100),1);

			// var request = environment.CreateWebResourceRequest(uri,"get",null,null);
			// environment.CreateWebResourceResponse(...)
			return environment;
		}

		private void Environment_NewBrowserVersionAvailable(object sender, object e) {
			
		}

		private void CoreWebView2_DocumentTitleChanged(object sender, object e) {
			DocumentTitle = CoreWebView2.DocumentTitle;
		}

		/// <inheritdoc cref="CoreWebView2.WindowCloseRequested"/>
		/// <seealso cref="CoreWebView2.WindowCloseRequested"/>
		private void CoreWebView2_WindowCloseRequested(object sender, object e) {
			EventManager.Raise<EventHandler<CloseRequestedEventArgs>, CloseRequestedEventArgs>(LazyWeakEventStore, nameof(CloseRequested), new CloseRequestedEventArgs());
		}

		private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e) {
			// e.Handled;
			// e.IsUserInitiated;
			// e.NewWindow;
			// e.Uri;
			// e.GetDeferral()

			// (A) same window
			// e.NewWindow = WebView2.CoreWebView2;
			// e.Handled = true;

			// (B) new tab
			//TabHost.CreateNewTab(new BrowserTabCreationOptions(e, this));
			EventManager.Raise<EventHandler<NewWindowRequestedEventArgs>, NewWindowRequestedEventArgs>(LazyWeakEventStore, nameof(NewWindowRequested), new Lazy<NewWindowRequestedEventArgs>(()=>new NewWindowRequestedEventArgs(new PrivateNewWindowRequestedEventArgs(e, this))));
			// => newTAB.Initialize(...)

			// (C) new window (this process)
			// Shell.NewWindow(this, e);

			// (D) new window (new process)
			// I don't think this is possible (so easily). Reference from new CoreWebView2 would required.

			// (E) new window (edge)
			// this easy, do nothing ;-) 
		}


		public override async void Initialize(object parameter) {
			switch (parameter) {
				case null:
					throw new ArgumentNullException(nameof(parameter));
				case PrivateNewWindowRequestedEventArgs args: {
					Debug.WriteLine($"InitializeNewPresenter NewWindowRequested");
					if (args.Referrer == null) throw new ArgumentNullException(nameof(PrivateNewWindowRequestedEventArgs.Referrer));

					var environment = args.Referrer.WebView2.CoreWebView2.Environment;
					await WebView2.EnsureCoreWebView2Async(environment);
					await OnCoreWebView2CreatedAsync();

					args.CoreArguments.NewWindow = WebView2.CoreWebView2;
					args.CoreArguments.Handled = true;
					args.Deferral.Complete();
					break;
				}
				case Uri uri:
					Debug.WriteLine($"InitializeNewPresenter {uri}");
					await NavigateToUriAsync(uri);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(parameter), $"Type not supported. Type:'{parameter.GetType().FullName}'");
			}
		}

		private class PrivateNewWindowRequestedEventArgs {

			public PrivateNewWindowRequestedEventArgs(CoreWebView2NewWindowRequestedEventArgs args, WebView2ControllerVM referrer) {
				Referrer = referrer;
				CoreArguments = args;
				Deferral = args.GetDeferral();
			}
			internal WebView2ControllerVM Referrer { get; }
			internal CoreWebView2Deferral Deferral { get; }

			internal CoreWebView2NewWindowRequestedEventArgs CoreArguments { get; }

		}
	}

}
