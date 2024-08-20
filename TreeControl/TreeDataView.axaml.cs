using AS_Desktop2.Misc;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView : UserControl {
	#region private
	#endregion

	#region public
	public static string PART_GridHeaderName = nameof(PART_GridHeader);

	public static DirectProperty<TreeDataView, IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView, IEnumerable?>(nameof(ItemsSource), x => x.ItemsSource, (x, value) => x.ItemsSource = value);

	public static StyledProperty<SelectionMode> SelectionModeProperty =
		AvaloniaProperty.Register<TreeDataView, SelectionMode>(nameof(SelectionMode), SelectionMode.Single);

	public static StyledProperty<TreeDataViewItem?> SelectedItemProperty =
		AvaloniaProperty.Register<TreeDataView, TreeDataViewItem?>(nameof(SelectedItem));

	public static StyledProperty<List<TreeDataViewItem>> SelectedItemsProperty =
		AvaloniaProperty.Register<TreeDataView, List<TreeDataViewItem>>(nameof(SelectedItems), new());

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<TreeDataView, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	public IEnumerable? ItemsSource {
		get => ItemsPresenter.ItemsSource;
		set => ItemsPresenter.ItemsSource = value;
	}

	public SelectionMode SelectionMode {
		get => GetValue(SelectionModeProperty);
		set => SetValue(SelectionModeProperty, value);
	}

	public event EventHandler<SelectionChangedEventArgs> SelectionChanged {
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	public TreeDataViewItem? SelectedItem {
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public List<TreeDataViewItem> SelectedItems {
		get => GetValue(SelectedItemsProperty);
		set => SetValue(SelectedItemsProperty, value);
	}

	public ObservableCollection<TreeDataViewColumn> Columns { get; } = new();
	#endregion

	public TreeDataView() {
        InitializeComponent();
		Columns.CollectionChanged += Columns_CollectionChanged;
		KeyDown += TreeDataView_KeyDown;
	}

	private void TreeDataView_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e) {
		//if (SelectedItem == null) return;

		//TreeDataViewItemsPresenter? itemsPresenter = SelectedItem.GetLogicalParentOfType<TreeDataViewItemsPresenter>();
		//if (itemsPresenter == null) return;

		//var itemsControl = itemsPresenter.ItemsControl;
	}

	private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
		switch (e.Action) {
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				if (e.NewItems == null) 
					break;
				PART_GridHeader.Children.Add(e.NewItems.OfType<TreeDataViewColumn>());
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if (e.OldItems == null)
					break;
				PART_GridHeader.Children.Remove(e.OldItems.OfType<TreeDataViewColumn>());
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				PART_GridHeader.Children.Clear();
				break;
			// TODO: остальные действия над коллекцией
			default:
				break;
		}
	}

	public void SelectionChangedHandler(TreeDataViewItem item) {
		List<TreeDataViewItem> removedItems = new();
		List<TreeDataViewItem> addedItems = new();

		if (SelectedItems.Contains(item)) {
			SelectedItems.Remove(item);
		}

		if (SelectionMode.HasFlag(SelectionMode.Toggle)) {
			item.IsSelected ^= true;
		}
		else {
			item.IsSelected = true;
		}

		if (SelectionMode.HasFlag(SelectionMode.AlwaysSelected)) {
			item.IsSelected = true;
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
		SelectedItems = SelectedItems.Where(it => it.IsSelected).ToList();
		SelectedItem = SelectedItems.FirstOrDefault();

		SelectionChangedEventArgs e = new(TreeDataView.SelectionChangedEvent, removedItems, addedItems);
		RaiseEvent(e);
	}
}
