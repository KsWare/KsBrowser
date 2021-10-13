/* ORIGINAL:ChromeTabsDemo\SampleData\SampleViewModelPinnedTabExampleWindow.cs
CHANGES
- remove using GalaSoft.MvvmLight;
- add KsWare.Presentation.ViewModelFramework
*/
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Demo.ViewModel;
using KsWare.Presentation.ViewFramework.Behaviors;
using KsWare.Presentation.ViewModels;

namespace Demo.DesignerData {

	class DesignerPinnedTabExampleWindowVM : BaseExampleWindowVM, IPinnedTabExampleWindowVM {

		public RelayCommand/*ChromeTabItemVM*/ PinTabCommand {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		private ObservableCollection<ChromeTabItemVM> _itemCollection;

		public new ObservableCollection<ChromeTabItemVM> ItemCollection {
			get {
				if (_itemCollection != null) return _itemCollection;
				_itemCollection = new ObservableCollection<ChromeTabItemVM>();
				ChromeTabItemVM tab1 = CreateTab1();
				tab1.IsPinned = true;
				_itemCollection.Add(tab1);
				_itemCollection.Add(CreateTab2());
				_itemCollection.Add(CreateTab3());
				_itemCollection.Add(CreateTabLoremIpsum());
				return _itemCollection;
			}
			set => _itemCollection = value;
		}

		public new ChromeTabItemVM SelectedTab {
			get => ItemCollection.FirstOrDefault();
			set => throw new NotImplementedException();
		}
	}

}
