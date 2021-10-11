using System.Diagnostics;
using System.Windows;

namespace KsWare.KsBrowser {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			Debug.WriteLine($"new MainWindow");
			InitializeComponent();
		}
	}
}
