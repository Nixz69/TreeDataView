using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Shared;
using System;

namespace AS_Desktop2.UserControls;

public partial class TreeDataViewTextColumn : TreeDataViewColumn {
    public TreeDataViewTextColumn() {
        InitializeComponent();

        ItemTemplate = Resources.GetResourceValue<IDataTemplate>(nameof(ItemTemplate));
    }

	public override Control? BuildCell(params object?[] param) {
		Control? item = base.BuildCell(param);
		
		if (item != null && Binding != null) {
			TextBlock? PART_CellText = item.FindVisualChildren<TextBlock>(nameof(PART_CellText));

			if (PART_CellText != null) {
				try {
					PART_CellText[!TextBlock.TextProperty] = Binding;
				}
				catch (Exception e) {
					Log.Error(e.Message);
				}
			}
		}

		return item;
	}
}