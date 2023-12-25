using System;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Text;
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

public partial class AddByISBNViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;

    [ObservableProperty] private BOOK _book;
    [ObservableProperty] private BOOK_DETAIL _bookDetail=new();
    [ObservableProperty] private string _releaseDate;
    [ObservableProperty] private string _imageUrl;
    [ObservableProperty] private bool _isCoverLoading;
    [ObservableProperty] private string _buttonContent="Confirm";

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddBookCommand))] private int _clickStage=0;

    public AddByISBNViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        BookDetail.PropertyChanged += ((sender, args) => { AddBookCommand.NotifyCanExecuteChanged(); });
    }

    partial void OnBookChanged(BOOK? oldValue, BOOK newValue)
    {
        if (Book != null)
        {
            ReleaseDate = Book.PublishDate.ToString("dd/MM/yyyy"); 
            ImageUrl = $"https://covers.openlibrary.org/b/isbn/{Book.ISBN13}-L.jpg";
        }
    }

    [RelayCommand(CanExecute =nameof(CheckAdd) )]
    async Task AddBook()
    {
        if (ClickStage == 1)
        {
            var loadspiner = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
            loadspiner.IsBusy = true;
            var createdBook = new
            {
                Title = Book.Title,
                Author = Book.Author,
                PublishDate = Book.PublishDate,
                ISBN13 = Book.ISBN13,
            };
            var payload = new
            {
                data = createdBook
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _authenticationService.PostAsync("/api/books", content);

            Console.WriteLine(response.StatusCode);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                loadspiner.IsBusy = false;
                MyMessageBox error = new MyMessageBox("Bad connection", "Error",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                return;
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                loadspiner.IsBusy = false;
                MyMessageBox error = new MyMessageBox("The book is already exists", "Error",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
                await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                return;
            }
            else
            {
                loadspiner.IsBusy = false;
                MyMessageBox success = new MyMessageBox("The book is added", "Success",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                await success.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
            }
            ClickStage = 0;
            ButtonContent = "Confirm";
        }
        
        if (ClickStage == 0)
        {
            ClickStage++;
            ButtonContent = "Add now";
        }
    }

    public async Task RetrieveBookByISBN(string isbn)
    {
        var addbookwin = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
        try
        {
            var response = await _authenticationService.GetAsync($@"/api/books/isbn/{isbn}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiRespondedBook = JsonConvert.DeserializeObject<ApiRespondedBook>(body);
            Book = apiRespondedBook.Data;
        }
        catch(HttpRequestException e)
        {
            MyMessageBox error = new MyMessageBox("Unable to find the book", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
        }
    }

    public bool CheckAdd()
    {
        if (ClickStage == 0) return true;
        return BookDetail.Quantity!=0;
    }
}

public class ApiRespondedBook
{
    public BOOK Data { get; set; }
}