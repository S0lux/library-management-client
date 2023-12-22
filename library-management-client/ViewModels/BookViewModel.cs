using System;
using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BookViewModel:ViewModelBase
{
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private ObservableCollection<BOOK> _bookList=new();
    public BookViewModel()
    {
        BookList.Add(new BOOK(){ISBN10 = "1234", Title = "THE FELLOWSHIP OF THE RING", Author = "J.R.R Tolkien"});
        BookList.Add(new BOOK(){ISBN10 = "4321", Title = "THE TWO TOWERS", Author = "J.R.R Tolkien"});
        BookList.Add(new BOOK(){ISBN10 = "3467", Title = "THE RETURN OF THE KING", Author = "J.R.R Tolkien"});

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
    
    
}