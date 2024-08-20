using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using SharedLib;
using System.Threading.Tasks;
using Terminal.Gui;

namespace AS_Desktop2.UserControls;

public partial class TreeDataViewTest : UserControl {
	#region private
	private NodeTreeViewModel root;
	#endregion

	public TreeDataViewTest() {
        InitializeComponent();

		root = new NodeTreeViewModel(StartupContainer.RootCursor);

		Load();
	}

    private async void Load() {
        await root.GetItems();

        TreeDataViewControl.ItemsSource = root.Items;
    }

	private void MenuItem_PointerPressed(object? sender, PointerPressedEventArgs e) {
		if (e.Source is not Control it) return;
		NodeTreeViewModel? target = null;

		if (TreeDataViewControl.SelectedItem != null && TreeDataViewControl.SelectedItem.DataContext is NodeTreeViewModel nodeModel) {
			target = nodeModel;
		}
		else {
			target = root;
		}

		if (target != null) {
			target.OperationCommand(it.DataContext);
		}
	}
}