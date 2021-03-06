//ORIGINAL: ChromeTabsDemo\ViewModel\ViewModelPinnedTabExampleWindow.cs

using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KsWare.Presentation.ViewModels;

namespace Demo.ViewModels {

	public class PinnedTabExampleWindowVM : BaseExampleWindowVM {

		public PinnedTabExampleWindowVM() {
			if (IsInDesignMode) OnStartup();

			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criteria,
			//as show below by sorting both by tab number and Pinned status.
			SortDescriptions.Clear();
			SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
			SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
		}

		public void OnStartup() {
			ChromeTabItemVM vm1 = CreateTab1();
			vm1.IsPinned = true;
			TabItems.Add(vm1);
			TabItems.Add(CreateTab2());
			TabItems.Add(CreateTab3());
			TabItems.Add(CreateTabLoremIpsum());
			CurrentTabItem = TabItems.FirstOrDefault();
		}
	}

}
