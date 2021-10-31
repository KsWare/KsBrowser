using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CefSharp;
using CefSharp.Wpf;
using JetBrains.Annotations;
using KsWare.KsBrowser.Controls;
using KsWare.KsBrowser.Logging;
using KsWare.KsBrowser.Modules.Common;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Modules.CefModules {

	public class DownloadManagerVM : CefModuleBaseVM, IDownloadManagerVM, IDownloadHandler {

		private readonly Log Log;

		/// <inheritdoc />
		public DownloadManagerVM() {
			Log = new Log(this);
			Log.Method(".ctor");
			RegisterChildren(() => this);
		}

		/// <inheritdoc />
		public override void Init(CefSharpControllerVM presenter, ChromiumWebBrowser browserControl) {
			Log.Method($"");
			base.Init(presenter, browserControl);
			if (presenter == null || browserControl.DownloadHandler != null) return;
			ChromiumWebBrowser.DownloadHandler = this;

			foreach (var operation in DownloadOperations.Where(op => !op.IsFinished)) {
				operation.NotifyViewerCrash();
			}
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
		void IDownloadHandler.OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback) {
			Log.Method($"{downloadItem.TotalBytes} {downloadItem.Url}");

			if (downloadItem.IsValid) {
				Debug.WriteLine("== File information ========================");
				Debug.WriteLine(" File URL: {0}", downloadItem.Url);
				Debug.WriteLine(" Suggested FileName: {0}", downloadItem.SuggestedFileName);
				Debug.WriteLine(" MimeType: {0}", downloadItem.MimeType);
				Debug.WriteLine(" Content Disposition: {0}", downloadItem.ContentDisposition);
				Debug.WriteLine(" Total Size: {0}", downloadItem.TotalBytes);
				Debug.WriteLine("============================================");
			}

			if (callback.IsDisposed) return;
			using (callback) {
				var downloadOperation = DownloadOperations.FirstOrDefault(op => op.Id==downloadItem.Id);
				((DownloadOperationCefSharpVM.INotification)downloadOperation).OnBeforeDownload(downloadItem);
				// Define the Downloads Directory Path
				// You can use a different one, in this example we will hard-code it
				var downloadFolder = !string.IsNullOrEmpty(DownloadFolder) ? DownloadFolder : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
				var downloadPath = Path.Combine(downloadFolder, downloadItem.SuggestedFileName);
				if(ShowFileDialog)
					callback.Continue(null, showDialog: true);
				else
					callback.Continue(downloadPath, showDialog: false);
			}
		}

		/// <inheritdoc />
		void IDownloadHandler.OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser,
			DownloadItem downloadItem, IDownloadItemCallback callback) {
			Log.Method($"[{downloadItem.Id}] {(downloadItem.IsInProgress?"P":"")}{(downloadItem.IsCancelled?"C":"")}{(downloadItem.IsComplete?"F":"")} {downloadItem.PercentComplete}% {downloadItem.Url}");
			
			if (!downloadItem.IsValid) return;
			var downloadOperation = DownloadOperations.FirstOrDefault(op => op.Id==downloadItem.Id);
			if (downloadOperation == null) {
				downloadOperation = new DownloadOperationCefSharpVM(downloadItem, callback);
				DownloadOperations.Add(downloadOperation);
			}
			else {
				((DownloadOperationCefSharpVM.INotification)downloadOperation).OnUpdate(downloadItem);
			}
		}
	}

}
