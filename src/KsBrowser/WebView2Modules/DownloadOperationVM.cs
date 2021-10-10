using System;
using System.IO;
using KsWare.KsBrowser.Extensions;
using KsWare.Presentation.ViewModelFramework;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.WebView2Modules {

	public class DownloadOperationVM : ObjectVM{

		private readonly CoreWebView2DownloadOperation _downloadOperation;

		/// <inheritdoc />
		public DownloadOperationVM() {
			if (IsInDesignMode) {
				StartTime = DateTime.Now;
				File = @"C:\Downloads\test.dat";
				FileName = "test.dat";
				State = CoreWebView2DownloadState.InProgress;
				EstimatedEndTime = DateTime.Now.AddMinutes(10);
				EstimatedTimeLeft=TimeSpan.FromMinutes(10);
				EstimatedTotalTime=TimeSpan.FromMinutes(10);
				BytesReceived = 34657;
				PercentComplete = 50;
				IsFinished = false;
				InterruptReason = 0;
			}
		}

		public DownloadOperationVM(CoreWebView2DownloadStartingEventArgs e) {
			_downloadOperation = e.DownloadOperation;
			StartTime = DateTime.Now;
			File = e.ResultFilePath;
			FileName = Path.GetFileName(e.ResultFilePath);

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

		public DateTime StartTime { get => Fields.GetValue<DateTime>(); set => Fields.SetValue(value); }

		public string File { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public string FileName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public CoreWebView2DownloadState State { get => Fields.GetValue<CoreWebView2DownloadState>(); set => Fields.SetValue(value); }
		
		public DateTime EstimatedEndTime { get => Fields.GetValue<DateTime>(); set => Fields.SetValue(value); }

		public TimeSpan EstimatedTotalTime { get => Fields.GetValue<TimeSpan>(); set => Fields.SetValue(value); }
		
		public long BytesReceived { get => Fields.GetValue<long>(); set => Fields.SetValue(value); }

		public double PercentComplete { get => Fields.GetValue<double>(); set => Fields.SetValue(value); }

		public TimeSpan EstimatedTimeLeft { get => Fields.GetValue<TimeSpan>(); set => Fields.SetValue(value); }

		public bool IsFinished { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public CoreWebView2DownloadInterruptReason InterruptReason { get => Fields.GetValue<CoreWebView2DownloadInterruptReason>(); set => Fields.SetValue(value); }

		private void DownloadOperationOnStateChanged(object sender = null, object e = null) {
			State = _downloadOperation.State;
			IsFinished = _downloadOperation.State == CoreWebView2DownloadState.Completed || _downloadOperation.State == CoreWebView2DownloadState.Interrupted;
			InterruptReason = _downloadOperation.InterruptReason;
			UpdateEstimatedEndTime();
		}

		private void DownloadOperationOnEstimatedEndTimeChanged(object sender = null, object e = null) {
			EstimatedEndTime = _downloadOperation.EstimatedEndTime.FloorSeconds();
			EstimatedTimeLeft = _downloadOperation.EstimatedEndTime.Subtract(DateTime.Now).FloorSeconds();
			EstimatedTotalTime = EstimatedEndTime.Subtract(StartTime).FloorSeconds();
		}

		private void UpdateEstimatedEndTime() {
			switch (State) {
				case CoreWebView2DownloadState.Interrupted:
					// EstimatedEndTime = infinite
					// EstimatedTimeLeft = infinite
					break;
				case CoreWebView2DownloadState.Completed:
					if(EstimatedEndTime>DateTime.Now) EstimatedEndTime=DateTime.Now;
					EstimatedTimeLeft = TimeSpan.Zero;
					break;
			}
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

		public void NotifyViewerCrash() {
			if (State == CoreWebView2DownloadState.InProgress) {
				State = CoreWebView2DownloadState.Interrupted;
				IsFinished = true;
				InterruptReason = CoreWebView2DownloadInterruptReason.DownloadProcessCrashed;
				UpdateEstimatedEndTime();
			}
		}

	}

}