using System;
using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryFormViewModel:ViewModelBase
{
    [ObservableProperty] private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0};
    [ObservableProperty]
    private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };

    [ObservableProperty] private Window _win;
    public MemberRegistryFormViewModel(Window window)
    {
        Win = window;
    }
    [RelayCommand]
    void Submit()
    {
        Win.Close();
    }

    [RelayCommand]
    void Cancel()
    {
        Win.Close();
    }
}