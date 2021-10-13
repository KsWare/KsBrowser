// ORIGINAL: ChromeTabsDemo\SampleData\SampleViewModelCustomStyleExampleWindow.cs

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Demo.ViewModel;
using KsWare.Presentation.ViewModels;

namespace Demo.DesignerData {

	class DesignerCustomStyleExampleWindowVM : BaseExampleWindowVM, ICustomStyleExampleWindowVM {
		private ObservableCollection<ChromeTabItemVM> _itemCollection;

		public new ObservableCollection<ChromeTabItemVM> ItemCollection {
			get => _itemCollection ?? (_itemCollection =
				new ObservableCollection<ChromeTabItemVM> {
					CreateTab1(),
					CreateTab2(),
					CreateTab3(),
					CreateTabLoremIpsum()
				});
			set => _itemCollection = value;
		}

		public new ChromeTabItemVM SelectedTab {
			get => ItemCollection.FirstOrDefault();
			set => throw new NotImplementedException();
		}
	}

}
