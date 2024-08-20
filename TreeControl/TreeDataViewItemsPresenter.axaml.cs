using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections;

namespace AS_Desktop2.UserControls;

public partial class TreeDataViewItemsPresenter : UserControl {
	#region private
	private TreeDataViewItemsPresenter? _next = null;
	#endregion

	#region public
	public static StyledProperty<IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.Register<TreeDataView, IEnumerable?>(nameof(ItemsSource));

	public static StyledProperty<int> LevelProperty =
		AvaloniaProperty.Register<TreeDataViewExpanderColumn, int>(nameof(Level), 0);

	public int Level {
		get => GetValue(LevelProperty);
		set => SetValue(LevelProperty, value);
	}

	public IEnumerable? ItemsSource {
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}
	#endregion

	public TreeDataViewItemsPresenter() {
        InitializeComponent();
    }
}