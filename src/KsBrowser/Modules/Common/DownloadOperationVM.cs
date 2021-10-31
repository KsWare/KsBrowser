using System;
using System.IO;
using CefSharp;
using JetBrains.Annotations;
using KsWare.KsBrowser.Extensions;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.Modules.Common {

	public class DownloadOperationVM : ObjectVM {
		
		/// <inheritdoc />
		public DownloadOperationVM() {
			RegisterChildren(()=>this);
			IsVisible = true;
			Fields[nameof(IsFinished)].ValueChangedEvent.add = (s, e) => { if (e.NewValue is bool b and true) OnFinished(); };
			Fields[nameof(SizeInByte)].ValueChangedEvent.add = (s, e) => { CalculateDisplaySize((long)e.NewValue, nameof(SizeValue), nameof(SizeUnit));};
			Fields[nameof(BytesReceived)].ValueChangedEvent.add = (s, e) => { CalculateDisplaySize((long)e.NewValue, nameof(ReceivedValue), nameof(ReceivedUnit));};
			Fields[nameof(File)].ValueChangedEvent.add = (s, e) => { FileName = !string.IsNullOrEmpty((string)e.NewValue) ? Path.GetFileName(File) : ""; };

			if (IsInDesignMode) {
				StartTime = DateTime.Now;
				File = @"C:\Downloads\test.dat";
				State = DownloadState.InProgress;
				EstimatedEndTime = DateTime.Now.AddMinutes(10);
				EstimatedTimeLeft=TimeSpan.FromMinutes(10);
				EstimatedTotalTime=TimeSpan.FromMinutes(10);
				BytesReceived = 34657;
				SizeInByte = 34657 * 2;
				PercentComplete = 50;
				IsFinished = false;
				InterruptReason = "";
			}
		}

		public ActionVM CancelAction { get; [UsedImplicitly] private set; }

		public ActionVM PauseAction { get; [UsedImplicitly] private set; }

		public ActionVM ResumeAction { get; [UsedImplicitly] private set; }

		public ActionVM HideAction { get; [UsedImplicitly] private set; }

		public int Id { get; protected set; }

		public DateTime StartTime { get => Fields.GetValue<DateTime>(); protected set => Fields.SetValue(value); }

		public string File { get => Fields.GetValue<string>(); protected set => Fields.SetValue(value); }

		public string FileName { get => Fields.GetValue<string>(); private set => Fields.SetValue(value); }

		public string Url { get => Fields.GetValue<string>(); protected set => Fields.SetValue(value); }

		public DownloadState State { get => Fields.GetValue<DownloadState>(); protected set => Fields.SetValue(value); }
		
		public DateTime EstimatedEndTime { get => Fields.GetValue<DateTime>(); protected set => Fields.SetValue(value); }

		public TimeSpan EstimatedTotalTime { get => Fields.GetValue<TimeSpan>(); protected set => Fields.SetValue(value); }
		
		public long BytesReceived { get => Fields.GetValue<long>(); protected set => Fields.SetValue(value); }
		
		public double ReceivedValue { get => Fields.GetValue<double>(); private set => Fields.SetValue(value); }

		public string ReceivedUnit { get => Fields.GetValue<string>(); private set => Fields.SetValue(value); }

		public double PercentComplete { get => Fields.GetValue<double>(); protected set => Fields.SetValue(value); }

		public TimeSpan EstimatedTimeLeft { get => Fields.GetValue<TimeSpan>(); protected set => Fields.SetValue(value); }

		public bool IsFinished { get => Fields.GetValue<bool>(); protected set => Fields.SetValue(value); }

		public long SizeInByte { get => Fields.GetValue<long>(); protected set => Fields.SetValue(value); }	

		public double SizeValue { get => Fields.GetValue<double>(); private set => Fields.SetValue(value); }

		public string SizeUnit { get => Fields.GetValue<string>(); private set => Fields.SetValue(value); }

		public string InterruptReason { get => Fields.GetValue<string>(); protected set => Fields.SetValue(value); }

		public bool IsVisible { get => Fields.GetValue<bool>(); protected set => Fields.SetValue(value); }

		protected virtual void OnFinished() {

		}

		protected void UpdateEstimatedEndTime() {
			switch (State) {
				case DownloadState.Interrupted:
					// EstimatedEndTime = infinite
					// EstimatedTimeLeft = infinite
					break;
				case DownloadState.Completed:
					if(EstimatedEndTime>DateTime.Now) EstimatedEndTime=DateTime.Now;
					EstimatedTimeLeft = TimeSpan.Zero;
					break;
			}
		}
		
		public void NotifyViewerCrash() {
			if (State == DownloadState.InProgress) {
				State = DownloadState.Interrupted;
				IsFinished = true;
				InterruptReason = $"{CoreWebView2DownloadInterruptReason.DownloadProcessCrashed}"; //TODO CoreWebView2DownloadInterruptReason
				UpdateEstimatedEndTime();
			}
		}

		protected virtual void DoCancel() {

		}

		protected virtual void DoPause() {

		}

		protected virtual void DoResume() {

		}

		protected virtual void DoHide() {
			IsVisible = false;
		}

		private void CalculateDisplaySize(long bytes, string valueProperty, string unitProperty) {
			var sizeValue = (double)bytes; 
			var sizeUnit = "B";
			var u = new[] { "kB", "MB", "GB", "TB" };
			for (var i = 0; sizeValue>1500 && i<u.Length; i++) {
				sizeValue /= 1024.0;
				sizeUnit = u[i];
			}
			Fields[valueProperty].Value = sizeValue;
			Fields[unitProperty].Value = sizeUnit;
		}
		
	}

	public class DownloadOperationWebView2VM : DownloadOperationVM {

		private readonly CoreWebView2DownloadOperation _downloadOperation;

		public DownloadOperationWebView2VM(CoreWebView2DownloadStartingEventArgs e) {
			RegisterChildren(()=>this);
			StartTime = DateTime.Now;
			File = e.ResultFilePath;

			_downloadOperation = e.DownloadOperation;
			_downloadOperation.BytesReceivedChanged += DownloadOperationOnBytesReceivedChanged;
			_downloadOperation.EstimatedEndTimeChanged += DownloadOperationOnEstimatedEndTimeChanged;
			_downloadOperation.StateChanged += DownloadOperationOnStateChanged;
			DownloadOperationOnStateChanged(); //			_downloadOperation.State;
			DownloadOperationOnEstimatedEndTimeChanged();// _downloadOperation.EstimatedEndTime;
			DownloadOperationOnBytesReceivedChanged(); //	_downloadOperation.BytesReceived;

			// _downloadOperation.CanResume;
			// _downloadOperation.ContentDisposition;
			// _downloadOperation.InterruptReason;
			// _downloadOperation.MimeType;
			// _downloadOperation.ResultFilePath;
			// _downloadOperation.TotalBytesToReceive;
			// _downloadOperation.Uri;
		}


		private void DownloadOperationOnStateChanged(object sender = null, object e = null) {
			State = (DownloadState)_downloadOperation.State+1;
			IsFinished = _downloadOperation.State == CoreWebView2DownloadState.Completed || _downloadOperation.State == CoreWebView2DownloadState.Interrupted;
			InterruptReason = $"{_downloadOperation.InterruptReason}"; //TODO
			UpdateEstimatedEndTime();
		}

		private void DownloadOperationOnEstimatedEndTimeChanged(object sender = null, object e = null) {
			EstimatedEndTime = _downloadOperation.EstimatedEndTime.FloorSeconds();
			EstimatedTimeLeft = _downloadOperation.EstimatedEndTime.Subtract(DateTime.Now).FloorSeconds();
			EstimatedTotalTime = EstimatedEndTime.Subtract(StartTime).FloorSeconds();
		}

		private void DownloadOperationOnBytesReceivedChanged(object sender = null, object e = null) {
			BytesReceived = _downloadOperation.BytesReceived;
			if (_downloadOperation.TotalBytesToReceive.HasValue) {
				PercentComplete = ((double)_downloadOperation.BytesReceived / (double)_downloadOperation.TotalBytesToReceive.Value) * 100.0;
			}
			else {
				PercentComplete = double.NaN;
			}
		}
	}

	public class DownloadOperationCefSharpVM : DownloadOperationVM, DownloadOperationCefSharpVM.INotification {
		private readonly IDownloadItemCallback _callback;

		public DownloadOperationCefSharpVM(DownloadItem downloadItem, IDownloadItemCallback callback) {
			_callback = callback;
			RegisterChildren(()=>this);
			Id = downloadItem.Id;
			Url = downloadItem.Url;
			// null downloadItem.FullPath;
			// downloadItem.ContentDisposition

			StartTime = downloadItem.StartTime ?? DateTime.Now;
			SizeInByte = downloadItem.TotalBytes;

			ResumeAction.SetCanExecute("NotPaused",false);
		}

		/// <inheritdoc />
		void DownloadOperationCefSharpVM.INotification.OnViewerCrash() {
			
		}

		void DownloadOperationCefSharpVM.INotification.OnBeforeDownload(DownloadItem downloadItem) {
			
		}

		/// <inheritdoc />
		void DownloadOperationCefSharpVM.INotification.OnUpdate(DownloadItem downloadItem) {
			if (!string.IsNullOrEmpty(downloadItem.FullPath))
				File = downloadItem.FullPath;

			if (false) ;
			else if (downloadItem.IsCancelled) State = DownloadState.Interrupted;
			else if (downloadItem.IsComplete) State = DownloadState.Completed;
			else if (downloadItem.IsInProgress) State = DownloadState.InProgress;
			IsFinished = State == DownloadState.Completed || State == DownloadState.Interrupted;
			// InterruptReason
			if (!IsFinished) {
				if (downloadItem.TotalBytes > 0 && downloadItem.ReceivedBytes > 0) {
					var t = DateTime.Now.Subtract(StartTime).TotalSeconds;
					var p = (double)downloadItem.ReceivedBytes / downloadItem.TotalBytes * 100;
					EstimatedEndTime = StartTime.Add(TimeSpan.FromSeconds(t / p * 100)).FloorSeconds();
					EstimatedTimeLeft = EstimatedEndTime.Subtract(DateTime.Now).FloorSeconds();
					EstimatedTotalTime = EstimatedEndTime.Subtract(StartTime).FloorSeconds();
				}
			}
			else {
				UpdateEstimatedEndTime();
			}

			BytesReceived = downloadItem.ReceivedBytes;

			if (downloadItem.TotalBytes>0) {
				PercentComplete = ((double)downloadItem.ReceivedBytes / (double)downloadItem.TotalBytes) * 100.0;
			}
			else {
				PercentComplete = double.NaN;
			}
		}

		protected override void OnFinished() {
			CancelAction.SetCanExecute("AlreadyFinished",false);
			ResumeAction.SetCanExecute("AlreadyFinished",false);
			PauseAction.SetCanExecute("AlreadyFinished",false);
		}

		/// <inheritdoc />
		protected override void DoCancel() {
			InterruptReason = "Canceled by user.";
			_callback.Cancel();
			CancelAction.SetCanExecute("AlreadyCanceled", false);
		}

		/// <inheritdoc />
		protected override void DoPause() {
			_callback.Pause();
			ResumeAction.SetCanExecute("NotPaused", true);
			PauseAction.SetCanExecute("AlreadyPaused", false);
		}

		/// <inheritdoc />
		protected override void DoResume() {
			_callback.Resume();
			ResumeAction.SetCanExecute("NotPaused", false);
			PauseAction.SetCanExecute("AlreadyPaused", true);
		}

		internal interface INotification {
			void OnViewerCrash();
			void OnBeforeDownload(DownloadItem downloadItem);
			void OnUpdate(DownloadItem downloadItem);
		}
	}

	/// <summary>
	/// The state of the <see cref="DownloadOperationVM" />.
	/// </summary>
	public enum DownloadState {
		None = 0,
		/// <summary>The download is in progress.</summary>
		InProgress,
		/// <summary>
		/// The connection with the file host was broken. The reason why a download was interrupted can accessed from <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2DownloadOperation.InterruptReason" />. See <see cref="T:Microsoft.Web.WebView2.Core.CoreWebView2DownloadInterruptReason" /> for descriptions of the different kinds of interrupt reasons. Host can check whether an interrupted download can be resumed with <see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2DownloadOperation.CanResume" />. Once resumed, the download state is in progress.
		/// </summary>
		Interrupted,
		/// <summary>The download completed successfully.</summary>
		Completed,
	}

}