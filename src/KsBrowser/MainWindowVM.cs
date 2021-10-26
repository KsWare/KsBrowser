using System;
using System.ComponentModel;
using System.Diagnostics;
using KsWare.KsBrowser.Extensions;
using KsWare.KsBrowser.Tools;
using KsWare.Presentation;
using KsWare.Presentation.ViewModels;

namespace KsWare.KsBrowser {

	public partial class MainWindowVM : ChromeTabsBaseWindowVM {

		/// <inheritdoc />
		public MainWindowVM() {
			RegisterChildren(() => this);
		}

		/// <inheritdoc />
		protected override async void DoAddNewTab(object parameter) {
			var uri = await CommonTools.EnsureUrlAsync($"{parameter}");
			AddNewTabItem(new BrowserTabCreationOptions(uri));
		}

		internal void OnStartup(Uri[] uris) {
			foreach (var uri in uris) {
				DoAddNewTab(uri);
			}
		}

		/// <inheritdoc/>
		public override void AddNewTabItem(ITabItemCreationOptions options) {
			Debug.WriteLine($"[{Environment.CurrentManagedThreadId,2}] ITabHost.AddNewTabItem options");
			BrowserTabItemVM newTabItem;
			if (options is BrowserTabCreationOptions o) {
				newTabItem = new BrowserTabItemVM(o);
				// options.Referrer?.SubTabs.Add(newTabItem);//< DEACTIVATED. managing the three for dragging between windows is currently not supported.
			}
			else {
				throw new NotSupportedException();
			}
			TabItems.Add(newTabItem);
			CurrentTabItem = newTabItem;
		}

		/// <inheritdoc/>
		/// Implements <seealso cref="IChromeTabsHostVM.CloseTabItem"/>
		public override bool CloseTabItem(ChromeTabItemVM tabItemVM) {
			var dummy = new CancelEventArgs();
			OnTabItemClosing(tabItemVM, dummy);
			var idx = TabItems.IndexOf(tabItemVM);
			TabItems.RemoveAt(idx);
			OnTabItemClosed(tabItemVM);
			if (idx == TabItems.Count) idx--;
			if (idx >= 0) {
				CurrentTabItem = (BrowserTabItemVM)TabItems[idx];
			}
			else {
				//last tab removed

				// A) close window
				// this.Close();

				// B) add empty tab
				DoAddNewTab(null);
			}
			return true;
		}
	}

}
