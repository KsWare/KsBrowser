using System.Windows;
using ChromeTabs;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace Demo {

	/// <summary>
	/// Interaction logic for PinnedTabExample.xaml
	/// </summary>
	public partial class PinnedTabExampleWindow : ChromeTabsBaseWindow {

		public PinnedTabExampleWindow() {
			InitializeComponent();
			TabControl.TabDraggedOutsideBonds += TabControl_TabDraggedOutsideBonds;
			TabControl.ContainerItemPreparedForOverride += TabControl_ContainerItemPreparedForOverride;
		}

	}

}
