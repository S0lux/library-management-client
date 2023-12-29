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
    [ObservableProperty] private BOOK_DETAIL? _bookDetail = new();
    [ObservableProperty] private string _releaseDate;
    [ObservableProperty] private string _imageUrl;
    [ObservableProperty] private bool _isCoverLoading;

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

    [RelayCommand(CanExecute = nameof(CheckAdd))]
    async Task AddBook()
    {
        MyMessageBox confirmation = new MyMessageBox(
            "Are you sure about adding this book", "Confirmation",
            MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Question,350,150);
        await confirmation.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());

        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            var loadspiner = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
            loadspiner.IsBusy = true;
            //Attempt to register book by isbn 
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

            //Error handling for book register
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                loadspiner.IsBusy = false;
                MyMessageBox error = new MyMessageBox("Bad connection", "Error",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error,350,150);
                await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                return;
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                loadspiner.IsBusy = false;
                MyMessageBox error = new MyMessageBox("The book is already exists", "Error",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error,350, 150);
                await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                return;
            }
            else
            {
                loadspiner.IsBusy = false;

                //Attempt to register book detail
                var createdBookDetail = new
                {
                    ISBN13 = Book.ISBN13,
                    Status = "normal",
                    Quantity = BookDetail.Quantity
                };
                var payload1 = new
                {
                    data = createdBookDetail
                };
                var json1 = JsonConvert.SerializeObject(payload1);
                var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

                var response1 = await _authenticationService.PutAsync("/api/book_details", content1);

                //Error handling for book detail register
                if (response1.StatusCode == HttpStatusCode.BadRequest)
                {
                    MyMessageBox falied = new MyMessageBox(
                        "An unexpected error has occured.\nPlease report this issue to your IT department.", "Failed",
                        MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
                    await falied.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                    return;
                }

                MyMessageBox success = new MyMessageBox("The book is added", "Success",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information,350,150);
                await success.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
                var revaluate = App.AppHost.Services.GetRequiredService<BookViewModel>();
                revaluate.GetData();
            }

            var box = App.AppHost!.Services.GetRequiredService<BookViewModel>();
            box.BookCheckedList.Clear();
            box.CheckedAmount = 0;
            box.GetData();
        }
    }

    public async Task<int> RetrieveBookByISBN(string isbn)
    {
        var addbookwin = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
        try
        {
            var response = await _authenticationService.GetAsync($@"/api/books/isbn/{isbn}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiRespondedBook = JsonConvert.DeserializeObject<ApiRespondedBook>(body);
            Book = apiRespondedBook.Data;
            return 1;
        }
        catch (HttpRequestException e)
        {
            MyMessageBox error = new MyMessageBox("Unable to find the book", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
            return 0;
        }
    }

    public bool CheckAdd()
    {
        return (BookDetail.Quantity != 0) && (BookDetail.Quantity != null);
    }
}

public class ApiRespondedBook
{
    public BOOK Data { get; set; }
}