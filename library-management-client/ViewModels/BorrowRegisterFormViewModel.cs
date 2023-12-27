using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.ViewModels
{
    public partial class BorrowRegisterFormViewModel:ViewModelBase
    {
        [ObservableProperty] 
        private string? _filterKey;

        [ObservableProperty]
        private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CheckOuttaCommand))]
        private MEMBER? _borrowMEMBER ;

        [ObservableProperty]
        private ObservableCollection<BORROW_DETAIL> _borrowDetailList = new ObservableCollection<BORROW_DETAIL>();

        [ObservableProperty]
        private BORROW_DETAIL? _borrowFormSelectedBookDetail;

        MemberListViewModel? memberSearchViewModel;

        BookViewModel? bookViewModel;

        BorrowViewModel? borrowViewModel;

        AuthenticationService? _authService;

        public BorrowRegisterFormViewModel(AuthenticationService authService)
        {
            memberSearchViewModel = App.AppHost!.Services.GetRequiredService<MemberListViewModel>();
            bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
            borrowViewModel = App.AppHost!.Services.GetRequiredService<BorrowViewModel>();
            _authService = authService;
        }

        partial void OnFilterKeyChanged(string? oldValue, string newValue)
        {
            MEMBER checkOutMember = memberSearchViewModel.MemberList.FirstOrDefault(e => e.CitizenID == FilterKey);
            
            if (checkOutMember != null)
            {
                BorrowMEMBER = checkOutMember;
                BorrowMEMBER.PropertyChanged += (sender, args) => { CheckOuttaCommand.NotifyCanExecuteChanged(); };
            }
            else
            {
                BorrowMEMBER = null;
            }
        }


        public bool checkSubmit()
        {
            if(BorrowMEMBER == null)
            {
                return false;
            }
            return true;
        }

        [RelayCommand(CanExecute = nameof(checkSubmit))]
        async public void CheckOutta()
        {
            List<BORROW_DETAIL> bORROW_DETAILs = new List<BORROW_DETAIL>();

            foreach(BORROW_DETAIL bORROW in BorrowDetailList)
            {
                var borrowDetailData = new BORROW_DETAIL()
                {
                    ISBN13 = bORROW.ISBN13,
                    Quantity = bORROW.Quantity,
                    DueDate = DateTime.Now.AddDays(bORROW.BorrowDuration),
                };
                
                bORROW_DETAILs.Add(borrowDetailData);
            }

            var invoiceInfo = new
            {
                EmployeeID = _authService.CurrentUser!.EmployeeID,
                MemberID = BorrowMEMBER.MemberID,
                BorrowingDate = DateTime.Now,
                BorrowDetails = bORROW_DETAILs,
            };

            var payload = new
            {
                data = invoiceInfo
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _authService.PostAsync("/api/books/invoices", content);

            borrowViewModel.RetrieveInvoices();
        }

        [RelayCommand]
        public void Delete()
        {
            BOOK bOOK = bookViewModel.BookCheckedList.FirstOrDefault(e => e.ISBN13 == BorrowFormSelectedBookDetail.ISBN13);

            bOOK.IsCheck = false;
            bookViewModel.BookCheckedList.Remove(bOOK);
            bookViewModel.CheckedAmount -= 1;
            
            BorrowDetailList.Remove(BorrowFormSelectedBookDetail);
            
            if(bookViewModel.BookCheckedList.Count() <= 0)
            {
                var box = App.AppHost!.Services.GetRequiredService<BorrowRegisterFormView>();
                box.Hide();
            }
        }

        public void Load()
        {
            foreach (BOOK bOOK in bookViewModel.BookCheckedList)
            {
                if (!BorrowDetailList.Contains(BorrowDetailList.FirstOrDefault(e => e.ISBN13 == bOOK.ISBN13)))
                {
                    BorrowDetailList.Add(new BORROW_DETAIL()
                    {
                        ISBN13 = bOOK.ISBN13,
                        Quantity = 1,
                        BorrowDuration = 30,
                        BookTitle = bOOK.Title
                    });
                }
            }
        }
    }
}
