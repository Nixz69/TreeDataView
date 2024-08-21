using AS_Desktop2.UserControls.Behaviors;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CSScripting;
using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2 : UserControl {
	#region private
	private IEnumerable? _itemsSource;
	private TreeDataView2ExpanderColumn? _expanderColumn;
	private List<TreeDataView2Item> _itemsViewSource = new();
	#endregion

	#region public
	public static StyledProperty<SelectionMode> SelectionModeProperty =
		AvaloniaProperty.Register<TreeDataView, SelectionMode>(nameof(SelectionMode), SelectionMode.Single);

	public static StyledProperty<TreeDataView2Item?> SelectedItemProperty =
		AvaloniaProperty.Register<TreeDataView, TreeDataView2Item?>(nameof(SelectedItem));

	public static StyledProperty<List<TreeDataView2Item>> SelectedItemsProperty =
		AvaloniaProperty.Register<TreeDataView, List<TreeDataView2Item>>(nameof(SelectedItems), new());

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<TreeDataView, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	public static DirectProperty<TreeDataView2, IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2, IEnumerable?>(nameof(ItemsSource), x => x.ItemsSource, (x, value) => x.ItemsSource = value);

	public static StyledProperty<TreeDataView2ItemRoot> ItemRootProperty =
		AvaloniaProperty.Register<TreeDataView2, TreeDataView2ItemRoot>(nameof(ItemRoot));

	public IEnumerable? ItemsSource {
		get => _itemsSource;
		set {
			_itemsSource = value;
			Dispatcher.UIThread.Post(UpdateSource, DispatcherPriority.Background);
		}
	}

	public TreeDataView2ItemRoot ItemRoot {
		get => GetValue(ItemRootProperty);
		set => SetValue(ItemRootProperty, value);
	}

	public ObservableCollection<TreeDataView2Column> Columns { get; } = new();

	public SelectionMode SelectionMode {
		get => GetValue(SelectionModeProperty);
		set => SetValue(SelectionModeProperty, value);
	}

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged {
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	public TreeDataView2Item? SelectedItem {
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public List<TreeDataView2Item> SelectedItems {
		get => GetValue(SelectedItemsProperty);
		set => SetValue(SelectedItemsProperty, value);
	}
	#endregion

	public TreeDataView2() {
        InitializeComponent();

		Columns.CollectionChanged += Columns_CollectionChanged;

		PART_ItemsRepeater.ItemTemplate = new FuncDataTemplate(@object => true, BuildRow);
	}

	private TreeDataView2Row? BuildRow(object? @object, INameScope scope) {
		TreeDataView2Row item = new();

		foreach (var column in Columns) 
			column.BuildCell(item, @object, scope);

		return item;
	}

	private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
		switch (e.Action) {
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				if (e.NewItems == null)
					break;
				foreach (var it in e.NewItems.OfType<TreeDataView2Column>()) {
					if (it is TreeDataView2ExpanderColumn expander) {
						_expanderColumn = expander;
					}

					it.AppendToGrid(PART_GridHeaders);
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if (e.OldItems == null)
					break;
				foreach (var it in e.OldItems.OfType<TreeDataView2Column>()) {
					it.RemoveFromGrid();
				}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				TreeDataView2Column[] columns = PART_GridHeaders.Children.OfType<TreeDataView2Column>().ToArray();
				foreach (var it in columns) {
					it.RemoveFromGrid();
				}
				break;
			// TODO: остальные действия над коллекцией
			default:
				break;
		}
	}

	//private void TreeDataView2_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e) {
	//	if (SelectedItem == null) return;

	//	if (e.KeyModifiers == Avalonia.Input.KeyModifiers.None) {
	//		int index = ItemRoot.ItemsView.IndexOf(SelectedItem);
	//		if (index < 0) return;

	//		switch (e.Key) {
	//			case Avalonia.Input.Key.Up:
	//				if (index - 1 >= 0) {
	//					var item = ItemRoot.ItemsView.ElementAt(index - 1);

	//					SelectedItem.IsSelected = false;
	//					item.IsSelected = true;
	//					SelectionChangedHandler(item);
	//				}
	//				break;
	//			case Avalonia.Input.Key.Down:
	//				if (index + 1 < ItemRoot.ItemsView.Count) {
	//					var item = ItemRoot.ItemsView.ElementAt(index + 1);

	//					SelectedItem.IsSelected = false;
	//					item.IsSelected = true;
	//					SelectionChangedHandler(item);
	//				}
	//				break;
	//			default:
	//				break;
	//		}
	//	}
	//}

	private void UpdateSource() {
		if (_itemsSource == null) return;

		IBinding? innerItemBinding = null;
		IBinding? isExpandedBinding = null;

		if (_expanderColumn != null) {
			innerItemBinding = _expanderColumn.InnerItemsSource;
			isExpandedBinding = _expanderColumn.IsExpanded;
		}

		ItemRoot = new(_itemsSource, innerItemBinding, isExpandedBinding);
	}

	public void SelectionChangedHandler(TreeDataView2Item item) {
		List<TreeDataView2Item> removedItems = new();
		List<TreeDataView2Item> addedItems = new();

		if (SelectedItems.Contains(item)) {
			SelectedItems.Remove(item);
		}

		if (SelectionMode.HasFlag(SelectionMode.Single)) {
			foreach (var it in SelectedItems) {
				removedItems.Add(it);
				it.IsSelected = false;
			}
		}

		if (item.IsSelected) {
			addedItems.Add(item);
		}

		SelectedItems.Add(item);
		SelectedItems = new(SelectedItems.Where(it => it.IsSelected));
		SelectedItem = SelectedItems.FirstOrDefault();

		SelectionChangedEventArgs e = new(TreeDataView.SelectionChangedEvent, removedItems, addedItems);
		RaiseEvent(e);
	}
}