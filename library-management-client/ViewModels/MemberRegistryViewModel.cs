using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryViewModel : ViewModelBase
{
    [ObservableProperty] private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0};
    [ObservableProperty]
    private ObservableCollection<string> _genders = new ObservableCollection<string>()
    { "Male", "Female" };

    [RelayCommand]
    void Submit()
    {
        Console.WriteLine(InputedMember.Gender);
    }
}