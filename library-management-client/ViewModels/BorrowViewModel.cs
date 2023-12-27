using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BorrowViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;

    [ObservableProperty] private string _selectedStatusFilter;
    [ObservableProperty] private ObservableCollection<CustomInvoice> _invoices;
    [ObservableProperty] private ObservableCollection<string> _statusFilters = new ObservableCollection<string>()
    {
        "Ongoing", "Overdue", "Completed"
    };

    public BorrowViewModel(AuthenticationService authService)
    {
        _authService = authService;
        SelectedStatusFilter = "Overdue";
        RetrieveInvoices();
    }

    public async Task RetrieveInvoices()
    {
        var retrieveRes = await _authService.GetAsync("/api/books/invoices");

        if (retrieveRes.StatusCode == HttpStatusCode.NotFound)
        {
            var messageBox = new MyMessageBox("No invoices found", "Empty list", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Warning);
            messageBox.Show();
            return;
        }
        else if (retrieveRes.StatusCode == HttpStatusCode.InternalServerError)
        {
            var messageBox = new MyMessageBox("An unexpected error has occured", "Error", MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Warning);
            messageBox.Show();
            return;
        }

        var stringContent = await retrieveRes.Content.ReadAsStringAsync();
        var deserializedObj = JsonConvert.DeserializeObject<RootObject>(stringContent);

        Invoices = deserializedObj.Data;
    }
}

public class CustomInvoice
{
    public int BorrowInvoiceID { get; set; }
    public DateTime BorrowingDate { get; set; }

    public List<BORROW_DETAIL> BorrowDetails { get; set; }
    public List<MEMBER> Member { get; set; }
    public List<EMPLOYEE> Employee { get; set; }

    private string _status;
    private SolidColorBrush _statusColor;

    public string Status
    {
        get
        {
            UpdateStatus();
            return _status;
        }
    }

    public SolidColorBrush StatusColor
    {
        get
        {
            UpdateStatusColor();
            return _statusColor;
        }
    }

    private void UpdateStatus()
    {
        DateTime today = DateTime.Today;

        if (BorrowDetails.Any(detail => detail.DueDate < today))
        {
            _status = "Overdue";
        }
        else if (BorrowDetails.Any(detail => detail.DueDate > today))
        {
            _status = "Ongoing";
        }
        else if (BorrowDetails.All(detail => detail.HasReturned))
        {
            _status = "Completed";
        }
        else
        {
            _status = "Unknown"; // Set a default status if needed
        }
    }

    private void UpdateStatusColor()
    {
        DateTime today = DateTime.Today;

        if (BorrowDetails.Any(detail => detail.DueDate < today))
        {
            _statusColor = new SolidColorBrush(Colors.Red);
        }
        else if (BorrowDetails.Any(detail => detail.DueDate > today))
        {
            _statusColor = new SolidColorBrush(Colors.Orange);
        }
        else if (BorrowDetails.All(detail => detail.HasReturned))
        {
            _statusColor = new SolidColorBrush(Colors.LimeGreen);
        }
        else
        {
            _statusColor = new SolidColorBrush(Colors.Black); // Set a default status if needed
        }
    }
}


public class RootObject
{
    public ObservableCollection<CustomInvoice> Data { get; set; }
}