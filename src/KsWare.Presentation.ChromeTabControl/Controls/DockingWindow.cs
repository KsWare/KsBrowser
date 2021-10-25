using System;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.ViewModels;

namespace KsWare.Presentation.Controls {

	public class DockingWindow : Window {

		public static readonly DependencyProperty DragDataProperty = DependencyProperty.Register(
			"DragData", typeof(DragData), typeof(DockingWindow), new PropertyMetadata(default(DragData)));

		public DragData DragData {
			get => (DragData)GetValue(DragDataProperty);
			set => SetValue(DragDataProperty, value);
		}

		public static event EventHandler WindowCreated;
		
		static DockingWindow() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingWindow), new FrameworkPropertyMetadata(typeof(DockingWindow)));
			// /KsWare.Presentation.ChromeTabControl;component/Themes/Resources/DockingWindow.xaml
		}

		public DockingWindow() {
			var r = (ResourceDictionary)TryFindResource("DockingWindow.ContentTemplates");
			if(r!=null) Resources.MergedDictionaries.Add(r);

			WindowCreated?.Invoke(this, EventArgs.Empty);
		}

		

	}

	public class DragData {

		public ChromeTabItemVM TabItemViewModel { get; set; }
		public Point Position { get; set; }
		public bool IsDragMove { get; set; }
		public ContentPresenter ItemPresenter { get; set; }
	}
}
// https://blog.magnusmontin.net/2013/03/16/how-to-create-a-custom-window-in-wpf/comment-page-1/