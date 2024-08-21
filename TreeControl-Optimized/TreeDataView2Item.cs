using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AS_Desktop2.UserControls;

public class TreeDataView2Item : StyledElement, INotifyPropertyChanged {
	#region private
	private TreeDataView2ItemRoot _root;

	private IBinding? _itemsSourceBinding;
	private IBinding? _isExpandedBinding;
	private IEnumerable? _itemsSource;
	private AvaloniaList<TreeDataView2Item> _items = new();
	// private int _level = 0;
	private bool _isExpanded = false;
	private bool _isSelected = false;
	#endregion

	#region public
	public new event PropertyChangedEventHandler? PropertyChanged;

	public static DirectProperty<TreeDataView2Item, IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Item, IEnumerable?>(nameof(ItemsSource), x => x.ItemsSource, (x, value) => x.ItemsSource = value);

	public static DirectProperty<TreeDataView2Item, bool> IsExpandedProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Item, bool>(nameof(IsExpanded), x => x.IsExpanded, (x, value) => x.IsExpanded = value, default, BindingMode.TwoWay);

	public static StyledProperty<int> LevelProperty =
		AvaloniaProperty.Register<TreeDataView2Item, int>(nameof(Level));

	public static DirectProperty<TreeDataView2Item, object?> ContextProperty =
		AvaloniaProperty.RegisterDirect<TreeDataView2Item, object?>("Context", x => x.DataContext, (x, value) => x.DataContext = value);

	public IBinding? ItemsSourceBinding {
		get => _itemsSourceBinding;
		set {
			_itemsSourceBinding = value;

			if (_itemsSourceBinding != null) {
				this[!ItemsSourceProperty] = _itemsSourceBinding;
			}
		}
	}

	public IBinding? IsExpandedBinding {
		get => _isExpandedBinding;
		set {
			_isExpandedBinding = value;

            if (_isExpandedBinding != null) {
				this[!IsExpandedProperty] = _isExpandedBinding;
			}
        }
	}

	public IEnumerable? ItemsSource {
		get => _itemsSource;
		set {
			_itemsSource = value;

			Items.Clear();

			if (_itemsSource != null) {
				foreach (var item in _itemsSource) {
					Items.Add(new(_root, item) {
						ItemsSourceBinding = ItemsSourceBinding,
						IsExpandedBinding = IsExpandedBinding,
						Level = Level + 1,
					});
				}
			}

			_root.UpdateView();
		}
	}

	public bool IsExpanded {
		get => _isExpanded;
		set {
			_isExpanded = value;
			//SetValue(IsExpandedProperty, value);

			_root.UpdateView();
		}
	}

	public bool IsSelected {
		get => _isSelected;
		set {
			_isSelected = value;

			OnPropertyChanged(nameof(IsSelected));
		}
	}

	public int Level {
		get => GetValue(LevelProperty);
		set => SetValue(LevelProperty, value);
	}

	public AvaloniaList<TreeDataView2Item> Items {
		get => _items;
		set {
			_items = value;
			OnPropertyChanged(nameof(Items));
		}
	}
	#endregion

	public TreeDataView2Item(TreeDataView2ItemRoot root, object? value) {
		_root = root;
		
		DataContext = value;
	}

	public void OnPropertyChanged([CallerMemberName] string prop = "") =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}

public class TreeDataView2ItemRoot : ReactiveObject {
	#region private
	private ObservableCollection<TreeDataView2Item> _itemsSource = new();
	private ObservableCollection<TreeDataView2Item> _itemsView = new();
	#endregion

	#region public
	public ObservableCollection<TreeDataView2Item> ItemsView {
		get => _itemsView;
		set => this.RaiseAndSetIfChanged(ref _itemsView, value);
	}
	#endregion

	public TreeDataView2ItemRoot(IEnumerable source, IBinding? innerItemsBinding, IBinding? isExpandedBinding) {
		foreach (var item in source) {
			_itemsSource.Add(new(this, item) {
				ItemsSourceBinding = innerItemsBinding,
				IsExpandedBinding = isExpandedBinding,
			});
		}

		UpdateView();
	}

	public void UpdateView() {
		List<TreeDataView2Item> items = new();
		
		foreach (var it in _itemsSource) {
			AppendItems(items, it);
		}

		ItemsView = new(items);
	}

	private void AppendItems(List<TreeDataView2Item> list, TreeDataView2Item node) {
		list.Add(node);
		if (node.IsExpanded)
			foreach (var ch in node.Items)
				AppendItems(list, ch);
	}
}
