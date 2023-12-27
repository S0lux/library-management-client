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
                case HttpStatusCode.InternalServerError:
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
        bool HasError = false;
        var loadspiner = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
        // Confirm with the user that they want to add this book to the database
        string messageBoxContentString =  "How many of this book do you want to add?\n" +
                                   $"Title: {book.Title}\n" +
                                   $"ISBN: {book.ISBN13}\n" + 
                                   $"Author: {book.Author}";
        var mess = new QuantityConfirmMessageBox(
            messageBoxContentString,
            "Confirm",
            QuantityConfirmMessageBox.MessageBoxImage.Question
            ,400,250);
        await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
        
        // If OK then...
        if (QuantityConfirmMessageBox.buttonResultClicked == QuantityConfirmMessageBox.ButtonResult.OK)
        {
            loadspiner.IsBusy = true;
            // Attempt to register book to database
            var postResult = await PostBookAsync(book);
            
            // Error handling for book registering
            var resultContentString = "Book successfully added to database";
            var resultBoxIcon = MyMessageBox.MessageBoxImage.Information;

            if (postResult.StatusCode == HttpStatusCode.Conflict)
            {
                resultContentString = "This book is already registered in the database.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
                HasError = true;
            }
            else if (postResult.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                resultContentString = "Unable to connect to database.\nPlease check your internet connection and try again.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
                HasError = true;
            }
            else if (postResult.StatusCode == HttpStatusCode.BadRequest)
            {
                resultContentString = "An unexpected error has occured.\nPlease report this issue to your IT department.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
                HasError = true;
            }
            // If no error then refresh the DataGrid in BookView
            else RefreshBookViewModel();
            if (HasError)
            {
                var resultBox = new MyMessageBox(resultContentString, "Result", 
                    MyMessageBox.MessageBoxButton.OK, resultBoxIcon);
                resultBox.Show();
                return;
            }
            
            // Attempt to register book detail to database
            var createdBookDetail = new
            {
                ISBN13 = book.ISBN13,
                Status = "normal",
                Quantity = BookQuantity
            };
            var payload1 = new
            {
                data = createdBookDetail
            };
            var json1 = JsonConvert.SerializeObject(payload1);
            var content1 = new StringContent(json1, Encoding.UTF8, "application/json");

            var response1 = await _authService.PutAsync("/api/book_details", content1);
            
            //Error handling for registering book detail
            if (response1.StatusCode == HttpStatusCode.BadRequest)
            {
                resultContentString = "An unexpected error has occured.\nPlease report this issue to your IT department.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
            }
            
            var resultBox1 = new MyMessageBox(resultContentString, "Result", 
                MyMessageBox.MessageBoxButton.OK, resultBoxIcon);
            resultBox1.Show();
            
        }
        loadspiner.IsBusy = false;
        HasError = false;
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
        bookViewModel.GetData();
    }
}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}