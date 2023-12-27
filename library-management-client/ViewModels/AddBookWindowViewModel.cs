using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddBookWindowViewModel : ViewModelBase
{
    private readonly AuthenticationService _authService;
    private readonly AddByISBNViewModel _addByIsbnViewModel;
    private readonly AddByTitleViewModel _addByTitleViewModel;
    private readonly ManualAddingViewModel _manualAddingViewModel;
    [ObservableProperty] private string _iconPathExit="/Assets/SVGs/xmark-royalblue.svg";

    [ObservableProperty] private ObservableCollection<string> _addByOptions = new ObservableCollection<string>()
        { "ISBN", "Title", "Manual" };

    [ObservableProperty] private string _addBy;
    [ObservableProperty] private bool _isEnable = true;
    [ObservableProperty] private string _addByWaterMark;
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(FindCommand))] private string _findKey;
    [ObservableProperty] private bool _isBusy=false;
    [ObservableProperty] private bool _isLoaded=false;

    [ObservableProperty] private ViewModelBase _currentAddView;

    public AddBookWindowViewModel(
        AuthenticationService authenticationService,
        AddByISBNViewModel addByIsbnViewModel, 
        AddByTitleViewModel addByTitleViewModel,
        ManualAddingViewModel manualAddingViewModel)
    {
        _authService = authenticationService;
        _addByIsbnViewModel = addByIsbnViewModel;
        _addByTitleViewModel = addByTitleViewModel;
        _manualAddingViewModel = manualAddingViewModel;
        AddBy = "ISBN";
    }
    
    partial void OnAddByChanged(string? oldValue, string newValue)
    {
        switch (newValue)
        {
            case "ISBN":
                CurrentAddView = _addByIsbnViewModel;
                AddByWaterMark = "ISBN number";
                IsEnable = true;
                break;
            case "Title":
                CurrentAddView = _addByTitleViewModel;
                AddByWaterMark = "Book title";
                IsEnable = true;
                break;
            case "Manual":
                CurrentAddView = _manualAddingViewModel;
                AddByWaterMark = "";
                IsEnable = false;
                break;
                
        }
    }

    [RelayCommand(CanExecute = nameof(CheckFind))]
    public async Task Find()
    {
        Console.WriteLine("Executed");
        IsLoaded = false;
        IsBusy = true;
        if (AddBy == "ISBN")
        {
            var isSuccess = await _addByIsbnViewModel.RetrieveBookByISBN(FindKey);
            if (isSuccess == 1) IsLoaded = true;
        }

        if (AddBy == "Title")
        {
            await _addByTitleViewModel.RetrieveBooksByTitle(FindKey);
            IsLoaded = true;
        }

        IsBusy = false;
    }

    [RelayCommand]
    void Close()
    {
        var temp = App.AppHost!.Services.GetRequiredService<AddBookWindow>();
        temp.Close();
    }

    private bool CheckFind()
    {
        return !string.IsNullOrEmpty(FindKey);
    }

}

