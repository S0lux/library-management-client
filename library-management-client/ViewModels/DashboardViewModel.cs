using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentViewModel = new();
    [ObservableProperty] private SidebarViewModel _sidebarViewModel;
    [ObservableProperty] private ViewModelBase _activeViewModel;
    [ObservableProperty] private TitlebarViewModel _titleBarViewModel;

    public DashboardViewModel(SidebarViewModel sidebarViewModel, TitlebarViewModel titlebarViewModel)
    {
        SidebarViewModel = sidebarViewModel;
        TitleBarViewModel = titlebarViewModel;
    }
}


