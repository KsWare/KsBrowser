using System.Windows;
using ChromeTabs;
using Demo.ViewModel;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace Demo
{
    /// <summary>
    /// Interaction logic for PinnedTabExample.xaml
    /// </summary>
    public partial class PinnedTabExampleWindow : ChromeTabsBaseWindow
    {
        public PinnedTabExampleWindow()
        {
            InitializeComponent();
        }

        private void TabControl_ContainerItemPreparedForOverride(object sender, ContainerOverrideEventArgs e)
        {
            e.Handled = true;
            if (e.TabItem != null && e.Model is ChromeTabItemVM viewModel)
            {
                e.TabItem.IsPinned = viewModel.IsPinned;
            }
        }

        private void TabControl_TabDraggedOutsideBonds(object sender, TabDragEventArgs e)
        {
            ChromeTabItemVM draggedTab = e.Tab as ChromeTabItemVM;
            if (TryDragTabToWindow(e.CursorPosition, draggedTab))
            {
                //Set Handled to true to tell the tab control that we have dragged the tab to a window, and the tab should be closed.
                e.Handled = true;
            }
        }

        protected override bool TryDockWindow(Point position, ChromeTabItemVM dockedWindowVM)
        {
            //Hit test against the tab control
            if (MyChromeTabControlWithPinnedTabs.InputHitTest(position) is FrameworkElement element)
            {
                ////test if the mouse is over the tab panel or a tab item.
                if (CanInsertTabItem(element))
                {
                    //TabBase dockedWindowVM = (TabBase)win.DataContext;
                    BaseExampleWindowVM vm = (BaseExampleWindowVM)DataContext;
                    vm.ItemCollection.Add(dockedWindowVM);
                    vm.SelectedTab = dockedWindowVM;
                    //We run this method on the tab control for it to grab the tab and position it at the mouse, ready to move again.
                    MyChromeTabControlWithPinnedTabs.GrabTab(dockedWindowVM);
                    return true;
                }
            }
            return false;
        }
    }
}
