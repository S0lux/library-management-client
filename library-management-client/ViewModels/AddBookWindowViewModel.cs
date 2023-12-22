using System;
using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddBookWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _iconPathExit="/Assets/SVGs/xmark-royalblue.svg";

    [ObservableProperty] private ObservableCollection<string> _addByOptions = new ObservableCollection<string>()
        { "ISBN", "Titile", "Manual adding" };

    [ObservableProperty] private string _addBy;

    [ObservableProperty] private ViewModelBase _currentAddView;
    public AddBookWindowViewModel()
    {

    }

    [RelayCommand]
    void Test()
    {
        Console.WriteLine("TEST");
    }

    [RelayCommand]
    void Close()
    {
        var temp = App.AppHost.Services.GetRequiredService<AddBookWindow>();
        temp.Hide();
    }

}