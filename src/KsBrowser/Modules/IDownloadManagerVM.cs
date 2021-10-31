using KsWare.KsBrowser.Modules.Common;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Modules {

	public interface IDownloadManagerVM {
		ListVM<DownloadOperationVM> DownloadOperations { get; }
		DownloadOperationVM SelectedDownloadOperation { get; }
		bool ShowFileDialog { get; set; }
		string DownloadFolder { get; set; }
		ActionVM ShowDownloadsAction { get; }
	}

}