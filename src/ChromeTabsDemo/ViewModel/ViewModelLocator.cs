/* ORIGINAL D:\Develop\Extern\GitHub.KsWare\KsBrowser\src\ChromeTabsDemo\ViewModel\ViewModelLocator.cs
== CHANGES ==
- viewmodel locator disabled and replaced with minimalistic logic
*/
/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator x:Key="Locator" xmlns:vm="clr-namespace:Demo"/>
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.ComponentModel;
using System.Windows;

namespace Demo.ViewModel {

	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator {
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		// public ViewModelLocator() {
		// 	ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
		// 	
		// 	if (IsInDesignMode) {
		// 		// Create design time view services and models
		// 		SimpleIoc.Default.Register<IViewModelMainWindow, SampleData.SampleViewModelMainWindow>();
		// 		SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, SampleData.SampleViewModelPinnedTabExampleWindow>();
		// 		SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, SampleData.SampleViewModelCustomStyleExampleWindow>();
		// 	}
		// 	else {
		// 		// Create run time view services and models
		// 		SimpleIoc.Default.Register<IViewModelMainWindow, ViewModelMainWindow>();
		// 		SimpleIoc.Default.Register<IViewModelPinnedTabExampleWindow, ViewModelPinnedTabExampleWindow>();
		// 		SimpleIoc.Default.Register<IViewModelCustomStyleExampleWindow, ViewModelCustomStyleExampleWindow>();
		// 	}
		// }

		// public ICustomStyleExampleWindowVM CustomStyleExampleWindowVM => ServiceLocator.Current.GetInstance<ICustomStyleExampleWindowVM>();
		//
		// public IMainWindowVM MainWindowVM => ServiceLocator.Current.GetInstance<IMainWindowVM>();
		//
		// public IPinnedTabExampleWindowVM PinnedTabExampleWindowVM => ServiceLocator.Current.GetInstance<IPinnedTabExampleWindowVM>();


		public ICustomStyleExampleWindowVM CustomStyleExampleWindowVM => IsInDesignMode ? (ICustomStyleExampleWindowVM)new DesignerData.DesignerCustomStyleExampleWindowVM() : new CustomStyleExampleWindowVM();

		public IMainWindowVM MainWindowVM =>  IsInDesignMode ? (IMainWindowVM)new DesignerData.DesignerMainWindowVM() : new MainWindowVM();

		public IPinnedTabExampleWindowVM PinnedTabExampleWindowVM => IsInDesignMode ? (IPinnedTabExampleWindowVM)new DesignerData.DesignerPinnedTabExampleWindowVM() : new PinnedTabExampleWindowVM();

		public static void Cleanup() {
			// TODO Clear the ViewModels
		}
	}

}