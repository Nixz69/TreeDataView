using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2ExpanderCell : TreeDataView2Cell {
	#region private
	private IEnumerable? _innerItemsProperty;
	#endregion

	#region public
	//public static DirectProperty<TreeDataView2ExpanderCell, IEnumerable?> InnerItemsSourceProperty =
	//	AvaloniaProperty.RegisterDirect<TreeDataView2ExpanderCell, IEnumerable?>(nameof(InnerItemsSource), x => x.InnerItemsSource, (x, value) => x.InnerItemsSource = value);

	public static StyledProperty<bool> IsExpandedProperty =
		AvaloniaProperty.Register<TreeDataView2ExpanderCell, bool>(nameof(IsExpanded), default, false, Avalonia.Data.BindingMode.TwoWay);

	//public IEnumerable? InnerItemsSource {
	//	get => _innerItemsProperty;
	//	set {
	//		_innerItemsProperty = value;
	//	}
	//}

	public bool IsExpanded {
		get => GetValue(IsExpandedProperty);
		set => SetValue(IsExpandedProperty, value);
	}

	#endregion

	public TreeDataView2ExpanderCell() {
        InitializeComponent();
    }
}