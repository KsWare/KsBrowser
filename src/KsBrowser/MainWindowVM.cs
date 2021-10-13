using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.XPath;
using KsWare.KsBrowser.Extensions;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModelFramework;
using KsWare.Presentation.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace KsWare.KsBrowser {

	public class MainWindowVM : WindowVM, ITabHostVM {

		/// <inheritdoc />
		public MainWindowVM() {
			RegisterChildren(() => this);

			NewTabCommand = new RelayCommand(DoNewTab);

			// var options = new CoreWebView2EnvironmentOptions();
			// var cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KsWare", "KsBrowser", "Default");
			// Directory.CreateDirectory(cacheFolder);

			// Task.Run(async () => {
			// 	BrowserEnvironment = await CoreWebView2Environment.CreateAsync(null,cacheFolder,options);
			// 	Debug.WriteLine($"BrowserEnvironment loaded");
			// });

			Dispatcher.TryBeginInvoke(() => DoNewTab(new Uri("http://www.microsoft.de/")));
		}

		public ListVM<TabItemVM> Tabs { get => Fields.GetValue<ListVM<TabItemVM>>(); set => Fields.SetValue(value); }

		[Hierarchy(HierarchyType.Reference)]
		public BrowserTabItemVM CurrentTab { get => Fields.GetValue<BrowserTabItemVM>(); set => Fields.SetValue(value); }

		public IEventSource<EventHandler<TabCreatedEventArgs>> TabCreatedEvent => EventSources.Get<EventHandler<TabCreatedEventArgs>>(nameof(TabCreatedEvent));

		public ICommand NewTabCommand { get; }

		private async void DoNewTab(object parameter = null) {
			var uri = await CommonTools.EnsureUrlAsync($"{parameter}");
			NewTab(uri);
		}

		private void NewTab(Uri uri) {
			var newTab = new BrowserTabItemVM(this);
			Tabs.Add(newTab);
			CurrentTab = newTab;
			CommonTools.WaitForRender(); // create templates and connect the view with the controller view model
			EventManager.Raise(EventSources.Get<EventHandler<TabCreatedEventArgs>>(nameof(TabCreatedEvent)), new TabCreatedEventArgs(newTab, new BrowserTabCreationOptions(uri)));
		}

		/// <summary>Implements ITabHost.CreateNewTab</summary> 
		public void CreateNewTab(ITabCreationOptions options) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] ITabHost.CreateNewTab options");
			BrowserTabItemVM newTab;
			if (options is BrowserTabCreationOptions o) {
				newTab = new BrowserTabItemVM(this, (BrowserTabItemVM)options.Referrer);
				options.Referrer.SubTabs.Add(newTab);
			} 
			else {
				throw new NotSupportedException();
			}
			Tabs.Add(newTab);
			CurrentTab = newTab;
			CommonTools.WaitForRender(); // create templates and connect the view with the controller view model
			EventManager.Raise(EventSources.Get<EventHandler<TabCreatedEventArgs>>(nameof(TabCreatedEvent)), new TabCreatedEventArgs(newTab, options));
		}

		/// <inheritdoc/>
		/// Implements <seealso cref="ITabHostVM.CloseTab"/>
		public void CloseTab(ChromeTabItemVM tabItemVM) {
			tabItemVM.NotifyClosing();
			var idx = Tabs.IndexOf(tabItemVM);
			Tabs.RemoveAt(idx);
			if (idx == Tabs.Count) idx--;
			if (idx >= 0) {
				CurrentTab = (BrowserTabItemVM)Tabs[idx];
			}
			else {
				//last tab removed

				// A) close window
				// this.Close();

				// B) add empty tab
				DoNewTab();
			}
		}
	}

}
