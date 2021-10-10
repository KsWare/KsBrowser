using System.Windows;
using KsWare.Presentation;

namespace KsWare.KsBrowser.Base {

	/// <summary>
	/// Despite the MVVM pattern some time the view model needs to control the view directly.
	/// In this case you can create a ViewControllerVM.
	/// For example for AvalonEdit create AvalonEditControllerVM and keep AvalonEdit private for all other view models.
	/// </summary>
	/// <typeparam name="TView">Type of the view</typeparam>
	public interface IViewControllerVM<TView> where TView: UIElement{

		void NotifyViewChanged(object sender, ValueChangedEventArgs<TView> e);

	}

	public interface IViewControllerVM{
		void NotifyViewChanged(object sender, ValueChangedEventArgs e);
	}
}