using System;
using System.ComponentModel;
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

		public BrowserTabItemVM(IChromeTabHostVM tabHost, BrowserTabCreationOptions options) : base(tabHost)  {
			_options = options;
			ParentTab = options?.Referrer;
			RegisterChildren(() => this);

			WebContentPresenter.NewWindowRequested.add = WebContentPresenter_NewWindowRequested;
			WebContentPresenter.CloseRequested.add = WebContentPresenter_CloseRequested;
			
			FieldBindingOperations.SetBinding(Fields[nameof(Title)], new FieldBinding(WebContentPresenter.Fields[nameof(WebContentPresenter.DocumentTitle)], BindingMode.OneWay));
		}

		[Obsolete("",true)]
		public BrowserTabItemVM(IChromeTabHostVM tabHost = null, BrowserTabItemVM referrer = null) : base(tabHost) {
			ParentTab = referrer;
			RegisterChildren(() => this);

			WebContentPresenter.NewWindowRequested.add = WebContentPresenter_NewWindowRequested;
			WebContentPresenter.CloseRequested.add = WebContentPresenter_CloseRequested;
			
			FieldBindingOperations.SetBinding(Fields[nameof(Title)], new FieldBinding(WebContentPresenter.Fields[nameof(WebContentPresenter.DocumentTitle)], BindingMode.OneWay));
		}

		// public WebView2ControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }
		public CefSharpControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		protected override void OnActivated() {
			base.OnActivated();
			if(!_isContentInitialized) InitializeContent();;
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