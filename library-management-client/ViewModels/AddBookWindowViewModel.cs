using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
    [ObservableProperty] private bool _isEnable=true;
    [ObservableProperty] private string _addByWaterMark;
    [ObservableProperty] private string _findKey;
    [ObservableProperty] private BOOK _foundedBook;

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
                AddByWaterMark = "Book Title";
                IsEnable = true;
                break;
            case "Manual":
                CurrentAddView = _manualAddingViewModel;
                AddByWaterMark = "";
                IsEnable = false;
                break;
                
        }
    }

    [RelayCommand]
    public async Task Find()
    {
        if (AddBy == "ISBN")
        {
            var response = await _authService.GetAsync($@"/api/books/isbn/{FindKey}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiRespondedBook = JsonConvert.DeserializeObject<ApiRespondedBook>(body);
            FoundedBook = apiRespondedBook.Data;
            _addByIsbnViewModel.Book = FoundedBook;
        }
        
        Console.WriteLine(FoundedBook.Title);
    }

    [RelayCommand]
    void Close()
    {
        var temp = App.AppHost!.Services.GetRequiredService<AddBookWindow>();
        temp.Hide();
    }

}

public class ApiRespondedBook
{
    public BOOK Data { get; set; }
}