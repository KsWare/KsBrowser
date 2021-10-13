// ORIGINAL: ChromeTabsDemo\SampleData\SampleViewModelMainWindow.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Demo.ViewModel;
using KsWare.Presentation.ViewModels;

namespace Demo.DesignerData {

	public class DesignerMainWindowVM : BaseExampleWindowVM, IMainWindowVM {
		public bool CanMoveTabs {
			get => true;
			set => throw new NotImplementedException();
		}

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

		public bool ShowAddButton {
			get => true;
			set => throw new NotImplementedException();
		}
	}

}
