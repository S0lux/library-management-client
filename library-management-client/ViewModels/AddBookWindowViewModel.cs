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

    partial void OnAddByChanged(string? oldValue, string newValue)
    {
        switch (newValue)
        {
            case "ISBN":
                CurrentAddView = App.AppHost.Services.GetRequiredService<AddByISBNViewModel>();
                break;
            case "Titile":
                CurrentAddView = App.AppHost.Services.GetRequiredService<AddByTitleViewModel>();
                break;
            case "Manual adding":
                CurrentAddView = App.AppHost.Services.GetRequiredService<ManualAddingViewModel>();
                break;
                
        }
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