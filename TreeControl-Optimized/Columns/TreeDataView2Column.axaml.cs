using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Column : UserControl {
	#region private
	private const double MinColumnWidth = 100;
	private const double SplitterWidth = 1;

	private int _columnIndex = -1;

	private IBinding? _binding;

	private Grid? _grid = null;

	private ColumnDefinition _columnHeader = new(1, GridUnitType.Star) { MinWidth = MinColumnWidth };
	private ColumnDefinition _columnSplitter = new(SplitterWidth, GridUnitType.Pixel);
	private GridSplitter _splitter;
	#endregion

	#region public
	public static DirectProperty<TreeDataView2Column, IDataTemplate?> HeaderTemplateProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Column, IDataTemplate?>(nameof(HeaderTemplate), x => x.HeaderTemplate, (x, template) => x.HeaderTemplate = template);

	public static DirectProperty<TreeDataView2Column, GridLength> ColumnWidthProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Column, GridLength>(nameof(ColumnWidth), x => x.ColumnWidth, (x, width) => x.ColumnWidth = width);

	public static StyledProperty<IDataTemplate?> ItemTemplateProperty =
		AvaloniaProperty.Register<TreeDataView2Column, IDataTemplate?>(nameof(ItemTemplate));

	public static StyledProperty<IDataTemplate?> CellContentTemplateProperty =
		AvaloniaProperty.Register<TreeDataView2Column, IDataTemplate?>(nameof(CellContentTemplate));

	public static StyledProperty<string?> HeaderProperty =
		AvaloniaProperty.Register<TreeDataView2Column, string?>(nameof(Header));

	public IDataTemplate? HeaderTemplate {
		get => GetValue(ContentTemplateProperty);
		set => SetValue(ContentTemplateProperty, value);
	}

	public IDataTemplate? ItemTemplate {
		get => GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	public IDataTemplate? CellContentTemplate {
		get => GetValue(CellContentTemplateProperty);
		set => SetValue(CellContentTemplateProperty, value);
	}

	public string? Header {
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public int ColumnIndex {
		get => _columnIndex;
		set => _columnIndex = value;
	}

	public GridLength ColumnWidth {
		get => _columnHeader.Width;
		set => _columnHeader.Width = value;
	}

	public IBinding? Binding {
		get => _binding;
		set => _binding = value;
	}
	#endregion

	public TreeDataView2Column() {
        InitializeComponent();

		_splitter = GenerateSplitter();
    }

	public void AppendToGrid(Grid grid) {
		if (_grid != null) {
			RemoveFromGrid();
		}
		
		_grid = grid;

		_columnIndex = _grid.ColumnDefinitions.Count;

		_grid.ColumnDefinitions.Add(_columnHeader);
		_grid.ColumnDefinitions.Add(_columnSplitter);

		Grid.SetColumn(this, _columnIndex);
		Grid.SetColumn(_splitter, _columnIndex + 1);

		_grid.Children.Add(this);
		_grid.Children.Add(_splitter);
	}

	public void RemoveFromGrid() {
		if (_grid == null) return;

		_columnIndex = -1;

		_grid.Children.Remove(this);
		_grid.Children.Remove(_splitter);

		_grid.ColumnDefinitions.Remove(_columnHeader);
		_grid.ColumnDefinitions.Remove(_columnSplitter);

		_grid = null;
	}

	private GridSplitter GenerateSplitter() => new GridSplitter() { 
		Background = Brushes.Transparent, 
		HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch 
	};

	public void BuildCell(TreeDataView2Row row, object? param, INameScope scope) {
		row.PART_GridCells.ColumnDefinitions.Add(new ColumnDefinition() {
			[!ColumnDefinition.WidthProperty] = new Binding() {
				Path = nameof(Width),
				Source = _columnHeader,
			},
		});
		row.PART_GridCells.ColumnDefinitions.Add(new ColumnDefinition(SplitterWidth, GridUnitType.Pixel));

		if (ItemTemplate == null) {
			return;
		}

		Control? control = ItemTemplate.Build(param);
		if (control is not TreeDataView2Cell cell) {
			return;
		}

		cell[!TreeDataView2Cell.DataContextProperty] = new Binding(nameof(DataContext));
		cell[!TreeDataView2Cell.CellContentTemplateProperty] = new Binding(nameof(CellContentTemplate), BindingMode.TwoWay) { Source = this };

		ApplyBindings(cell);

		Grid.SetColumn(cell, ColumnIndex);
		row.PART_GridCells.Children.Add(cell);
	}

	public virtual void ApplyBindings(Control cell) { }
}