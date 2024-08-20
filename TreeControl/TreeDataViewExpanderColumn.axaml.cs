using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;

namespace AS_Desktop2.UserControls;

public partial class TreeDataViewExpanderColumn : TreeDataViewColumn {
	#region private
	private const string ItemContainerTemplate = nameof(ItemContainerTemplate);

	private IBinding? _innerItemsBinding;
	private IBinding? _isExpandedBinding;

	private IDataTemplate _defaultItemTemplate;
	private IDataTemplate _itemContainerTemplate;
	#endregion

	#region public
	public IBinding? InnerItemsBinding {
		get => _innerItemsBinding;
		set => _innerItemsBinding = value;
	}

	public IBinding? IsExpandedBinding {
		get => _isExpandedBinding;
		set => _isExpandedBinding = value;
	}
	#endregion

	public TreeDataViewExpanderColumn() {
        InitializeComponent();

		_defaultItemTemplate = Resources.GetResourceValue<IDataTemplate>(nameof(ItemTemplate)) ?? throw new NullReferenceException($"{nameof(ItemTemplate)} is null");
		_itemContainerTemplate = Resources.GetResourceValue<IDataTemplate>(ItemContainerTemplate) ?? throw new NullReferenceException($"{ItemContainerTemplate} is null");
    }

	public override Control? BuildCell(params object?[] param) {
		Control? itemContainer = _itemContainerTemplate.Build(null); //base.BuildCell(param);
		if (itemContainer == null) {
			return null;
		}

		Thickness margin = new();

		if (param.Length > 0 && param[0] is TreeDataViewItemsPresenter presenter) {
			margin = new(presenter.Level * 25, 0, 10, 0);
		}

		Border? PART_ExpanderBorder = itemContainer.FindVisualChildren<Border>(nameof(PART_ExpanderBorder));
		if (PART_ExpanderBorder != null) {
			PART_ExpanderBorder.Margin = margin;
		}

		ToggleSwitch? PART_ExpanderCell = itemContainer.FindVisualChildren<ToggleSwitch>(nameof(PART_ExpanderCell));
		if (PART_ExpanderCell != null && IsExpandedBinding != null) {
			PART_ExpanderCell[!ToggleSwitch.IsCheckedProperty] = IsExpandedBinding;

			if (param.Length > 1 && param[1] is TreeDataViewItemsPresenter innerPresenter) {
				PART_ExpanderCell[!ToggleSwitch.IsVisibleProperty] = new Binding($"!!{nameof(innerPresenter.ItemsSource)}.Count") { Source = innerPresenter };
			}
		}

		Grid? PART_ExpanderPanel = itemContainer.FindVisualChildren<Grid>(nameof(PART_ExpanderPanel));
		if (PART_ExpanderPanel != null) {
			Control? item;

			if (ItemTemplate == null) {
				item = _defaultItemTemplate.Build(null);

				if (item != null) {
					TextBlock? PART_TextCell = item.FindVisualChildren<TextBlock>(nameof(PART_TextCell));
					if (PART_TextCell != null && Binding != null) {
						PART_TextCell[!TextBlock.TextProperty] = Binding;
					}
				}
			}
			else {
				item = base.BuildCell(param);
			}

			if (item != null) {
				Grid.SetColumn(item, 1);
				PART_ExpanderPanel.Children.Add(item);
			}
		}

		return itemContainer;
	}
}