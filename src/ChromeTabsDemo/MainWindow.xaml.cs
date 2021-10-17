using System.Windows;
using ChromeTabs;
using Demo.ViewModel;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace Demo {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ChromeTabsBaseWindow {

		public MainWindow() {
			InitializeComponent();
			TabControl.TabDraggedOutsideBonds += TabControl_TabDraggedOutsideBonds;
		}

		private void BnOpenPinnedTabExample_Click(object sender, RoutedEventArgs e) {
			new PinnedTabExampleWindow().Show();
		}

		private void BnOpenCustomStyleExample_Click(object sender, RoutedEventArgs e) {
			new CustomStyleExampleWindow().Show();
		}
	}

}
