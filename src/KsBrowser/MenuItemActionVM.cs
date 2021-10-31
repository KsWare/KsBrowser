using System;
using KsWare.KsBrowser.Modules.CefModules;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.KsBrowser {

	public class MenuItemActionVM : ActionVM {

		public string Caption { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public void OnBeforeContextMenu(BeforeContextMenuEventArgs args) {
			if (CanExecuteCallback != null) {
				SetCanExecute("CanExecuteFunc",CanExecuteCallback(args));
			}
		}

		public Func<BeforeContextMenuEventArgs,bool> CanExecuteCallback { get; set; }
	}

}
