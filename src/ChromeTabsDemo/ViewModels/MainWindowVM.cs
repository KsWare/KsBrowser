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
			//Adding items to the collection creates a tab
			TabItems.Add(CreateTab1());
			TabItems.Add(CreateTab2());
			TabItems.Add(CreateTab3());
			TabItems.Add(CreateTabLoremIpsum());

			CurrentTabItem = TabItems.FirstOrDefault();
			var view = CollectionViewSource.GetDefaultView(TabItems);

			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criterias.
			view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

			AllowMoveTabs = true;
			ShowAddButton = true;
		}

		public ActionVM ShowPinnedTabExampleAction { get; [UsedImplicitly] private set; }
		public ActionVM ShowCustomStyleExampleAction { get; [UsedImplicitly] private set; }

		[UsedImplicitly]
		private void DoShowPinnedTabExample() {
			new PinnedTabExampleWindowVM().Show();
		}

		[UsedImplicitly]
		private void DoShowCustomStyleExample() {
			new CustomStyleExampleWindowVM().Show();
		}
	}

}
