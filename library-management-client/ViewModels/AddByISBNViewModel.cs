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
    [ObservableProperty] private string _releaseDate;

    public AddByISBNViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    partial void OnBookChanged(BOOK? oldValue, BOOK newValue)
    {
        if (Book != null)
        {
            ReleaseDate = Book.PublishDate.ToString("dd/MM/yyyy");
        }
    }

    [RelayCommand]
    async Task AddBook()
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
        
        
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            loadspiner.IsBusy = false;
            MyMessageBox error = new MyMessageBox("Bad connection", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            loadspiner.IsBusy = false;
            MyMessageBox error = new MyMessageBox("The book is already exists", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
        }
        else
        {
            loadspiner.IsBusy = false;
            MyMessageBox success = new MyMessageBox("The book is added", "Success",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information);
            await success.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
        }

    }
}