using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CefSharp.Handler;
using CefSharp.Wpf;
using KsWare.KsBrowser.Base;
using KsWare.KsBrowser.CefSpecific;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework.Behaviors;

namespace KsWare.KsBrowser {

	public class CefSharpControllerVM : WebContentPresenterVM, IViewControllerVM<ChromiumWebBrowser>  {

		/// <inheritdoc />
		public CefSharpControllerVM() {
			Debug.WriteLine($"new CefSharpControllerVM");
			RegisterChildren(() => this);

			// NavigateBackCommand = initialize later
			// NavigateForwardCommand = initialize later
		}

		public ChromiumWebBrowser ChromiumWebBrowser { get => Fields.GetValue<ChromiumWebBrowser>(); set => Fields.SetValue(value); }

		/// <inheritdoc />
		public void NotifyViewChanged(object sender, ValueChangedEventArgs<ChromiumWebBrowser> e) {
			if (e.OldValue != null) {

			}

			if (e.NewValue != null) {
				ChromiumWebBrowser = e.NewValue;

				NavigateBackCommand = ChromiumWebBrowser.BackCommand; OnPropertyChanged(nameof(NavigateBackCommand));
				NavigateForwardCommand = ChromiumWebBrowser.ForwardCommand; OnPropertyChanged(nameof(NavigateForwardCommand));
				RefreshCommand = ChromiumWebBrowser.ReloadCommand; OnPropertyChanged(nameof(RefreshCommand));

				ChromiumWebBrowser.AddressChanged += ChromiumWebBrowser_AddressChanged;
				ChromiumWebBrowser.TitleChanged += ChromiumWebBrowser_TitleChanged;
				ChromiumWebBrowser.IsBrowserInitializedChanged += ChromiumWebBrowser_IsBrowserInitializedChanged;

				var lifeSpanHandler = new MyLifeSpanHandler();
				lifeSpanHandler.AfterCreated += LifeSpanHandler_AfterCreated;
				lifeSpanHandler.BeforeClose += LifeSpanHandler_BeforeClose;
				lifeSpanHandler.NewWindowRequested += LifeSpanHandler_NewWindowRequested;
				ChromiumWebBrowser.LifeSpanHandler = lifeSpanHandler;
			}
		}

		private void LifeSpanHandler_NewWindowRequested(object sender, CefSpecific.NewWindowRequestedEventArgs e) {
			Dispatcher.Invoke(DispatcherPriority.Normal, () =>
				EventManager.Raise<EventHandler<NewWindowRequestedEventArgs>, NewWindowRequestedEventArgs>(
					LazyWeakEventStore, nameof(NewWindowRequested),
					new Lazy<NewWindowRequestedEventArgs>(() =>
						new NewWindowRequestedEventArgs(new PrivateNewWindowRequestedEventArgs(e, this)))));
		}

		private void LifeSpanHandler_BeforeClose(object sender, BeforeCloseEventArgs e) {

		}

		private void LifeSpanHandler_AfterCreated(object sender, AfterCreatedEventArgs e) {

		}

		private void ChromiumWebBrowser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			
		}

		private void ChromiumWebBrowser_TitleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			DocumentTitle = e.NewValue as string;
		}

		private void ChromiumWebBrowser_AddressChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Address = e.NewValue as string;
		}

		/// <inheritdoc />
		public override async void Initialize(object parameter) {
			switch (parameter) {
				case null:
					throw new ArgumentNullException(nameof(parameter));
				case PrivateNewWindowRequestedEventArgs args: {
					Debug.WriteLine($"InitializeNewPresenter NewWindowRequested");
					if (args.Referrer == null) throw new ArgumentNullException(nameof(PrivateNewWindowRequestedEventArgs.Referrer));

					args.CoreArguments.NewBrowser = ChromiumWebBrowser;
					args.CoreArguments.Handled = true;
					break;
				}
				case Uri uri:
					Debug.WriteLine($"InitializeNewPresenter {uri}");
					await NavigateToUriAsync(uri);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(parameter), $"Type not supported. Type:'{parameter.GetType().FullName}'");
			}
		}

		/// <inheritdoc />
		protected override async void DoNavigate(object parameter) {
			var uri = await CommonTools.EnsureUrlAsync($"{parameter}");
			await NavigateToUriAsync(uri);
		}

		private async Task NavigateToUriAsync(Uri uri) {
			var result = await ChromiumWebBrowser.LoadUrlAsync(uri.AbsoluteUri);
		}

		private class PrivateNewWindowRequestedEventArgs : EventArgs {

			public PrivateNewWindowRequestedEventArgs(CefSpecific.NewWindowRequestedEventArgs coreArguments, CefSharpControllerVM referrer) {
				CoreArguments = coreArguments;
				Referrer = referrer;
			}
			internal CefSpecific.NewWindowRequestedEventArgs CoreArguments { get; }
			internal CefSharpControllerVM Referrer { get; }
		}
	}

}
