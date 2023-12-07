using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class SidebarScreenViewModel: ViewModelBase
{
    [ObservableProperty]
    private bool _isExpanded;
    
    [ObservableProperty] private string _displayName;
    [ObservableProperty] private string? _iconPath;
    public ObservableCollection<SidebarScreenViewModel> Screens { get; }
    public Type? ViewModel;

    public SidebarScreenViewModel(string displayName, Type? type = null, string? iconPath = null, ObservableCollection<SidebarScreenViewModel>? screens = null)
    {
        ViewModel = type;
        DisplayName = displayName;
        Screens = screens ?? new ObservableCollection<SidebarScreenViewModel>();
        IconPath = iconPath;
    }
}