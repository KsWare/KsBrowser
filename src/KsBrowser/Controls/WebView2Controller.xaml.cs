using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KsWare.KsBrowser.Base;
using KsWare.Presentation;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace KsWare.KsBrowser.Controls {

	/// <summary>
	/// Interaction logic for WebView2Controller.xaml
	/// </summary>
	public partial class WebView2Controller : UserControl {

		private bool _isControlInVisualTree;

		public WebView2Controller() {
			InitializeComponent();
			Debug.WriteLine($"WebView2Controller InitializeComponent");

			DataContextChanged += OnDataContextChanged;

			OnWebView2Changed(this, new ValueChangedEventArgs<WebView2>(WebView2));
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine($"WebView2Controller DataContextChanged {e.NewValue?.GetType().Name??"null"}");
			if (e.OldValue is WebView2ControllerVM oldVM) {
				oldVM.NotifyViewChanged(this, new ValueChangedEventArgs<WebView2>(null, WebView2)); // detach WebView2 from old view model
			}

			if (e.NewValue is WebView2ControllerVM newVM) {
				newVM.NotifyViewChanged(this, new ValueChangedEventArgs<WebView2>(WebView2)); // attach WebView2 to new view model
			}
		}

		private void WebView2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e) {
			WebView2.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
		}

		void CoreWebView2_ProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e) {
			// SOURCE: https://github.com/MicrosoftEdge/WebView2Samples

			void ReinitIfSelectedByUser(CoreWebView2ProcessFailedKind kind) {
				string caption;
				string message;
				if (kind == CoreWebView2ProcessFailedKind.BrowserProcessExited) {
					caption = "Browser process exited";
					message = "WebView2 Runtime's browser process exited unexpectedly. Recreate WebView?";
				}
				else {
					caption = "Web page unresponsive";
					message = "WebView2 Runtime's render process stopped responding. Recreate WebView?";
				}

				var selection = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
				if (selection == MessageBoxResult.Yes) {
					// The control cannot be re-initialized so we setup a new instance to replace it.
					// Note the previous instance of the control is disposed of and removed from the
					// visual tree before attaching the new one.
					if (_isControlInVisualTree) RemoveControlFromVisualTree(WebView2);
					AttachControlToVisualTree(GetReplacementControl(false));
				}
			}

			void ReloadIfSelectedByUser(CoreWebView2ProcessFailedKind kind) {
				string caption;
				string message;
				if (kind == CoreWebView2ProcessFailedKind.RenderProcessExited) {
					caption = "Web page unresponsive";
					message = "WebView2 Runtime's render process exited unexpectedly. Reload page?";
				}
				else {
					caption = "App content frame unresponsive";
					message = "WebView2 Runtime's render process for app frame exited unexpectedly. Reload page?";
				}

				var selection = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
				if (selection == MessageBoxResult.Yes) {
					WebView2.Reload();
				}
			}

			bool IsAppContentUri(Uri source) {
				// Sample virtual host name for the app's content.
				// See CoreWebView2.SetVirtualHostNameToFolderMapping: https://docs.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.setvirtualhostnametofoldermapping
				return source.Host == "appassets.example";
			}

			switch (e.ProcessFailedKind) {
				case CoreWebView2ProcessFailedKind.BrowserProcessExited:
					// Once the WebView2 Runtime's browser process has crashed,
					// the control becomes virtually unusable as the process exit
					// moves the CoreWebView2 to its Closed state. Most calls will
					// become invalid as they require a backing browser process.
					// Remove the control from the visual tree so the framework does
					// not attempt to redraw it, which would call the invalid methods.
					RemoveControlFromVisualTree(WebView2);
					goto case CoreWebView2ProcessFailedKind.RenderProcessUnresponsive;
				case CoreWebView2ProcessFailedKind.RenderProcessUnresponsive:
					System.Threading.SynchronizationContext.Current.Post((_) => { ReinitIfSelectedByUser(e.ProcessFailedKind); }, null);
					break;
				case CoreWebView2ProcessFailedKind.RenderProcessExited:
					System.Threading.SynchronizationContext.Current.Post((_) => { ReloadIfSelectedByUser(e.ProcessFailedKind); }, null);
					break;
				case CoreWebView2ProcessFailedKind.FrameRenderProcessExited:
					// A frame-only renderer has exited unexpectedly. Check if reload is needed.
					// In this sample we only reload if the app's content has been impacted.
					foreach (CoreWebView2FrameInfo frameInfo in e.FrameInfosForFailedProcess) {
						if (IsAppContentUri(new System.Uri(frameInfo.Source))) {
							goto case CoreWebView2ProcessFailedKind.RenderProcessExited;
						}
					}

					break;
				default:
					// Show the process failure details. Apps can collect info for their logging purposes.
					StringBuilder messageBuilder = new StringBuilder();
					messageBuilder.AppendLine($"Process kind: {e.ProcessFailedKind}");
					messageBuilder.AppendLine($"Reason: {e.Reason}");
					messageBuilder.AppendLine($"Exit code: {e.ExitCode}");
					messageBuilder.AppendLine($"Process description: {e.ProcessDescription}");
					System.Threading.SynchronizationContext.Current.Post((_) => {
							MessageBox.Show(messageBuilder.ToString(), "Child process failed", MessageBoxButton.OK);
						}, null);
					break;
			}
		}

		WebView2 GetReplacementControl(bool useNewEnvironment) {
			WebView2 replacementControl = new WebView2();
			((System.ComponentModel.ISupportInitialize)(replacementControl)).BeginInit();
			// Setup properties and bindings.
			if (useNewEnvironment) {
				// Create a new CoreWebView2CreationProperties instance so the environment
				// is made anew.
				replacementControl.CreationProperties = new CoreWebView2CreationProperties();
				replacementControl.CreationProperties.BrowserExecutableFolder = WebView2.CreationProperties.BrowserExecutableFolder;
				replacementControl.CreationProperties.Language = WebView2.CreationProperties.Language;
				replacementControl.CreationProperties.UserDataFolder = WebView2.CreationProperties.UserDataFolder;
				// shouldAttachEnvironmentEventHandlers = true;
			}
			else {
				//TODO always null, because BrowserTabItemVM does not initialize CreationProperties
				//TODO WebView2 is null too because is set to null in RemoveControlFromVisualTree
				//replacementControl.CreationProperties = WebView2.CreationProperties;
			}
			
			// Binding urlBinding = new Binding() {
			// 	Source = replacementControl,
			// 	Path = new PropertyPath("Source"),
			// 	Mode = BindingMode.OneWay
			// };
			// url.SetBinding(TextBox.TextProperty, urlBinding);
			
			// AttachControlEventHandlers(replacementControl);
			// replacementControl.Source = webView.Source ?? new Uri("https://www.bing.com");
			((System.ComponentModel.ISupportInitialize)(replacementControl)).EndInit();

			return replacementControl;
		}


		private void RemoveControlFromVisualTree(WebView2 control) {
			Root.Content = null;
			_isControlInVisualTree = false;
			OnWebView2Changed(this, new ValueChangedEventArgs<WebView2>(control, null));
			control.Dispose();
			WebView2 = null;
		}

		private void AttachControlToVisualTree(WebView2 control) {
			var oldValue = (WebView2)Root.Content; // should always be null because we call RemoveControlFromVisualTree before
			Root.Content = control;
			WebView2 = control;
			_isControlInVisualTree = true;
			OnWebView2Changed(this, new ValueChangedEventArgs<WebView2>(oldValue, control));
			oldValue?.Dispose();
		}

		private void OnWebView2Changed(object sender, ValueChangedEventArgs<WebView2> e) {

			if (e.OldValue != null) {
				//because we call Dispose() we don't need to remove event handler
			}

			if (e.NewValue != null) {
				e.NewValue.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
			}

			if (DataContext is WebView2ControllerVM vm) {
				vm.NotifyViewChanged(this, e);
			}
		}

		void AttachControlEventHandlers(WebView2 control) {
			// control.NavigationStarting += WebView_NavigationStarting;
			// control.NavigationCompleted += WebView_NavigationCompleted;
			// control.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
			// control.KeyDown += WebView_KeyDown;
		}

	}

}
