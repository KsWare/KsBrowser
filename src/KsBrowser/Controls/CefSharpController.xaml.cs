using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CefSharp.Wpf;
using KsWare.KsBrowser.Base;
using KsWare.Presentation;

namespace KsWare.KsBrowser.Controls {

	/// <summary>
	/// Interaction logic for CefSharpController.xaml
	/// </summary>
	public partial class CefSharpController : UserControl {
		
		public CefSharpController() {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] new CefSharpController");
			InitializeComponent();
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] CefSharpController InitializeComponent");

			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] CefSharpController DataContextChanged {e.NewValue?.GetType().Name??"null"}");
			if (e.OldValue is IViewControllerVM<ChromiumWebBrowser> oldVM) {
				oldVM.NotifyViewChanged(this, new ValueChangedEventArgs<ChromiumWebBrowser>(null, BrowserControl)); // detach WebView2 from old view model
			}

			if (e.NewValue is IViewControllerVM<ChromiumWebBrowser> newVM) {
				newVM.NotifyViewChanged(this, new ValueChangedEventArgs<ChromiumWebBrowser>(BrowserControl)); // attach WebView2 to new view model
			}
		}

	}

}
