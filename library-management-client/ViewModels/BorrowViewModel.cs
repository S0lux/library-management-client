using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BorrowViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;

    [ObservableProperty] private string _selectedStatusFilter;
    [ObservableProperty] private string? _findKey;
    [ObservableProperty] private ObservableCollection<CustomInvoice> _invoices=new();
    [ObservableProperty] private ObservableCollection<CustomInvoice> _invoicesFindList=new();
    [ObservableProperty] private ObservableCollection<CustomInvoice> _showingList=new();

    [ObservableProperty] private ObservableCollection<string> _statusFilters = new ObservableCollection<string>()
    {
        "All" , "Ongoing" , "Overdue" , "Completed"
    };

    public BorrowViewModel(AuthenticationService authService)
    {
        _authService = authService;
        SelectedStatusFilter=StatusFilters.FirstOrDefault();
        RetrieveInvoices();
    }

    partial void OnFindKeyChanged(string value)
    {
       InvoicesFindList.Clear();
       switch (SelectedStatusFilter)
       {
           case "Ongoing":
               foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Ongoing"))
               {
                   foreach (MEMBER mem in ci.Member)
                   {
                       if (mem.CitizenID.ToLower().Contains(value.ToLower())) InvoicesFindList.Add(ci);
                   }
               }
    
               break;
            
           case "Overdue":
               foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Overdue"))
               {
                   foreach (MEMBER mem in ci.Member)
                   {
                       if (mem.CitizenID.ToLower().Contains(value.ToLower())) InvoicesFindList.Add(ci);
                   }
               }
    
               break;
            
           case "Completed":
               foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Completed"))
               {
                   foreach (MEMBER mem in ci.Member)
                   {
                       if (mem.CitizenID.ToLower().Contains(value.ToLower())) InvoicesFindList.Add(ci);
                   }
               }
    
               break;
           
           case "All":
               foreach (CustomInvoice ci in Invoices)
               {
                   foreach (MEMBER mem in ci.Member)
                   {
                       if (mem.CitizenID.ToLower().Contains(value.ToLower())) InvoicesFindList.Add(ci);
                   }
               }

               break;
       }

       ShowingList = InvoicesFindList;
    }

    partial void OnSelectedStatusFilterChanged(string? oldValue, string newValue)
    {
        InvoicesFindList.Clear();
        string finkey = (FindKey == null) ? "" : FindKey;
        switch (newValue)
        {
            case "Ongoing":
                foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Ongoing"))
                {
                    foreach (MEMBER mem in ci.Member)
                    {
                        if (mem.CitizenID.ToLower().Contains(finkey.ToLower())) InvoicesFindList.Add(ci);
                    }
                }
    
                break;
            
            case "Overdue":
                foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Overdue"))
                {
                    foreach (MEMBER mem in ci.Member)
                    {
                        if (mem.CitizenID.ToLower().Contains(finkey.ToLower())) InvoicesFindList.Add(ci);
                    }
                }
    
                break;
            
            case "Completed":
                foreach (CustomInvoice ci in Invoices.Where(e=>e.Status=="Completed"))
                {
                    foreach (MEMBER mem in ci.Member)
                    {
                        if (mem.CitizenID.ToLower().Contains(finkey.ToLower())) InvoicesFindList.Add(ci);
                    }
                }
    
                break;
            
            case "All":
                foreach (CustomInvoice ci in Invoices)
                {
                    foreach (MEMBER mem in ci.Member)
                    {
                        if (mem.CitizenID.ToLower().Contains(finkey.ToLower())) InvoicesFindList.Add(ci);
                    }
                }

                break;
        }
        ShowingList = InvoicesFindList;
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

        Invoices = new ObservableCollection<CustomInvoice>(Invoices.OrderByDescending(e => e.BorrowInvoiceID));
        ShowingList = Invoices;
    }

    [RelayCommand]
    public void ShowInvoice(CustomInvoice invoice)
    {
        var window = App.AppHost!.Services.GetRequiredService<InvoiceWindow>();
        var viewModel = App.AppHost!.Services.GetRequiredService<InvoiceWindowViewModel>();
        viewModel.Invoice = invoice;
        window.DataContext = viewModel;
        window.Show();
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
        
        if (BorrowDetails.All(detail => detail.HasReturned))
        {
            _status = "Completed";
        }
        else if (BorrowDetails.Any(detail => detail.DueDate < today))
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

        if (BorrowDetails.All(detail => detail.HasReturned))
        {
            _statusColor = new SolidColorBrush(Colors.LimeGreen);
        }
        else if (BorrowDetails.Any(detail => detail.DueDate < today))
        {
            _statusColor = new SolidColorBrush(Colors.Red);
        }
        else if (BorrowDetails.Any(detail => detail.DueDate > today))
        {
            _statusColor = new SolidColorBrush(Colors.Orange);
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