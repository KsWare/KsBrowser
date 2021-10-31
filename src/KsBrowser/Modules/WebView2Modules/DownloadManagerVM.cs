using System.IO;
using System.Linq;
using JetBrains.Annotations;
using KsWare.KsBrowser.Controls;
using KsWare.KsBrowser.Modules;
using KsWare.KsBrowser.Modules.Common;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;

namespace KsWare.KsBrowser.WebView2Modules {

	public class DownloadManagerVM : CoreWebView2AdapterVM, IDownloadManagerVM {

		/// <inheritdoc />
		public DownloadManagerVM() {
			RegisterChildren(() => this);
		}

		public ActionVM ShowDownloadsAction { get; private set; }

		void DoShowDownloads() {
			new DownloadsWindow{DataContext = this}.Show(); //TODO use DownloadsWindowVM
		}

		/// <inheritdoc/>
		public ListVM<DownloadOperationVM> DownloadOperations { get; [UsedImplicitly] private set; }
		
		/// <inheritdoc/>
		[Hierarchy(HierarchyType.Reference)]
		public DownloadOperationVM SelectedDownloadOperation { get => Fields.GetValue<DownloadOperationVM>(); set => Fields.SetValue(value); }
		
		/// <inheritdoc/>
		public bool ShowFileDialog { get; set; } = false;
		
		/// <inheritdoc/>
		public string DownloadFolder { get; set; }

		/// <inheritdoc />
		public override void Init(WebView2ControllerVM tab, CoreWebView2 coreWebView2) {
			base.Init(tab, coreWebView2);

			foreach (var operation in DownloadOperations.Where(op=>!op.IsFinished)) {
				operation.NotifyViewerCrash();
			}
			if (CoreWebView2 != null) {
				CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
			}
		}

		private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e) {
			// e.Cancel				bool get/set
			// e.Handled;			bool get/set
			// e.DownloadOperation;	Microsoft.Web.WebView2.Core.CoreWebView2DownloadOperation
			// e.ResultFilePath;	string get/set
			// e.GetDeferral()

			// BytesReceived	1645976	long
			// CanResume	false	bool
			// ContentDisposition	""	string
			// EstimatedEndTime	{01.10.2021 21:26:05}	System.DateTime
			// InterruptReason	None	Microsoft.Web.WebView2.Core.CoreWebView2DownloadInterruptReason
			// MimeType	"application/octet-stream"	string
			// ResultFilePath	"C:\\Users\\Kay\\Downloads\\vs_community__a1f04914.774b.4eb9.b627.b882eeffa00f.exe"	string
			// State	InProgress	Microsoft.Web.WebView2.Core.CoreWebView2DownloadState
			// TotalBytesToReceive	1645976	ulong?
			// Uri	"blob:https://visualstudio.microsoft.com/47039fd1-2c29-4db7-ba3e-46c1248d5674"	string
			CoreWebView2Deferral deferral = null;

			if (ShowFileDialog) {
				//TODO MUST NOT BLOCK! convert ty async operation
				var dlg = new SaveFileDialog {
					Filter = "All Files (*.*)|*.*",
					FilterIndex = 1,
					Title = "Save file to ..",
					FileName = Path.GetFileName(e.ResultFilePath),
					CheckFileExists = true,
					CheckPathExists = true
				};
				deferral = e.GetDeferral();
				if (dlg.ShowDialog() != true) {
					e.Cancel = true;
					e.Handled = true;
					deferral.Complete();
					return;
				}
				e.ResultFilePath = dlg.FileName;
			} else if (DownloadFolder != null) {
				var n = Path.Combine(DownloadFolder, Path.GetFileName(e.ResultFilePath));
				//TODO check path
				e.ResultFilePath = n;
			}

			var downLoadOperation = new DownloadOperationWebView2VM(e);
			DownloadOperations.Add(downLoadOperation);

			e.Handled = true;
			if (deferral != null) deferral.Complete();
		}

		
	}

}