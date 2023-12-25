using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private void RegisterBook(BOOK book)
    {
        var messageBoxContentString =  "Do you want to add this book?\n" +
                                       $"Title: {book.Title}\n" +
                                       $"ISBN: {book.ISBN13}\n" + 
                                       $"Author: {book.Author}";

        var myBox = new MyMessageBox(messageBoxContentString, "Confirmation", 
                                     MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Question);
        myBox.Show();
    }
}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}