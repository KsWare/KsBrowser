using System;
using CefSharp.Wpf;
using JetBrains.Annotations;
using KsWare.KsBrowser.Base;
using KsWare.KsBrowser.Logging;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser {

	public class CefSharpWebToolsVM : ObjectVM, IViewControllerVM<ChromiumWebBrowser> {
		private readonly Log Log;

		/// <inheritdoc />
		public CefSharpWebToolsVM() {
			Log = new Log(this);
			Log.Method($".ctor");
			RegisterChildren(() => this);
			Fields[nameof(IsVisible)].ValueChangedEvent.add = (s, e) => { if (IsVisible) ChromiumWebBrowser.Address = "localhost:8080"; };
		}

		public ChromiumWebBrowser ChromiumWebBrowser { get => Fields.GetValue<ChromiumWebBrowser>(); set => Fields.SetValue(value); }

		public ActionVM ToggleVisibilityAction { get; [UsedImplicitly] private set; }

		public bool IsVisible { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		private void DoToggleVisibility() {
			IsVisible = !IsVisible;
		}	

		/// <inheritdoc />
		void IViewControllerVM<ChromiumWebBrowser>.NotifyViewChanged(object sender, ValueChangedEventArgs<ChromiumWebBrowser> e) {
			Log.Method($"");
			if (ChromiumWebBrowser != null && e.NewValue != ChromiumWebBrowser) {
				throw new NotSupportedException();
				// clean old
				ChromiumWebBrowser = null;
			}

			if (ChromiumWebBrowser == null && e.NewValue != null) {
				Log.Message("initialize ChromiumWebBrowser");
				ChromiumWebBrowser = e.NewValue;
			}
		}
	}

}