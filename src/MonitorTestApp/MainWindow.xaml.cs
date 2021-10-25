using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using KsWare.Presentation.Utilities;

namespace MonitorTestApp {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private DispatcherTimer _timer;

		public MainWindow() {
			InitializeComponent();
			_timer=new DispatcherTimer(TimeSpan.FromMilliseconds(100),DispatcherPriority.Normal,Timer_Tick,Dispatcher );
		}

		private void Timer_Tick(object sender, EventArgs e) {
			var p = WinApi.GetCursorPos();
			CursorXTextBlock.Text = p.X.ToString();
			CursorYTextBlock.Text = p.Y.ToString();
			try {
				var r = WinApi.GetMonitorRectFromPoint(p);
				MonitorXTextBlock.Text = r.X.ToString();
				MonitorYTextBlock.Text = r.Y.ToString();
				MonitorWTextBlock.Text = r.Width.ToString();
				MonitorHTextBlock.Text = r.Height.ToString();
			}
			catch {
				MonitorXTextBlock.Text = "";
				MonitorYTextBlock.Text = "";
				MonitorWTextBlock.Text = "";
				MonitorHTextBlock.Text = "";
			}

		}
	}

}
