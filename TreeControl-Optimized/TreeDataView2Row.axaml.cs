using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;

using System.Collections.Specialized;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Row : UserControl {
	#region private
	private TreeDataView2ExpanderCell? _expanderCell;
	#endregion

	#region public
	public static StyledProperty<bool> IsSelectedProperty =
		AvaloniaProperty.Register<TreeDataViewItem, bool>(nameof(IsSelected), default, false, BindingMode.TwoWay);

	public bool IsSelected {
		get => GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}
	#endregion

	public TreeDataView2Row() {
		InitializeComponent();

		this[!IsSelectedProperty] = new Binding(nameof(IsSelected));

		PointerPressed += TreeDataView2Row_PointerPressed;
		PART_GridCells.Children.CollectionChanged += Children_CollectionChanged;
	}

	private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.Action == NotifyCollectionChangedAction.Add) {
			if (e.NewItems != null) {
				foreach (var item in e.NewItems) {
					if (item is TreeDataView2ExpanderCell expander) {
						_expanderCell = expander;
					}
				}
			}
		}
	}

	private void TreeDataView2Row_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e) {
		if (DataContext is not TreeDataView2Item item) return;
		
		TreeDataView2? root = this.FindLogicalAncestorOfType<TreeDataView2>();
		if (root == null) return;

		bool bSelected = IsSelected;

		switch (e.ClickCount) {
			case 1:
				if (root.SelectionMode.HasFlag(SelectionMode.Toggle)) {
					bSelected ^= true;
				}
				else {
					bSelected = true;
				}

				if (root.SelectionMode.HasFlag(SelectionMode.AlwaysSelected)) {
					bSelected = true;
				}
				e.Handled = true;
				break;
			case 2:
				if (_expanderCell != null) {
					if (item.IsExpandedBinding != null) {
						_expanderCell.IsExpanded ^= true;
					}

					bSelected = true;
				}
				e.Handled = true;
				break;
			default:
				break;
		}

		IsSelected = bSelected;

		root.SelectionChangedHandler(item);
	}
}