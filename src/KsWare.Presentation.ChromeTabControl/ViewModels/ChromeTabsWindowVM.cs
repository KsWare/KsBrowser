using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.Presentation.ViewModels {

	public sealed class ChromeTabsWindowVM : ChromeTabsBaseWindowVM {

		/// <inheritdoc />
		internal ChromeTabsWindowVM() {
			RegisterChildren(() => this);

			if (IsInDesignMode) {
				TabItems.Add(new ChromeTabItemVM{Title = "Tab 1", TabNumber = 1});
				TabItems.Add(new ChromeTabItemVM{Title = "Tab 2", TabNumber = 2});
				TabItems.Add(new ChromeTabItemVM{Title = "Tab 3", TabNumber = 3});
				TabItems.Add(new ChromeTabItemVM{Title = "Tab 4", TabNumber = 4});

				CurrentTabItem = TabItems[1];
			}
		}
	}

}
