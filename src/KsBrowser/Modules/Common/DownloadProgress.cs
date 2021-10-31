using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.KsBrowser.Modules.Common
{
	public class DownloadProgress
	{
		public int Id { get; set; }
		public long TotalBytes { get; set; }
		public long ReceivedBytes { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
	}
}
