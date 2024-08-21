using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS_Desktop2.UserControls;

public class TreeDataView2Cell : UserControl {
	#region private
	#endregion

	#region public
	public static StyledProperty<IDataTemplate?> CellContentTemplateProperty =
		AvaloniaProperty.Register<TreeDataView2Cell, IDataTemplate?>(nameof(CellContentTemplate));

	public IDataTemplate? CellContentTemplate {
		get => GetValue(CellContentTemplateProperty);
		set => SetValue(CellContentTemplateProperty, value);
	}
	#endregion

	public TreeDataView2Cell() {
		ClipToBounds = true;
	}
}
