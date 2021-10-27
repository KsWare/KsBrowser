// ORIGINAL ChromeTabsDemo\ViewModel\ViewModelExampleBase.cs

using System;
using System.Windows.Media.Imaging;
using Demo.Properties;
using KsWare.Presentation.ViewModels;

namespace Demo.ViewModels {

	public class BaseExampleWindowVM : ChromeTabsBaseWindowVM {

		/// <inheritdoc />
		public BaseExampleWindowVM() {
		}
		
		protected Class1TabItemVM CreateTab1() {
			var tab = new Class1TabItemVM {
				Title = "Tab class 1", MyStringContent = "Try drag the tab from left to right",
				TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative))
			};
			return tab;
		}

		protected Class2TabItemVM CreateTab2() {
			var tab = new Class2TabItemVM {
				Title = "Tab class 2, with a long name",
				MyStringContent = "Try drag the tab outside the bonds of the tab control",
				MyNumberCollection = new[] { 1, 2, 3, 4 }, MySelectedNumber = 1,
				TabIcon = new BitmapImage(new Uri("/Resources/2.png", UriKind.Relative))
			};
			return tab;

		}

		protected Class3TabItemVM CreateTab3() {
			var tab = new Class3TabItemVM {
				Title = "Tab class 3",
				MyStringContent =
					"Try right clicking on the tab header. This tab can not be dragged out to a new window, to demonstrate that you can dynamically choose what tabs can, based on the viewmodel.",
				MyImageUrl = new Uri("/Resources/Kitten.jpg", UriKind.Relative),
				TabIcon = new BitmapImage(new Uri("/Resources/3.png", UriKind.Relative))
			};
			return tab;
		}

		protected Class4TabItemVM CreateTab4() {
			var tab = new Class4TabItemVM {
				Title = "Tab class 4", MyStringContent = "This tab demonstrates a custom tab header implementation",
				IsBlinking = true
			};
			return tab;
		}

		protected Class1TabItemVM CreateTabLoremIpsum() {
			var tab = new Class1TabItemVM {
				Title = "Tab class 1", MyStringContent = Resources.LoremIpsum,
				TabIcon = new BitmapImage(new Uri("/Resources/1.png", UriKind.Relative))
			};
			return tab;
		}

		/// <inheritdoc />
		protected override void DoAddNewTab(object parameter) {
			var num = KsWare.Random.Next(1, 4);
			ChromeTabItemVM newTab;
			switch (num) {
				case 1: newTab = CreateTab1(); break;
				case 2: newTab = CreateTab2(); break;
				case 3: newTab = CreateTab3(); break;
				default: throw new Exception();
			}
			AddNewTabItem(new TabItemCreationOptions(newTab, activate:true));
		}
	}

}
