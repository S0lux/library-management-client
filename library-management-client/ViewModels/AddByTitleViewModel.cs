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
    }

    [RelayCommand]
    private async Task RegisterBook(BOOK book)
    {
        // Confirm with the user that they want to add this book to the database
        var messageBoxContentString =  "Do you want to add this book?\n" +
                                       $"Title: {book.Title}\n" +
                                       $"ISBN: {book.ISBN13}\n" + 
                                       $"Author: {book.Author}";

        var messageBox = new MyMessageBox(messageBoxContentString, "Register Result", 
                                     MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Question);
        await messageBox.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());
        
        // If yes then...
        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            // Attempt to register book to database
            var postResult = await PostBookAsync(book);
            
            // Error handling
            var resultContentString = "Book successfully added to database";
            var resultBoxIcon = MyMessageBox.MessageBoxImage.Information;

            if (postResult.StatusCode == HttpStatusCode.Conflict)
            {
                resultContentString = "This book is already registered in the database.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
            }
            else if (postResult.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                resultContentString = "Unable to connect to database.\nPlease check your internet connection and try again.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
            }
            else if (postResult.StatusCode == HttpStatusCode.BadRequest)
            {
                resultContentString = "An unexpected error has occured.\nPlease report this issue to your IT department.";
                resultBoxIcon = MyMessageBox.MessageBoxImage.Error;
            }
            // If no error then refresh the DataGrid in BookView
            else RefreshBookViewModel();
            
            var resultBox = new MyMessageBox(resultContentString, "Result", 
                MyMessageBox.MessageBoxButton.OK, resultBoxIcon);
            resultBox.Show();
        }
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

    private void RefreshBookViewModel()
    {
        var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
        bookViewModel.GetData();
    }
}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}