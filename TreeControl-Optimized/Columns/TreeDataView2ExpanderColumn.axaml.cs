using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2ExpanderColumn : TreeDataView2Column {
	#region private
	private IBinding? _innerItemsSourceBinding;
	private IBinding? _isExpandedBinding;
	#endregion

	#region public
	public IBinding? InnerItemsSource {
		get => _innerItemsSourceBinding;
		set => _innerItemsSourceBinding = value;
	}

	public IBinding? IsExpanded {
		get => _isExpandedBinding;
		set => _isExpandedBinding = value;
	}
	#endregion

	public TreeDataView2ExpanderColumn() {
        InitializeComponent();
    }

	public override void ApplyBindings(Control cell) {
		if (cell is not TreeDataView2ExpanderCell expanderCell) return;
		
		if (Binding != null) 
			expanderCell.PART_TextBlock[!TextBlock.TextProperty] = Binding;
		if (IsExpanded != null)
			expanderCell[!TreeDataView2ExpanderCell.IsExpandedProperty] = IsExpanded;

		// expanderCell[!TreeDataView2ExpanderCell.InnerItemsSourceProperty] = InnerItemsSource;
	}
}