// ORIGINAL: D:\Develop\Extern\GitHub.KsWare\KsBrowser\src\ChromeTabsDemo\ViewModel\ViewModelMainWindow.cs

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace Demo.ViewModels {

	public class MainWindowVM : BaseExampleWindowVM {
		
		public MainWindowVM() {
			RegisterChildren(()=>this);
			if(IsInDesignMode) OnStartup();

			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criteria
			SortDescriptions.Clear();
			SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

			AllowMoveTabs = true;
			ShowAddButton = true;
		}

		public void OnStartup() {
			//Adding items to the collection creates a tab
			TabItems.Add(CreateTab1());
			TabItems.Add(CreateTab2());
			TabItems.Add(CreateTab3());
			TabItems.Add(CreateTabLoremIpsum());

			CurrentTabItem = TabItems.First();
		}

		public ActionVM ShowPinnedTabExampleAction { get; [UsedImplicitly] private set; }
		public ActionVM ShowCustomStyleExampleAction { get; [UsedImplicitly] private set; }

		[UsedImplicitly]
		private void DoShowPinnedTabExample() {
			var vm = new PinnedTabExampleWindowVM();
			vm.OnStartup();
			vm.Show();
		}

		[UsedImplicitly]
		private void DoShowCustomStyleExample() {
			var vm = new CustomStyleExampleWindowVM();
			vm.OnStartup();
			vm.Show();
		}

	}

}
