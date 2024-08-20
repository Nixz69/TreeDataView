using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS_Desktop2.UserControls;

public class TreeDataViewItem : UserControl {
	#region private
	private static string PART_StackItemPresenter = nameof(PART_StackItemPresenter);
	private static string PART_ItemRow = nameof(PART_ItemRow);

	private TreeDataView? _treeDataView = null;

	private bool _builded = false;
	#endregion

	#region public
	public static StyledProperty<bool> IsSelectedProperty =
		AvaloniaProperty.Register<TreeDataViewItem, bool>(nameof(IsSelected));

	public bool IsSelected {
		get => GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}
	#endregion

	public TreeDataViewItem() {
		Focusable = true;
		
		PointerPressed += TreeDataViewItem_PointerPressed;
		DoubleTapped += TreeDataViewItem_DoubleTapped;
	}

	public void Build() {
		if (_treeDataView == null) {
			_treeDataView = this.GetLogicalParentOfType<TreeDataView>();

			if (_treeDataView == null) return;
		}

		StackPanel? parent = this.GetLogicalParent<StackPanel>();
		if (parent == null || parent.Name != PART_StackItemPresenter) return;

		int level = 0;
		TreeDataViewItemsPresenter? itemsPresenter = this.GetLogicalParentOfType<TreeDataViewItemsPresenter>();
		if (itemsPresenter != null) {
			level = itemsPresenter.Level;
		}

		Grid item = new() {
			Name = PART_ItemRow,
		};

		foreach (var column in _treeDataView.Columns) {
			List<object?> @params = [itemsPresenter];

			if (column is TreeDataViewExpanderColumn expander) {
				if (expander.IsExpandedBinding != null &&
					expander.InnerItemsBinding != null) {

					TreeDataViewItemsPresenter innerItemsPresenter = new() {
						[!TreeDataViewItemsPresenter.IsVisibleProperty] = expander.IsExpandedBinding,
						[!TreeDataViewItemsPresenter.ItemsSourceProperty] = expander.InnerItemsBinding,
						Level = level + 1,
					};
					@params.Add(innerItemsPresenter);
					parent.Children.Add(innerItemsPresenter);
				}
			}

			item.ColumnDefinitions.Add(new ColumnDefinition() {
				[!ColumnDefinition.WidthProperty] = new Binding() {
					Path = "Width",
					Source = column.ColumnDefinition,
				},
				SharedSizeGroup = column.ColumnGroup,
			});

			item.ColumnDefinitions.Add(new ColumnDefinition(2.5, GridUnitType.Pixel));

			Control? control = column.BuildCell(@params.ToArray());
			if (control == null) continue;
			control.ClipToBounds = true;

			Grid.SetColumn(control, column.ColumnIndex);
			item.Children.Add(control);
		}

		Content = item;
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
		if (!_builded) {
			Build();

			_builded = true;
		}

		base.OnAttachedToLogicalTree(e);
	}

	private void TreeDataViewItem_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e) {
		if (_treeDataView == null) return;

		_treeDataView.SelectionChangedHandler(this);
	}

	private void TreeDataViewItem_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e) {
		if (_treeDataView == null) return;

		// TODO: ...
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
		base.OnDetachedFromLogicalTree(e);
	}
}
