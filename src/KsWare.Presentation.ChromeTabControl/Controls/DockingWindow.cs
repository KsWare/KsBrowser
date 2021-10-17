using System;
using System.Windows;

namespace KsWare.Presentation.Controls {

	public class DockingWindow : Window {

		public static readonly DependencyProperty DragContentProperty = DependencyProperty.Register(
			"DragContent", typeof(object), typeof(DockingWindow), new PropertyMetadata(default(object)));

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

		public object DragContent {
			get => (object)GetValue(DragContentProperty);
			set => SetValue(DragContentProperty, value);
		}

	}
}
// https://blog.magnusmontin.net/2013/03/16/how-to-create-a-custom-window-in-wpf/comment-page-1/