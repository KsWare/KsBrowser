using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using CefSharp.Wpf;
using KsWare.KsBrowser.Base;
using KsWare.KsBrowser.CefSpecific;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.Controls;

namespace KsWare.KsBrowser {

	public class CefSharpControllerVM : WebContentPresenterVM, IViewControllerVM<ChromiumWebBrowser>  {

		/// <inheritdoc />
		public CefSharpControllerVM() {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] new CefSharpControllerVM");
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
				Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] {nameof(CefSharpControllerVM)}.{nameof(NotifyViewChanged)}");
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

		/// <remarks>
		/// --> <see cref="BrowserTabItemVM.WebContentPresenter_NewWindowRequested"/>
		/// ==> <see cref="Initialize">Initialize(PrivateNewWindowRequestedEventArgs)</see>
		/// </remarks>
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
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] LifeSpanHandler_AfterCreated");
		}

		private void ChromiumWebBrowser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] ChromiumWebBrowser_IsBrowserInitializedChanged");
		}

		private void ChromiumWebBrowser_TitleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			DocumentTitle = e.NewValue as string;
		}

		private void ChromiumWebBrowser_AddressChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			Address = e.NewValue as string;
			Source =  Uri.TryCreate(Address,UriKind.RelativeOrAbsolute, out var uri) ? uri : new Uri("about:unknown");
		}

		/// <inheritdoc />
		public override async void Initialize(object parameter) {
			switch (parameter) {
				case null:
					throw new ArgumentNullException(nameof(parameter));
				case PrivateNewWindowRequestedEventArgs args: {
					Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] InitializeNewPresenter NewWindowRequested");
					if (args.Referrer == null) throw new ArgumentNullException(nameof(PrivateNewWindowRequestedEventArgs.Referrer));

					// == BUG CefBrowser can not use the new ChromiumWebBrowser
					args.CoreArguments.NewBrowser = ChromiumWebBrowser;
					args.CoreArguments.Handled = true;
					// == WORKAROUND
					// await Task.Run(async () => { while (ChromiumWebBrowser.IsBrowserInitialized == false) { await Task.Delay(25).ConfigureAwait(false); } });
					await Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, () => { });
					await NavigateToUriAsync(new Uri(args.CoreArguments.TargetUrl, UriKind.Absolute)); 
					// == END
					// System.Exception: 'The browser has not been initialized. Load can only be called after the underlying CEF browser is initialized (CefLifeSpanHandler::OnAfterCreated).'
					// WORKAROUND: wait for OnAfterCreated

					break;
				}
				case Uri uri:
					Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] InitializeNewPresenter {uri}");
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

		public void MoveToNewWindow(DockingWindow window) {
			var parent = (UserControl)ChromiumWebBrowser.Parent;
			parent.Content = null;
			window.Content = ChromiumWebBrowser;
		}
	}

}
