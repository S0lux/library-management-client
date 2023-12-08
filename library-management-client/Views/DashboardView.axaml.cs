using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void TitleBarContainer_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var _mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
        _mainWindow.BeginMoveDrag(e);
    }
}