using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2TextColumn : TreeDataView2Column {
    public TreeDataView2TextColumn() {
        InitializeComponent();
    }

	public override void ApplyBindings(Control cell) {
        if (cell is not TreeDataView2TextCell textCell) return;
       
        if (Binding != null) 
		    textCell.PART_TextBlock[!TextBlock.TextProperty] = Binding;
    }
}