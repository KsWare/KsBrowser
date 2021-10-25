using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class EventLoggerVM : CoreWebView2AdapterVM {

		/// <inheritdoc />
		public EventLoggerVM() {
			RegisterChildren(() => this);
		}

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);
			if (CoreWebView2 != null) {
				RegisterEvents();
			}
		}

		private void RegisterEvents() {
			CoreWebView2.NewWindowRequested += CoreWebView_NewWindowRequested;
			CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
			CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
			CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
			CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
			CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
			CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
			CoreWebView2.ContentLoading += CoreWebView2_ContentLoading;
			CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
		 // CoreWebView2.DownloadStarting => DownloadManager
		 // CoreWebView2.WebMessageReceived += (s, e) => Debug.WriteLine($"    WebMessageReceived");
		 // CoreWebView2.WebResourceRequested => handled by WebResourceFilter
			CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
		 // CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged; //TODO handle fullscreen request
		 // CoreWebView2.ClientCertificateRequested += CoreWebView2_ClientCertificateRequested;
			CoreWebView2.FrameNavigationStarting += CoreWebView2_FrameNavigationStarting;
			CoreWebView2.FrameNavigationCompleted += CoreWebView2_FrameNavigationCompleted;
			CoreWebView2.FrameCreated += CoreWebView2_FrameCreated;
			CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening;
			CoreWebView2.WindowCloseRequested += (s, e) => Debug.WriteLine($"    WindowCloseRequested");
		 //	CoreWebView2.ContextMenuRequested							//v>=1.0.1018
		 // CoreWebView2.IsDocumentPlayingAudioChanged => AudioManager	//v>=1.0.1018
		 // CoreWebView2.IsMutedChanged => AudioManager					//v>=1.0.1018
		}

		private void CoreWebView2_FrameNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e) {
			// e.NavigationId;
			// e.Cancel;
			// e.IsRedirected;
			// e.IsUserInitiated;
			// e.RequestHeaders;
			// e.Uri
			Debug.WriteLine($"{e.NavigationId,3} FrameNavigationStarting {(e.IsUserInitiated?"user":"")} {(e.IsRedirected?"→":"↬")} {e.Uri}");
		}

		private void CoreWebView2_FrameNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e) {
			// e.NavigationId;
			// e.IsSuccess;
			// e.WebErrorStatus
			Debug.WriteLine($"{e.NavigationId,3} FrameNavigationCompleted"); 
		}

		private void CoreWebView2_FrameCreated(object sender, CoreWebView2FrameCreatedEventArgs e) {
			// CoreWebView2Frame frame = e.Frame;
			Debug.WriteLine($"    FrameCreated");
		}

		private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e) {
			// e.NavigationId
			Debug.WriteLine($"{e.NavigationId,3} DOMContentLoaded");
		}

		private void CoreWebView2_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e) {
			// e.NavigationId
			// e.IsErrorPage
			Debug.WriteLine($"{e.NavigationId,3} ContentLoading {(e.IsErrorPage?"ErrorPage":"")}");
		}

		/// <summary> SourceChanged is raised when the Source property changes. </summary>
		private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e) {
			// SourceChanged is raised when navigating to a different site or fragment navigations.
			// It is not raised for other types of navigations such as page refreshes
			// or history.pushState with the same URL as the current page.
			// This event is raised before ContentLoading for navigation to a new document.

			// e.IsNewDocument

			Debug.WriteLine($"    SourceChanged {(e.IsNewDocument ? "NewDocument" : "")} {CoreWebView2.Source}");
		}

		private void CoreWebView2_DocumentTitleChanged(object sender, object e) {
			Debug.WriteLine($"    DocumentTitleChanged {CoreWebView2.DocumentTitle}");
		}

		private void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e) {
			// e.Request;
			// e.Response
			Debug.WriteLine($"    WebResourceResponseReceived {e.Request.Method} {e.Request.Uri} {e.Response.StatusCode} {TryGetHeader("Content-Type", e.Response.Headers)}");
		}

		private void CoreWebView_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e) {
			// e.IsUserInitiated;
			// e.Uri;
			CoreWebView2WindowFeatures windowFeatures = e.WindowFeatures;
			// e.NewWindow;
			// e.Handled;
			// e.GetDeferral()

			Debug.WriteLine($"    NewWindowRequested {(e.IsUserInitiated?"UserInitiated":"")} {e.Uri}");
		}

		private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e) {
			// e.NavigationId;
			// e.Uri;
			// e.Cancel;			{set;}
			// e.IsRedirected;
			// e.RequestHeaders

			Debug.WriteLine($"{e.NavigationId,3} NavigationStarting {(e.IsRedirected?"→":"↬")} {e.Uri}");

			// Tab.SourceInput = e.Uri;

			// if (!e.Uri.StartsWith("https://")) {
			// 	WebView2.CoreWebView2.ExecuteScriptAsync($"alert('{e.Uri} is not safe, try an https link')");
			// 	e.Cancel = true;
			// }
			// WebView2.CoreWebView2.ExecuteScriptAsync($"alert('NavigationStarting {e.Uri} {(e.IsRedirected?"redirected":"")}')");
		}

		private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e) {
			// e.NavigationId;
			// e.IsSuccess;
			// e.WebErrorStatus
			
			Debug.WriteLine($"{e.NavigationId,3} NavigationCompleted {e.WebErrorStatus} {(e.IsSuccess ? "successfully" : "with error")}");
			// WebView2.CoreWebView2.ExecuteScriptAsync($"alert('NavigationCompleted')");
		}

		private void CoreWebView2_ProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e) {
			// e.ExitCode;
			// e.FrameInfosForFailedProcess;
			// e.ProcessDescription;
			// CoreWebView2ProcessFailedKind x=e.ProcessFailedKind;
			// e.Reason
			Debug.WriteLine($"CoreWebView2.ProcessFailed {e.Reason} {e.ProcessFailedKind} \n{e.FrameInfosForFailedProcess}");
		}

		private void CoreWebView2_ScriptDialogOpening(object sender, CoreWebView2ScriptDialogOpeningEventArgs e) {
			// e.DefaultText {get;
			// e.Kind { get; }
			// e.Message;
			// e.ResultText
			// e.Uri
			// e.Accept();
			// e.GetDeferral()
			Debug.WriteLine($"ScriptDialogOpening\n{e.Message}\nKind:{e.Kind} {e.DefaultText} {e.ResultText}");
		}

		private void CoreWebView2_HistoryChanged(object sender, object e) {
			// WebView2.CoreWebView2.ExecuteScriptAsync($"alert('HistoryChanged')");
			Debug.WriteLine($"    HistoryChanged");
		}

		//helper
		private string TryGetHeader(string name, CoreWebView2HttpResponseHeaders responseHeaders) {
			if(!responseHeaders.Contains(name)) return null;
			return $"{name}: {responseHeaders.GetHeader(name)}";
		}
	}
}
