using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia_DependencyInjection.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentViewModel=new();

    public DashboardViewModel()
    {
        
    }

    [RelayCommand]
    void toRegistryView()
    {
        CurrentViewModel = new MemberRegistryViewModel();
    }

    [RelayCommand]
    void toMemberListView()
    {
        CurrentViewModel = new MemberListViewModel();
    }
}
