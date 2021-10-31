using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.KsBrowser.Modules.Common {

	public class DownloadInfo {
		public string Url { get; set; }
		public string File { get; set; }
		public string MimeType { get; set; }
		public string ContentDisposition { get; set; }
		public long TotalBytes { get; set; }
	}

}
