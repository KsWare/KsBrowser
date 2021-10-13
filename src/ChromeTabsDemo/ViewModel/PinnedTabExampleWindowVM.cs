//ORIGINAL: ChromeTabsDemo\ViewModel\ViewModelPinnedTabExampleWindow.cs
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModels;

namespace Demo.ViewModel {

	public class PinnedTabExampleWindowVM : BaseExampleWindowVM, IPinnedTabExampleWindowVM {

		public RelayCommand /*ChromeTabItemVM*/ PinTabCommand { get; set; }

		public PinnedTabExampleWindowVM() {
			ChromeTabItemVM vm1 = CreateTab1();
			vm1.IsPinned = true;
			ItemCollection.Add(vm1);
			ItemCollection.Add(CreateTab2());
			ItemCollection.Add(CreateTab3());
			ItemCollection.Add(CreateTabLoremIpsum());
			SelectedTab = ItemCollection.FirstOrDefault();
			ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criterias,
			//as show below by sorting both by tab number and Pinned status.
			view.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
			view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

			PinTabCommand = new RelayCommand /*ChromeTabItemVM*/(PinTabCommandAction);
		}

		private void PinTabCommandAction(object parameter) {
			ChromeTabItemVM tab = (ChromeTabItemVM)parameter;
			tab.IsPinned = !tab.IsPinned;
			ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
			view.Refresh();
		}
	}

}
