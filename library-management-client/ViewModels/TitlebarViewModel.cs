using System;
using Avalonia_DependencyInjection.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class TitlebarViewModel : ViewModelBase
{
    private readonly MainWindow _mainWindow;
    [ObservableProperty] private string _iconPathExit = "/Assets/SVGs/xmark.svg";
    [ObservableProperty] private string _iconPathMaximize = "/Assets/SVGs/window-maximize.svg";
    [ObservableProperty] private string _iconPathMinimize = "/Assets/SVGs/window-minimize.svg";
    [ObservableProperty] private bool _isMaximized = false;

    public TitlebarViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    [RelayCommand]
    void CloseApplication()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    void MaximizeApplcation()
    {
        if (!IsMaximized)
        {
            _mainWindow.WindowState = WindowState.Maximized;
            IsMaximized = true;
        }
        else
        {
            _mainWindow.WindowState = WindowState.Normal;
            IsMaximized = false;
        }
    }

    [RelayCommand]
    void MinimizeApplication()
    {
        _mainWindow.WindowState = WindowState.Minimized;
    }
}