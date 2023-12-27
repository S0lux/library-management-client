using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia_DependencyInjection.Controls;
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

    [ObservableProperty] private int _selectedNumber = 0;

    [ObservableProperty] private ObservableCollection<BOOK> _bookList = new();
    [ObservableProperty] private ObservableCollection<BOOK> _bookFindList = new ObservableCollection<BOOK>();
    [ObservableProperty] private ObservableCollection<BOOK> _showingList;
    [ObservableProperty] private int _checkedAmount=0;
    [ObservableProperty] private string? _filterKey;


    
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(CheckOutCommand))] private ObservableCollection<BOOK> _bookCheckedList = new();

    [ObservableProperty] private BOOK selectedBOOK;

    public BookViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        GetData();
        BookCheckedList.CollectionChanged += (sender, args) => { CheckOutCommand.NotifyCanExecuteChanged(); };
    }

    [RelayCommand]
    void Assign()
    {
        var temp = App.AppHost!.Services.GetService<BorrowRegisterFormView>();
        temp.Show();

        Console.WriteLine("Assigned");
    }

    [RelayCommand]
    void AddBook()
    {
        var temp = App.AppHost.Services.GetRequiredService<AddBookWindow>();
        temp.Show();
    }

    [RelayCommand(CanExecute = nameof(checkCheckOut))]
    void CheckOut()
    {
        var temp1 = App.AppHost.Services.GetRequiredService<BorrowRegisterFormViewModel>();
        temp1.Load();

        var temp2 = App.AppHost.Services.GetRequiredService<BorrowRegisterFormView>();
        temp2.Show();
    }


    public async Task GetData()
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
            ShowingList = BookList;
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

    public bool checkCheckOut()
    {
        if(BookCheckedList.Count > 0)
        {
            return true;
        }
        return false;
    }

    [RelayCommand]
    public void BookChecked()
    {
        
        if (SelectedBOOK.IsCheck == true)
        {
            BookCheckedList.Add(SelectedBOOK);
            CheckedAmount++;
        }
        else
        {
            var temp1 = App.AppHost.Services.GetRequiredService<BorrowRegisterFormViewModel>();
            temp1.BorrowDetailList.Remove(temp1.BorrowDetailList.FirstOrDefault(e => e.ISBN13 == SelectedBOOK.ISBN13));

            BookCheckedList.Remove(SelectedBOOK);
            CheckedAmount--;
        }
    }

    partial void OnFilterKeyChanged(string? oldValue, string? newValue)
    {
        BookFindList.Clear();
        IsBusy = true;
        foreach (BOOK book in BookList)
        {
            if (book.ISBN13.Contains(newValue) || book.Title.ToLower().Contains(newValue.ToLower()))
            {
                BookFindList.Add(book);
            }
        }

        ShowingList = BookFindList;
        IsBusy = false;
    }
}

public class ApiResponseBookList
{
    public ObservableCollection<BOOK> Data { get; set; }
}