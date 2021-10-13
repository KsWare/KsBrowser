﻿using JetBrains.Annotations;
using KsWare.Presentation.ViewModels;

namespace KsWare.KsBrowser {

	public class BrowserTabItemVM : ChromeTabItemVM {

		public BrowserTabItemVM(ITabHostVM tabHost = null, BrowserTabItemVM referrer = null) : base(tabHost) {
			ParentTab = referrer;
			RegisterChildren(() => this);

			WebContentPresenter.NewWindowRequested.add = WebContentPresenter_NewWindowRequested;
			WebContentPresenter.CloseRequested.add = WebContentPresenter_CloseRequested;
		}

		// public WebView2ControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }
		public CefSharpControllerVM WebContentPresenter { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		protected override void OnTabCreated(ITabCreationOptions options) {
			base.OnTabCreated(options);

			if (options is BrowserTabCreationOptions opt) {
				if (opt.NewWindowRequest != null) WebContentPresenter.Initialize(opt.NewWindowRequest);
				else if (opt.NavigationUri != null) WebContentPresenter.Initialize(opt.NavigationUri);
			}
		}

		private void WebContentPresenter_NewWindowRequested(object sender, NewWindowRequestedEventArgs e) {
			TabHost.CreateNewTab(new BrowserTabCreationOptions(e.InternalArguments, this));
		}

		private void WebContentPresenter_CloseRequested(object sender, CloseRequestedEventArgs e) {
			DoClose();
		}

		/// <inheritdoc/>
		protected override void DoClose() {
			TabHost.CloseTab(this);
		}

		/// <inheritdoc />
		protected override void OnClosing() {
			base.OnClosing();
		}
	}

}