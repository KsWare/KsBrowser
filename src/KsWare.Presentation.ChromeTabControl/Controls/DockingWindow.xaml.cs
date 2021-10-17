using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace KsWare.Presentation.Controls {

	/// <summary>
	/// Interaction logic for DockingWindow.xaml
	/// </summary>
	public partial class DockingWindow : Window {

		public static event EventHandler WindowCreated;
		
		public DockingWindow() {
			InitializeComponent();

			var r = (ResourceDictionary)TryFindResource("DockingWindow.Resources");
			if(r!=null) Resources.MergedDictionaries.Add(r);
			WindowCreated?.Invoke(this, EventArgs.Empty);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			Storyboard sb = Resources["FadeInContentAnim"] as Storyboard;
			sb?.Begin();
		}
	}

}
