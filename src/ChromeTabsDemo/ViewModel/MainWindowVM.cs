﻿// ORIGINAL: D:\Develop\Extern\GitHub.KsWare\KsBrowser\src\ChromeTabsDemo\ViewModel\ViewModelMainWindow.cs

using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Demo.ViewModel {

	public class MainWindowVM : BaseExampleWindowVM, IMainWindowVM {
		//this property is to show you can lock the tabs with a binding
		public bool CanMoveTabs {
			get => Fields.GetValue<bool>();
			set => Fields.SetValue(value);
		}

		//this property is to show you can bind the visibility of the add button
		public bool ShowAddButton {
			get => Fields.GetValue<bool>();
			set => Fields.SetValue(value);
		}


		public MainWindowVM() {
			//Adding items to the collection creates a tab
			ItemCollection.Add(CreateTab1());
			ItemCollection.Add(CreateTab2());
			ItemCollection.Add(CreateTab3());
			ItemCollection.Add(CreateTabLoremIpsum());

			SelectedTab = ItemCollection.FirstOrDefault();
			ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);

			//This sort description is what keeps the source collection sorted, based on tab number. 
			//You can also use the sort description to manually sort the tabs, based on your own criterias.
			view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

			CanMoveTabs = true;
			ShowAddButton = true;
		}
	}

}
