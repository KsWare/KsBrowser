using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using KsWare.Presentation;

namespace KsWare.KsBrowser.Extensions {

	public static class DispatcherExtensions {

		private static void SafeAction(Action action, string caller, string file, int lineNumber) {
			try {
				action();
			}
			catch (Exception ex) {
				Debug.WriteLine($"{caller} in {file}:{lineNumber}");
				Debug.WriteLine(ex.ToString());
				MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private static void SafeAction<T>(Func<T> action, string caller, string file, int lineNumber) {
			try {
				action();
			}
			catch (Exception ex) {
				Debug.WriteLine($"{caller} in {file}:{lineNumber}");
				Debug.WriteLine(ex.ToString());
				MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		public static void TryBeginInvoke(this IDispatcher d, Action action, 
			[CallerMemberName] string caller = null, [CallerFilePath] string path = null, [CallerLineNumber] int lineNumber = -1) {
			d.BeginInvoke(DispatcherPriority.Normal, () => SafeAction(action, caller, path, lineNumber));
		}

		// evaluate
		public static void TryTaskRun(this IDispatcher d, Action action, 
			[CallerMemberName] string caller = null, [CallerFilePath] string path = null, [CallerLineNumber] int lineNumber = -1) {
			Task.Run(() => SafeAction(action, caller, path, lineNumber));
		}
		// evaluate
		public static void TryTaskRun<T>(this IDispatcher d, Func<T> action, 
			[CallerMemberName] string caller = null, [CallerFilePath] string path = null, [CallerLineNumber] int lineNumber = -1) {
			Task.Run(() => SafeAction(action, caller, path, lineNumber));
		}

		public static Task<T> RunAsync<T>(this IDispatcher d, Func<T> asyncFunction) {
			var dispatcherOperation = d.ThreadDispatcher.InvokeAsync(asyncFunction);
			return dispatcherOperation.Task;
		}

		public static Task RunAsync(this IDispatcher d, Func<Task> asyncAction) {
			var dispatcherOperation = d.ThreadDispatcher.InvokeAsync(asyncAction);
			return dispatcherOperation.Task;
		}

		// Run async method on UI thread:
		// await Application.Current.Dispatcher.BeginInvoke(() => WebView2.EnsureCoreWebView2Async(environment)).Task;
		
	}
}
