using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BookViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;
    [ObservableProperty] private bool _isBusy = false;
    [ObservableProperty] private ObservableCollection<BOOK> _bookList = new();
    public BookViewModel( AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        GetData();
    }

    [RelayCommand]
    void Assign()
    {
        Console.WriteLine("Assigned");
    }

    [RelayCommand]
    void AddBook()
    {
        var temp = App.AppHost.Services.GetRequiredService<AddBookWindow>();
        temp.Show();
    }
    public async void GetData()
    {
        IsBusy = true;
        BookList.Clear();
        try
        {
            var response = await _authenticationService.GetAsync(@"/api/books");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiResponseBook = JsonConvert.DeserializeObject<ApiResponseBookList>(body);
            BookList = apiResponseBook.Data;
        }
        catch (HttpRequestException e)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", "Add Member Failed!",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();
        }
        IsBusy = false;
    }
}

public class ApiResponseBookList
{
    public ObservableCollection<BOOK> Data { get; set; }
}