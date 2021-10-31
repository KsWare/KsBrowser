using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser.Modules.CefModules {

	public class ContextMenuHandlerVM : CefModuleBaseVM, IContextMenuHandler {

		private readonly Dictionary<CefMenuCommand, MenuItemActionVM> _map = new Dictionary<CefMenuCommand, MenuItemActionVM>();

		/// <inheritdoc />
		public ContextMenuHandlerVM() {
			RegisterChildren(() => this);

			ContextMenu.Add(
				new MenuItemActionVM {
					Caption = "Open in new tab",
					MːDoActionP = (e) => {
						var args = (ContextMenuEventArgs)e;
						args.Frame.ExecuteJavaScriptAsync($"window.open('{args.Parameters.LinkUrl}', '_blank')");
					},
					CanExecuteCallback = args => {
						return !string.IsNullOrWhiteSpace(args.Parameters.LinkUrl);
					}
				});
			ContextMenu.Add(
				new MenuItemActionVM {
					Caption = "Search in Google",
					MːDoActionP = (e) => {
						var args = (ContextMenuEventArgs)e;
						var searchAddress = "https://www.google.com/search?q=" + args.Parameters.SelectionText;
						args.Frame.ExecuteJavaScriptAsync($"window.open('{searchAddress}', '_blank')");
					},
					CanExecuteCallback = args => {
						return !string.IsNullOrWhiteSpace(args.Parameters.SelectionText);
					}
				});
		}

		public ListVM<MenuItemActionVM> ContextMenu { get; [UsedImplicitly] private set; }

		/// <inheritdoc />
		public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, 
			IFrame frame, IContextMenuParams parameters, IMenuModel model) {

			model.Clear(); 
			_map.Clear();
			var i = 0;
			foreach (var item in ContextMenu) {
				item.OnBeforeContextMenu(new BeforeContextMenuEventArgs(chromiumWebBrowser, browser, frame, parameters, model));
				var commandId = CefMenuCommand.CustomFirst + (i++);
				model.AddItem(commandId, item.Caption);
				model.SetEnabledAt(1, item.CanExecute);
				_map.Add(commandId, item);
			}
		}

		/// <inheritdoc />
		public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
			IContextMenuParams parameters,
			CefMenuCommand commandId, CefEventFlags eventFlags) {

			if(_map.TryGetValue(commandId, out var menuItemVM)) {
				if (!menuItemVM.CanExecute) return false;
				var parameter = new ContextMenuEventArgs(chromiumWebBrowser, browser, frame, parameters, commandId, eventFlags);
				menuItemVM.Execute(parameter);
				return true;
			}

			// switch (commandId) {
			// 	case CefMenuCommand.Find: {
			// 		var searchAddress = "https://www.bing.com/search?q=" + parameters.SelectionText;
			// 		frame.ExecuteJavaScriptAsync($"window.open('{searchAddress}', '_blank')");
			// 		return true;
			// 	}
			// 	case CefMenuCommand.CustomFirst + 1: {
			// 		var searchAddress = "https://www.google.com/search?q=" + parameters.SelectionText;
			// 		frame.ExecuteJavaScriptAsync($"window.open('{searchAddress}', '_blank')");
			// 		return true;
			// 	}
			// 	default:
			// 		return false;
			// }

			return false;
		}

		/// <inheritdoc />
		public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame) {
			
		}

		/// <inheritdoc />
		public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
			IContextMenuParams parameters,
			IMenuModel model, IRunContextMenuCallback callback) {
			return false;
		}
	}

	public class BeforeContextMenuEventArgs : EventArgs {

		public BeforeContextMenuEventArgs(IWebBrowser chromiumWebBrowser, IBrowser browser, 
			IFrame frame, IContextMenuParams parameters, IMenuModel model) {
			ChromiumWebBrowser = chromiumWebBrowser;
			Browser = browser;
			Frame = frame;
			Parameters = parameters;
			Model = model;
		}

		public IWebBrowser ChromiumWebBrowser { get; }
		public IBrowser Browser { get; }
		public IFrame Frame { get; }
		public IContextMenuParams Parameters { get; }
		public IMenuModel Model { get; }
	}

	public class ContextMenuEventArgs : EventArgs {

		/// <inheritdoc />
		public ContextMenuEventArgs(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
			IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags) {
			ChromiumWebBrowser = chromiumWebBrowser;
			Browser = browser;
			Frame = frame;
			Parameters = parameters;
			CommandId = commandId;
			EventFlags = eventFlags;
		}

		public IWebBrowser ChromiumWebBrowser { get; }
		public IBrowser Browser { get; }
		public IFrame Frame { get; }
		public IContextMenuParams Parameters { get; }
		public CefMenuCommand CommandId { get; }
		public CefEventFlags EventFlags { get; }
	}

}
