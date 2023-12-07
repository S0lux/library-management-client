using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class DropDownMenuView : UserControl
{
    public DropDownMenuView(DropDownMenuViewModel dropDownMenuViewModel)
    {
        InitializeComponent();
        this.DataContext = dropDownMenuViewModel;
    }
}