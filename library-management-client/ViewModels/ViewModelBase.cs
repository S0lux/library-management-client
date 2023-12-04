using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [RelayCommand]
    void CloseWindow()
    {
        Environment.Exit(0);
    }
}