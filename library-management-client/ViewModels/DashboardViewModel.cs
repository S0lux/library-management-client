using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentViewModel = new();
    [ObservableProperty] private SidebarViewModel _sidebarViewModel;
    [ObservableProperty] private ObservableObject _activeViewModel;
    [ObservableProperty] private TitlebarViewModel _titleBarViewModel;
    [ObservableProperty] private bool _isTransitioning;

    public DashboardViewModel(SidebarViewModel sidebarViewModel, TitlebarViewModel titlebarViewModel, HomeViewModel homeViewModel)
    {
        SidebarViewModel = sidebarViewModel;
        TitleBarViewModel = titlebarViewModel;
        ActiveViewModel = homeViewModel;
    }

    partial void OnActiveViewModelChanged(ObservableObject value)
    {
        PlayTransitionAnimation();
    }

    private async Task PlayTransitionAnimation()
    {
        IsTransitioning = true;
        await Task.Delay(TimeSpan.FromSeconds(0.5));
        IsTransitioning = false;
    }
}


