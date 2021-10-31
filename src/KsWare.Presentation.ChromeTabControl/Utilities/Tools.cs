using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChromeTabs;
using KsWare.Presentation.Controls;
using KsWare.Presentation.ViewModels;

namespace KsWare.Presentation.Utilities {

	internal static class Tools {

		private static readonly Dictionary<Type, bool> ItemsHolderSupport = new Dictionary<Type, bool>();

		public static bool SupportsItemsHolder(Window window) {
			var t = window.GetType();
			if (ItemsHolderSupport.ContainsKey(t)) return ItemsHolderSupport[t];
			if (!window.IsLoaded) throw new InvalidOperationException();
			var tabControl = FindTabControl(window);
			if (tabControl == null) {
				ItemsHolderSupport.Add(t, false);
				return false;
			}
			var itemsHolder = FindVisualTreeElement<Panel>(tabControl,"PART_ItemsHolder");
			if (itemsHolder == null) {
				ItemsHolderSupport.Add(t, false);
				return false;
			}
			ItemsHolderSupport.Add(t, true);
			return true;
		}

		public static Panel FindItemsHolder(ChromeTabsBaseWindow window) {
			return FindVisualTreeElement<Panel>(FindTabControl(window), "PART_ItemsHolder");
		}

		public static T FindVisualTreeElement<T>(DependencyObject parent) where T:FrameworkElement{
			var children = VisualTreeHelper_GetChildren(parent);
			var elmt = children.OfType<T>().FirstOrDefault();
			if (elmt != null) return elmt;
			foreach (var child in children.OfType<DependencyObject>()) {
				elmt = FindVisualTreeElement<T>(child);
				if (elmt != null) return elmt;
			}
			return null;
		}

		public static T FindVisualTreeElement<T>(DependencyObject parent, string name) where T:FrameworkElement{
			var children = VisualTreeHelper_GetChildren(parent);
			var elmt = children.OfType<T>().FirstOrDefault(t => t.Name == name);
			if (elmt != null) return elmt;
			foreach (var child in children.OfType<DependencyObject>()) {
				elmt = FindVisualTreeElement<T>(child, name);
				if (elmt != null) return elmt;
			}
			return null;
		}

		private static ICollection<DependencyObject> VisualTreeHelper_GetChildren(DependencyObject parent) {
			var children = new List<DependencyObject>();
			var count = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < count; i++) {
				children.Add(VisualTreeHelper.GetChild(parent, i));
			}
			return children;
		}

		public static ContentPresenter FindContentPresenter(FrameworkElement itemHolder, ChromeTabItemVM tabItemVM) {
			if(itemHolder.Name!="PART_ItemsHolder") itemHolder = FindVisualTreeElement<Panel>(itemHolder, "PART_ItemsHolder");
			var panel = (Panel)itemHolder;
			var presenter = panel.Children.OfType<ContentPresenter>().FirstOrDefault(c => c.Content == tabItemVM);
			return presenter;
		}

		public static ChromeTabControl FindTabControl(DependencyObject parent) {
			var children = VisualTreeHelper_GetChildren(parent);
			if (children.Count == 0) children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>().ToArray();
			var tabControl = children.OfType<ChromeTabControl>().FirstOrDefault();
			if (tabControl != null) return tabControl;
			foreach (var child in children.OfType<DependencyObject>()) {
				tabControl = FindTabControl(child);
				if (tabControl != null) return tabControl;
			}
			return null;
		}
	}

}
