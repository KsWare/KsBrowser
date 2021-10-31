using System;
using System.ComponentModel;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;
using BindingMode = System.Windows.Data.BindingMode;

namespace KsWare.KsBrowser {

	public class BrowserTabItemVM : ChromeTabItemVM {
		private readonly BrowserTabCreationOptions _options;
		private bool _isContentInitialized;

		public BrowserTabItemVM(BrowserTabCreationOptions options) {
			_options = options;
			// ParentTab = options?.Referrer;//< DEACTIVATED. managing the three for dragging between windows is currently not supported.
			RegisterChildren(() => this);

			WebContentPresenter.NewWindowRequested.add = WebContentPresenter_NewWindowRequested;
			WebContentPresenter.CloseRequested.add = WebContentPresenter_CloseRequested;
			
			FieldBindingOperations.SetBinding(Fields[nameof(Title)], new FieldBinding(WebContentPresenter.Fields[nameof(WebContentPresenter.DocumentTitle)], BindingMode.OneWay));
		}

		// public WebView2ControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }
		// public WebView2WebToolsVM WebTools { get; [UsedImplicitly] private set; }
		public CefSharpControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }
		public CefSharpWebToolsVM WebTools { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		protected override void OnActivated() {
			base.OnActivated();
			if (!_isContentInitialized) {
				_isContentInitialized = true;
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, InitializeContent);
			}
		}

		private void InitializeContent() {
			var options = _options;
			CommonTools.WaitForRender();
			if (options.NewWindowRequest != null) WebContentPresenter.Initialize(options.NewWindowRequest);
			else if (options.NavigationUri != null) WebContentPresenter.Initialize(options.NavigationUri);
		}
		
		private void WebContentPresenter_NewWindowRequested(object sender, NewWindowRequestedEventArgs e) {
			TabHost.AddNewTabItem(new BrowserTabCreationOptions(e.InternalArguments, this));
		}

		private void WebContentPresenter_CloseRequested(object sender, CloseRequestedEventArgs e) {
			DoClose();
		}

		/// <inheritdoc/>
		protected override void DoClose() {
			TabHost.CloseTabItem(this);
		}

		/// <inheritdoc />
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
		}
	}

}