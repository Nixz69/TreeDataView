using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Shared.Nodes;
using SharedLib;
using System.Linq;

namespace AS_Desktop2.UserControls;

public partial class TreeDataView2Test : UserControl {
	private NodeTreeViewModel root;

	public TreeDataView2Test() {
		InitializeComponent();

		root = new NodeTreeViewModel(StartupContainer.RootCursor);

		Load();
	}

	private async void Load() {
		await root.GetItems();
		TreeDataView2Control.ItemsSource = root.Items;

		//NodeCursor orgs = await NodeCursor.ByPath("node://62844F41-D819-408D-A0B1-65070BD54BEB|DEE0BCCE-FE46-4836-919F-9D9841394CB9|3668BA66-690E-42A0-B807-CB526B1DD78D|E580A541-6C34-4DE9-BDFC-58C34D7FEA91|F0151012-A9B8-4065-BEED-EEF25EA95D4E|6D3539F0-5CDA-4295-8CEC-8E8D4D0A6C7A|05F16D3539F05CDA42958CEC8E8D4D0A6C7A|61E7515FC196080F689E8AB1281B2F9F|2EE6FFB6-3407-416A-84F2-6ECB63DCDD71|112|VIEW|V$GC_ORGS");
		//var ie = await orgs.DirectQuery("select * from Personal.v$gc_orgs").ToListAsync();
		//TreeDataView2Control.ItemsSource = ie;
	}
}