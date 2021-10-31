using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using KsWare.KsBrowser.Base;
using KsWare.Presentation;

namespace KsWare.KsBrowser.Controls {

	/// <summary>
	/// Interaction logic for CefSharpWebTools.xaml
	/// </summary>
	public partial class CefSharpWebToolsView : UserControl {

		public CefSharpWebToolsView() {
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] CefSharpWebTools DataContextChanged {e.NewValue?.GetType().Name ?? "null"}");
			if (e.OldValue is IViewControllerVM<ChromiumWebBrowser> oldVM) {
				oldVM.NotifyViewChanged(this,
					new ValueChangedEventArgs<ChromiumWebBrowser>(null,
						BrowserControl)); // detach BrowserControl from old view model
			}

			if (e.NewValue is IViewControllerVM<ChromiumWebBrowser> newVM) {
				newVM.NotifyViewChanged(this,
					new ValueChangedEventArgs<ChromiumWebBrowser>(BrowserControl)); // attach BrowserControl to new view model
			}
		}
	}

}
