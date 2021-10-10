using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser.Converter {

	public class InterruptReasonConverter : MarkupExtension, IValueConverter {
		private static readonly Dictionary<CoreWebView2DownloadInterruptReason, string> _dic = new Dictionary<CoreWebView2DownloadInterruptReason, string>() {
				{ CoreWebView2DownloadInterruptReason.None, "" }, // "No interrupt reason."
				{ CoreWebView2DownloadInterruptReason.FileFailed, "Generic file error." },
				{ CoreWebView2DownloadInterruptReason.FileAccessDenied, "Access denied due to security restrictions." },
				{ CoreWebView2DownloadInterruptReason.FileNoSpace, "Disk full. User should free some space or choose a different location to store the file." },
				{ CoreWebView2DownloadInterruptReason.FileNameTooLong, "Result file path with file name is too long." },
				{ CoreWebView2DownloadInterruptReason.FileTooLarge, "File is too large for file system." },
				{ CoreWebView2DownloadInterruptReason.FileMalicious, "Microsoft Defender Smartscreen detected a virus in the file." },
				{ CoreWebView2DownloadInterruptReason.FileTransientError, "File was in use, too many files opened, or out of memory." },
				{ CoreWebView2DownloadInterruptReason.FileBlockedByPolicy, "File blocked by local policy." },
				{ CoreWebView2DownloadInterruptReason.FileSecurityCheckFailed, "Security check failed unexpectedly. Microsoft Defender SmartScreen could not scan this file." },
				{ CoreWebView2DownloadInterruptReason.FileTooShort, "Seeking past the end of a file in opening a file, as part of resuming an  interrupted download. The file did not exist or was not as large as expected. Partially downloaded file was truncated or deleted, and download will be restarted automatically." },
				{ CoreWebView2DownloadInterruptReason.FileHashMismatch, "Partial file did not match the expected hash and was deleted. Download will be restarted automatically." },
				{ CoreWebView2DownloadInterruptReason.NetworkFailed, "Generic network error. User can retry the download manually." },
				{ CoreWebView2DownloadInterruptReason.NetworkTimeout, "Network operation timed out." },
				{ CoreWebView2DownloadInterruptReason.NetworkDisconnected, "Network connection lost. User can retry the download manually." },
				{ CoreWebView2DownloadInterruptReason.NetworkServerDown, "Server has gone down. User can retry the download manually." },
				{ CoreWebView2DownloadInterruptReason.NetworkInvalidRequest, "Network request invalid because original or redirected URI is invalid, has an unsupported scheme, or is disallowed by network policy." },
				{ CoreWebView2DownloadInterruptReason.ServerFailed, "Generic server error. User can retry the download manually." },
				{ CoreWebView2DownloadInterruptReason.ServerNoRange, "Server does not support range requests." },
				{ CoreWebView2DownloadInterruptReason.ServerBadContent, "Server does not have the requested data." },
				{ CoreWebView2DownloadInterruptReason.ServerUnauthorized, "Server did not authorize access to resource." },
				{ CoreWebView2DownloadInterruptReason.ServerCertificateProblem, "Server certificate problem." },
				{ CoreWebView2DownloadInterruptReason.ServerForbidden, "Unexpected server response. Responding server may not be intended server. User can retry the download manually." },
				{ CoreWebView2DownloadInterruptReason.ServerUnexpectedResponse, "Server sent fewer bytes than the Content-Length header. Content-Length header  may be invalid or connection may have closed. Download is treated as complete unless there are [strong validators](https://tools.ietf.org/html/rfc7232#section-2) present to interrupt the download." },
				{ CoreWebView2DownloadInterruptReason.ServerContentLengthMismatch, "Unexpected cross-origin redirect." },
				{ CoreWebView2DownloadInterruptReason.ServerCrossOriginRedirect, "User canceled the download." },
				{ CoreWebView2DownloadInterruptReason.UserCanceled, "User canceled the download." },
				{ CoreWebView2DownloadInterruptReason.UserShutdown, "User shut down the WebView. Resuming downloads that were interrupted during shutdown is not yet supported." },
				{ CoreWebView2DownloadInterruptReason.UserPaused, "User paused the download." },
				{ CoreWebView2DownloadInterruptReason.DownloadProcessCrashed, "WebView crashed." }
			};

		/// <inheritdoc />
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is CoreWebView2DownloadInterruptReason v) {
				if (!_dic.TryGetValue(v, out var outV)) return $"{value}";
				return outV;
			}
			else {
				throw new NotImplementedException();
			}
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}


	}
}
