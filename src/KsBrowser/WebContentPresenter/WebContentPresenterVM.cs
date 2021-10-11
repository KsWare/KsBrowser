using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser {

	public abstract class WebContentPresenterVM : ObjectVM {

		/// <inheritdoc />
		public WebContentPresenterVM() {
			RegisterChildren(() => this);

			NavigateCommand = new RelayCommand(DoNavigate);
			RefreshCommand = new RelayCommand(DoRefresh, CanRefresh);
			// NavigateBackCommand = new RelayCommand(() => WebView2?.GoBack(), () => WebView2?.CanGoBack??false);
			// NavigateForwardCommand = new RelayCommand(() => WebView2?.GoForward(), () => WebView2?.CanGoForward??false);
		}

		public string Address { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public string DocumentTitle { get => Fields.GetValue<string>(); protected set => Fields.SetValue(value); }

		public ICommand NavigateCommand { get; }
		public ICommand RefreshCommand { get; protected set; }
		public ICommand NavigateBackCommand { get; protected set; }
		public ICommand NavigateForwardCommand { get; protected set; }
		
		public IEventSource<EventHandler<CloseRequestedEventArgs>> CloseRequested => LazyWeakEventStore.Value.Get<EventHandler<CloseRequestedEventArgs>>();
		public IEventSource<EventHandler<NewWindowRequestedEventArgs>> NewWindowRequested => LazyWeakEventStore.Value.Get<EventHandler<NewWindowRequestedEventArgs>>();

		protected abstract void DoNavigate(object parameter);
		protected virtual void DoRefresh() { }
		protected virtual bool CanRefresh() => false;

		public abstract void Initialize(object parameter);
	}
}
