using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.LogicalTree;
using Shared;
using Avalonia.VisualTree;
using System;
using YamlDotNet.Core.Tokens;
using Avalonia.Media;
using Avalonia.Styling;

namespace AS_Desktop2.UserControls;

public class TreeDataViewColumn : UserControl {
	#region private
	private const int MinColumnWidth = 25;

	private bool _init = false;
	private GridSplitter _splitter;
	private IBinding? _binding;
	private GridLength _columnWidth = new(1, GridUnitType.Star);

	private ColumnDefinition _columnDefinition = new ColumnDefinition(1, GridUnitType.Star) { MinWidth = MinColumnWidth };
	private ColumnDefinition _columnDefinitionSplitter = new ColumnDefinition(2.5, GridUnitType.Pixel);
	#endregion

	#region public
	public static StyledProperty<IDataTemplate?> HeaderTemplateProperty =
		AvaloniaProperty.Register<TreeDataViewColumn, IDataTemplate?>(nameof(HeaderTemplate));

	public static StyledProperty<IDataTemplate?> ItemTemplateProperty =
		AvaloniaProperty.Register<TreeDataViewColumn, IDataTemplate?>(nameof(ItemTemplate));

	public static StyledProperty<string?> HeaderProperty =
		AvaloniaProperty.Register<TreeDataViewColumn, string?>(nameof(Header));

	//public static StyledProperty<string?> BindingProperty =
	//	AvaloniaProperty.Register<TreeDataViewColumn, string?>(nameof(Binding));

	public static StyledProperty<string?> ColumnGroupProperty =
		AvaloniaProperty.Register<TreeDataViewColumn, string?>(nameof(ColumnGroup));

	public IDataTemplate? HeaderTemplate {
		get => GetValue(HeaderTemplateProperty);
		set {
			SetValue(HeaderTemplateProperty, value);

			if (HeaderTemplate != null) {
				Content = HeaderTemplate.Build(null);
			}
		}
	}

	public IDataTemplate? ItemTemplate {
		get => GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	public string? Header {
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public IBinding? Binding {
		get => _binding;
		set => _binding = value;
	}

	public int ColumnIndex {
		get => Grid.GetColumn(this);
		private set => Grid.SetColumn(this, value);
	}

	public string? ColumnGroup {
		get => GetValue(ColumnGroupProperty);
		private set => SetValue(ColumnGroupProperty, value);
	}

	public GridLength ColumnWidth {
		get => _columnWidth;
		set => ColumnDefinition.Width = _columnWidth = value;
	}

	public ColumnDefinition ColumnDefinition {
		get => _columnDefinition;
	}

	public ColumnDefinition ColumnDefinitionSplitter {
		get => _columnDefinitionSplitter;
	}
	#endregion

	public TreeDataViewColumn() {
		ColumnGroup = $"group_{Guid.NewGuid().ToString().Replace('-', '_')}";
		
		_splitter = new GridSplitter() {
			HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
			Background = Brushes.Transparent,
		};

		// TODO: Axaml
		HeaderTemplate = new FuncDataTemplate(obj => true, (item, scope) => {
			return new Border() {
				Name = "Header",
				Padding = new(7.5),
				Child = new TextBlock() {
					VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
					[!TextBlock.TextProperty] = new Binding("Header") { 
						Source = this, 
					},
				},
				Styles = {
					new Style(x => x.OfType<Border>().Name("Header").PropertyEquals(Border.IsPointerOverProperty, false)) {
						Setters = {
							new Setter(Border.BackgroundProperty, Brushes.Transparent)
						}
					},
					new Style(x => x.OfType<Border>().Name("Header").PropertyEquals(Border.IsPointerOverProperty, true)) {
						Setters = {
							new Setter(Border.BackgroundProperty, Brush.Parse("#2bffffff"))
						}
					},
				}
			};
		});
	}

	public virtual Control? BuildHeader(params object?[] param) =>
		HeaderTemplate == null ? null : HeaderTemplate.Build(param);

	public virtual Control? BuildCell(params object?[] param) =>
		ItemTemplate == null ? null : ItemTemplate.Build(param);
		
	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
		if (this.GetLogicalParent() is not Grid headers || headers.Name != TreeDataView.PART_GridHeaderName) {

			base.OnAttachedToLogicalTree(e);
			return;
		}

		if (!_init) {
			Grid.SetColumn(this, headers.ColumnDefinitions.Count);
			headers.ColumnDefinitions.Add(_columnDefinition);

			Grid.SetColumn(_splitter, headers.ColumnDefinitions.Count);
			headers.ColumnDefinitions.Add(_columnDefinitionSplitter);

			headers.Children.Add(_splitter);

			_init = true;
		}

		base.OnAttachedToLogicalTree(e);
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e) {
		//if (this.GetLogicalParent() is not Grid headers || headers.Name != TreeDataView.PART_GridHeaderName) {

		//	base.OnDetachedFromLogicalTree(e);
		//	return;
		//}

		//headers.Children.Remove(_splitter);
		//headers.ColumnDefinitions.Remove(_columnDefinition);
		//headers.ColumnDefinitions.Remove(_columnDefinitionSplitter);

		base.OnDetachedFromLogicalTree(e);
	}
}
