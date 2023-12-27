using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddByTitleViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;
    
    [ObservableProperty] private ObservableCollection<BOOK>? _books;
    [ObservableProperty] private BOOK _selectedBook;
    [ObservableProperty] private string _imageUrl;
    [ObservableProperty] private int _clickStage;
    public int BookQuantity { get; set; }

    public AddByTitleViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }
    
    public async Task RetrieveBooksByTitle(string title)
    {
        var response = await _authService.GetAsync($"/api/books/title/{title}");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            // Notify user
            // 404 -> No book found
            // 500 -> Unknown error

            var messageBoxString = "";
            var messageBoxIcon = MyMessageBox.MessageBoxImage.Error;

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    messageBoxString = "No book with that title was found.";
                    break;
                default:
                    messageBoxString =
                        "An unexpected error has occured.\nPlease report this issue to your IT department.";
                    break;
            }
            
            var messageBox = new MyMessageBox(messageBoxString, "Failed to retrieve books", 
                MyMessageBox.MessageBoxButton.OK, messageBoxIcon);
            messageBox.Show();
            
            return;
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<apiResponse>(responseBody);

        if (responseObj != null) Books = responseObj.data;
    }

    // For updating the book cover preview
    partial void OnSelectedBookChanged(BOOK value)
    {
        ImageUrl = $"https://covers.openlibrary.org/b/isbn/{value.ISBN13}-L.jpg?default=false";
        ClickStage = 0;
    }


    [RelayCommand]
    private async Task RegisterBook(BOOK book)
    {
        var loadingSpinner = App.AppHost!.Services.GetRequiredService<AddBookWindowViewModel>();
        var confirmationResult = await ShowQuantityConfirmMessageBox(book);

        if (confirmationResult == QuantityConfirmMessageBox.ButtonResult.OK)
        {
            loadingSpinner.IsBusy = true;

            var postResult = await PostBookAsync(book);
            var resultContentString = GetResultContentString(postResult.StatusCode);
            var resultBoxIcon = GetResultBoxIcon(postResult.StatusCode);

            if (postResult.IsSuccessStatusCode)
            {
                await RefreshBookViewModel();
                await RegisterBookDetailAsync(book.ISBN13);
            }

            ShowResultMessageBox(resultContentString, resultBoxIcon);
        }

        loadingSpinner.IsBusy = false;
    }

    private async Task<QuantityConfirmMessageBox.ButtonResult> ShowQuantityConfirmMessageBox(BOOK book)
    {
        var messageBoxContentString = $"How many of this book do you want to add?\n" +
                                      $"Title: {book.Title}\n" +
                                      $"ISBN: {book.ISBN13}\n" +
                                      $"Author: {book.Author}";

        var mess = new QuantityConfirmMessageBox(
            messageBoxContentString,
            "Confirm",
            QuantityConfirmMessageBox.MessageBoxImage.Question,
            400, 250);

        await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());

        return QuantityConfirmMessageBox.buttonResultClicked;
    }

    private async Task RegisterBookDetailAsync(string isbn13)
    {
        var createdBookDetail = new
        {
            ISBN13 = isbn13,
            Status = "normal",
            Quantity = BookQuantity
        };
        
        var payload = new
        {
            data = createdBookDetail
        };
        
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _authService.PutAsync("/api/book_details", content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ShowResultMessageBox("An unexpected error has occurred.\nPlease report this issue to your IT department.",
                                 MyMessageBox.MessageBoxImage.Error);
        }
    }

    private string GetResultContentString(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Conflict => "This book is already registered in the database.",
            HttpStatusCode.ServiceUnavailable => "Unable to connect to the database.\nPlease check your internet connection and try again.",
            HttpStatusCode.BadRequest => "An unexpected error has occurred.\nPlease report this issue to your IT department.",
            _ => "Book successfully added to the database"
        };
    }

    private MyMessageBox.MessageBoxImage GetResultBoxIcon(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Conflict => MyMessageBox.MessageBoxImage.Error,
            HttpStatusCode.ServiceUnavailable => MyMessageBox.MessageBoxImage.Error,
            HttpStatusCode.BadRequest => MyMessageBox.MessageBoxImage.Error,
            _ => MyMessageBox.MessageBoxImage.Information
        };
    }

    private void ShowResultMessageBox(string contentString, MyMessageBox.MessageBoxImage boxIcon)
    {
        var resultBox = new MyMessageBox(contentString, "Result",
            MyMessageBox.MessageBoxButton.OK, boxIcon);
        resultBox.Show();
    }

    private Task<HttpResponseMessage> PostBookAsync(BOOK book)
    {
        var payload = new
        {
            data = book
        };

        var contentString = JsonConvert.SerializeObject(payload);
        var requestContent = new StringContent(contentString, Encoding.UTF8, "application/json");

        return _authService.PostAsync("/api/books", requestContent);
    }

    private async Task RefreshBookViewModel()
    {
        var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
        await bookViewModel.GetData();
    }

}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}