using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public BorrowRegisterFormViewModel()
        {
            memberSearchViewModel = App.AppHost!.Services.GetRequiredService<MemberListViewModel>();
            bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();

            
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
        public void CheckOutta()
        {
            uint a = 0;

            foreach(BORROW_DETAIL bOOK in BorrowDetailList) {
                a += bOOK.Quantity;
            }

            new MyMessageBox(a.ToString()).Show();
        }

        [RelayCommand]
        public void Delete()
        {
            BOOK bOOK = bookViewModel.BookCheckedList.FirstOrDefault(e => e.ISBN13 == BorrowFormSelectedBookDetail.ISBN13);

            bOOK.IsCheck = false;
            bookViewModel.BookCheckedList.Remove(bOOK);
            bookViewModel.SelectedNumber -= 1;
            
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
