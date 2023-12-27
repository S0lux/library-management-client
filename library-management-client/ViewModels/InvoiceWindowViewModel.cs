using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class InvoiceWindowViewModel: ViewModelBase
{
    [ObservableProperty]
    private CustomInvoice _invoice;

    [ObservableProperty] private ObservableCollection<BORROW_DETAIL> _borrowDetails = new ObservableCollection<BORROW_DETAIL>();
    
    private readonly AuthenticationService _authService;

    public InvoiceWindowViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }

    partial void OnInvoiceChanged(CustomInvoice value)
    {
        foreach (var book in Invoice.BorrowDetails)
        {
            Task.Run(async () =>
            {
                book.BookTitle = await FetchBookTitle(book.ISBN13);
                BorrowDetails = new ObservableCollection<BORROW_DETAIL>(value.BorrowDetails);
            });
        }
    }

    partial void OnBorrowDetailsChanged(ObservableCollection<BORROW_DETAIL> value)
    {
        Console.WriteLine("Changed!");
    }

    private async Task<string> FetchBookTitle(string isbn)
    {
        var res = await _authService.GetAsync($"/api/books/isbn/{isbn}");

        if (res.StatusCode != HttpStatusCode.OK)
        {
            var messageBox = new MyMessageBox($"Unable to fetch title for: {isbn}", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            messageBox.Show();
            
            return "Unknown";
        }
        
        var stringContent = await res.Content.ReadAsStringAsync();
        var deserializedObject = JsonConvert.DeserializeObject<ApiRespondedBook>(stringContent);

        Console.WriteLine(deserializedObject.Data.Title);

        return deserializedObject.Data.Title;
    }
}