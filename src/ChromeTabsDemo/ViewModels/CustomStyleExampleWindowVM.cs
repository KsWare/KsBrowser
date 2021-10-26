// ORIGINAL: ChromeTabsDemo\ViewModel\ViewModelCustomStyleExampleWindow.cs

using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Demo.ViewModels {

	public class CustomStyleExampleWindowVM : BaseExampleWindowVM {

		public CustomStyleExampleWindowVM() {
			TabItems.Add(CreateTab1());
			TabItems.Add(CreateTab2());
			TabItems.Add(CreateTab3());
			TabItems.Add(CreateTab4());

			CurrentTabItem = TabItems.FirstOrDefault();
			var view = CollectionViewSource.GetDefaultView(TabItems);
			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criteria.
			view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
		}
	}

}
