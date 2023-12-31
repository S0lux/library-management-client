using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class InvoiceWindowViewModel: ViewModelBase
{
    [ObservableProperty] private CustomInvoice _invoice;
    [ObservableProperty] private ObservableCollection<BORROW_DETAIL> _borrowDetails = new ObservableCollection<BORROW_DETAIL>();
    [ObservableProperty] private bool _isLoading;
    
    private readonly AuthenticationService _authService;

    public InvoiceWindowViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }

    partial void OnInvoiceChanged(CustomInvoice value)
    {
        PopulateDataGrid();
    }

    private async Task PopulateDataGrid()
    {
        IsLoading = true;
        List<Task> fetchTasks = new List<Task>();
        
        foreach (var book in Invoice.BorrowDetails)
        {
            var fetchTask = Task.Run(async () =>
            {
                book.BookTitle = await FetchBookTitle(book.ISBN13);
                BorrowDetails = new ObservableCollection<BORROW_DETAIL>(Invoice.BorrowDetails);
            });
            
            fetchTasks.Add(fetchTask);
        }

        await Task.WhenAll(fetchTasks);

        foreach (var detail in BorrowDetails)
        {
            detail.ErrorsChanged += (sender, args) => SaveBookDetailsCommand.NotifyCanExecuteChanged();
        }
        
        IsLoading = false;
    }

    public bool IsSaveAble()
    {
        if (BorrowDetails.Any(detail => detail.HasErrors)) return false;
        return true;
    }

    [RelayCommand(CanExecute = nameof(IsSaveAble))]
    private async Task SaveBookDetails()
    {
        IsLoading = true;
        List<Task> tasks = new List<Task>();
        
        // Update borrow details
        foreach (var borrowDetail in Invoice.BorrowDetails)
        {
            var payloadObj = new
            {
                InvoiceID = borrowDetail.BorrowInvoiceID,
                ISBN13 = borrowDetail.ISBN13,
                Quantity = borrowDetail.Quantity,
                Returned = borrowDetail.Returned,
                Damaged = borrowDetail.Damaged,
                Lost = borrowDetail.Lost,
            };

            var payloadJson = JsonConvert.SerializeObject(payloadObj);
            var payloadContent = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            var task = Task.Run(async () =>
            {
                await _authService.PutAsync("/api/borrow_details", payloadContent);
            });
            
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        var res = await _authService.GetAsync($"/api/books/invoices/{Invoice.BorrowInvoiceID}");
        if (res.StatusCode == HttpStatusCode.OK)
        {
            var body = await res.Content.ReadAsStringAsync();
            var newInvoice = JsonConvert.DeserializeObject<CustomInvoice>(body);
            Invoice = newInvoice;

            var borrowVM = App.AppHost!.Services.GetRequiredService<BorrowViewModel>();
            await borrowVM.RetrieveInvoices();
        }
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