using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class BookView : UserControl
{
    private DataGrid _dataGrid;

    public BookView()
    {
        InitializeComponent();
    }
}