using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
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