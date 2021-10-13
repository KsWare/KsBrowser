// ORIGINAL: ChromeTabsDemo\ViewModel\IViewModelPinnedTabExampleWindow.cs
using KsWare.Presentation.ViewFramework.Behaviors;

namespace Demo.ViewModel {

	public interface IPinnedTabExampleWindowVM {
		RelayCommand/*ChromeTabItemVM*/ PinTabCommand { get; set; }
	}

}
