using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentViewModel = new();
    [ObservableProperty] private SidebarViewModel _sidebarViewModel;
    [ObservableProperty] private ViewModelBase _activeViewModel;
    [ObservableProperty] private TitlebarViewModel _titleBarViewModel;

    public DashboardViewModel(SidebarViewModel sidebarViewModel, TitlebarViewModel titlebarViewModel, HomeViewModel homeViewModel)
    {
        SidebarViewModel = sidebarViewModel;
        TitleBarViewModel = titlebarViewModel;
        ActiveViewModel = homeViewModel;
    }
}


