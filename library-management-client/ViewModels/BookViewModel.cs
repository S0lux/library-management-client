using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using Tmds.DBus.Protocol;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BookViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;

    [ObservableProperty] private bool _isBusy = false;

    [ObservableProperty] private ObservableCollection<BOOK> _bookList = new();
    [ObservableProperty] private ObservableCollection<BOOK> _bookFindList = new ObservableCollection<BOOK>();
    [ObservableProperty] private ObservableCollection<BOOK> _showingList;
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(InfoCommand),nameof(CheckOutCommand))] 
    private int _checkedAmount=0;
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

    [RelayCommand]
    async Task Info()
    {
        var infoView = App.AppHost.Services.GetRequiredService<BookInfoView>();
        var infoVM = App.AppHost.Services.GetRequiredService<BookInfoViewModel>();
        infoVM.Book = SelectedBOOK;
        infoVM.ImageUrl = $"https://covers.openlibrary.org/b/isbn/{infoVM.Book.ISBN13}-L.jpg?default=false";
        await infoVM.GetData();
        infoView.Show();
    }

    [RelayCommand(CanExecute = nameof(checkCheckOut))]
    async Task CheckOut()
    {
        try
        {
            var temp1 = App.AppHost.Services.GetRequiredService<BorrowRegisterFormViewModel>();
            temp1.Load();

            var temp2 = App.AppHost.Services.GetRequiredService<BorrowRegisterFormView>();
            temp2.Show();
        }
        catch (Exception e)
        {
            MyMessageBox error = new MyMessageBox(
                "One of the chosen book does not have the required quantity.\nPlease update the book information.",
                "Attempt to check out failed!",
                MyMessageBox.MessageBoxButton.OK,
                MyMessageBox.MessageBoxImage.Error,400,200
            );

            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());
        }
    }

    [RelayCommand]
    async void Delete()
    {
        
        await TrueDelete();

        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            var box = App.AppHost!.Services.GetRequiredService<BookViewModel>();
            box.GetData();
        }
        
    }

    async public Task TrueDelete()
    {
        MyMessageBox khang = new MyMessageBox(
            "Are you sure you want to delete the book?",
            "Confirm",
            MyMessageBox.MessageBoxButton.YesNo,
            MyMessageBox.MessageBoxImage.Question
            );

        await khang.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());

        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            var response = await _authenticationService.DeleteAsync($"/api/books/isbn/{SelectedBOOK.ISBN13}");
        }
    }

    [RelayCommand]
    public async Task GetData()
    {
        IsBusy = true;
        BookList.Clear();
        CheckedAmount = 0;
        try
        {
            var response = await _authenticationService.GetAsync(@"/api/books");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", "An unexpected error has occured.");
                await box.ShowAsync();
                
                IsBusy = false;
                return;
            }
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiResponseBook = JsonConvert.DeserializeObject<ApiResponseBookList>(body);
            BookList = apiResponseBook.Data;
            ShowingList = BookList;

            response = await _authenticationService.GetAsync(@"/api/book_details");
            response.EnsureSuccessStatusCode();

            body = await response.Content.ReadAsStringAsync();
            var apiResponseMember = JsonConvert.DeserializeObject<ApiResBookDetail>(body);

            var retrievedBookDetail = new ObservableCollection<BOOK_DETAIL>(apiResponseMember!.data);
            
            foreach (BOOK bOOK in BookList)
            {
                foreach (BOOK_DETAIL bt in retrievedBookDetail)
                {
                    if (bt.ISBN13 == bOOK.ISBN13)
                    {
                        bOOK.BOOK_DETAILs.Add(bt);
                    }
                }
            }


        }
        catch (HttpRequestException e)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", "An unexpected error has occured.");

            var result = await box.ShowAsync();
        }
        IsBusy = false;
    }

    public bool checkCheckOut()
    {
        return (CheckedAmount > 0);
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