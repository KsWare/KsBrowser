using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

// Extended TabControl which saves the displayed item so you don't get the performance hit of 
// unloading and reloading the VisualTree when switching tabs

// Obtained from http://eric.burke.name/dotnetmania/2009/04/26/22.09.28
// and made a some modifications so it reuses a TabItem's ContentPresenter when doing drag/drop operations
// https://stackoverflow.com/a/9802346/2369575

namespace KsWare.Presentation.Controls {

	/// <inheritdoc/>
	/// <remarks>
	/// New Features:
	/// <list type="bullet">
	/// <item><description>TabItem content is cached.</description></item>
	/// <item><description><see cref="NewTabButtonTemplate"/> property.</description></item>
	/// <item><description>See also <see cref="DocumentTabPanel"/></description></item>
	/// </list>
	/// <para><b>Note: </b>If you use your own template make sure that a <see cref="Grid"/> named 'PART_ItemsHolder' exists.</para>
	/// </remarks>
	[TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Grid))]
	public sealed class DocumentTabControl : TabControl { //Alias: ContentCachingTabControl, if ContentCaching is optional use Alias: KsTabControl

		public static readonly DependencyProperty NewTabButtonTemplateProperty = DependencyProperty.Register(
			"NewTabButtonTemplate", typeof(ControlTemplate), typeof(DocumentTabControl), new PropertyMetadata(default(ControlTemplate)));

		public ControlTemplate NewTabButtonTemplate {
			get => (ControlTemplate)GetValue(NewTabButtonTemplateProperty);
			set => SetValue(NewTabButtonTemplateProperty, value);
		}

		private Panel _itemsHolderPanel;

		/// <inheritdoc/>
		public DocumentTabControl() : base() {
			// This is necessary so that we get the initial databound selected item
			ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
		}

		private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e) {
			// If containers are done, generate the selected item
			if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) {
				ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
				UpdateSelectedItem();
			}
		}

		/// <inheritdoc/>
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			// Get the ItemsHolder and generate any children
			_itemsHolderPanel = (Panel)GetTemplateChild("PART_ItemsHolder") ?? throw new KeyNotFoundException("PART_ItemsHolder not found in ControlTemplate.");
			UpdateSelectedItem();
		}

		/// <inheritdoc/>
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			// When the items change we remove any generated panel children and add any new ones as necessary

			base.OnItemsChanged(e);

			if (_itemsHolderPanel == null) return;

			switch (e.Action) {
				case NotifyCollectionChangedAction.Reset:
					_itemsHolderPanel.Children.Clear();
					break;

				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					if (e.OldItems != null) {
						foreach (var item in e.OldItems) {
							var cp = FindChildContentPresenter(item);
							if (cp != null)
								_itemsHolderPanel.Children.Remove(cp);
						}
					}

					// Don't do anything with new items because we don't want to
					// create visuals that aren't being shown

					UpdateSelectedItem();
					break;

				case NotifyCollectionChangedAction.Replace:
					throw new NotImplementedException("Replace not implemented yet");
			}
		}

		/// <inheritdoc/>
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			UpdateSelectedItem();
		}

		private void UpdateSelectedItem() {
			if (_itemsHolderPanel == null) return;

			// Generate a ContentPresenter if necessary
			var item = GetSelectedTabItem();
			if (item != null) CreateChildContentPresenter(item);

			// show the right child
			foreach (ContentPresenter child in _itemsHolderPanel.Children)
				child.Visibility = ((child.Tag as TabItem)?.IsSelected == true) ? Visibility.Visible : Visibility.Collapsed;
		}

		private ContentPresenter CreateChildContentPresenter(object item) {
			if (item == null) return null;

			var cp = FindChildContentPresenter(item);
			if (cp != null) return cp;

			// the actual child to be added.  cp.Tag is a reference to the TabItem
			cp = new ContentPresenter {
				Content = (item is TabItem tabItem) ? tabItem.Content : item,
				ContentTemplate = SelectedContentTemplate,
				ContentTemplateSelector = SelectedContentTemplateSelector,
				ContentStringFormat = SelectedContentStringFormat,
				Visibility = Visibility.Collapsed,
				Tag = (item is TabItem) ? item : ItemContainerGenerator.ContainerFromItem(item)
			};
			cp.ApplyTemplate(); // hack to early create the view
			cp.DataContext = (item as FrameworkElement)?.DataContext; // hack to early bound
			_itemsHolderPanel.Children.Add(cp);
			return cp;
		}

		private ContentPresenter FindChildContentPresenter(object data) {
			if (data is TabItem item) data = item.Content;

			if (data == null) return null;

			if (_itemsHolderPanel == null) return null;

			foreach (ContentPresenter cp in _itemsHolderPanel.Children) {
				if (cp.Content == data) return cp;
			}

			return null;
		}

		private TabItem GetSelectedTabItem() {
			var selectedItem = SelectedItem;
			if (selectedItem == null) return null;

			var item = selectedItem as TabItem;
			if (item == null) item = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;

			return item;
		}

		static DocumentTabControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentTabControl), new FrameworkPropertyMetadata(typeof(DocumentTabControl)));
		}
	}

}