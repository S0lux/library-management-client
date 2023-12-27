using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private MEMBER? _borrowMEMBER;

        [ObservableProperty]
        private ObservableCollection<BOOK> _borrowList = new ObservableCollection<BOOK>();

        [ObservableProperty]
        private BOOK? _borrowFormSelectedBook;

        MemberListViewModel? memberSearchViewModel;

        public BorrowRegisterFormViewModel()
        {
            memberSearchViewModel = App.AppHost!.Services.GetRequiredService<MemberListViewModel>();
        }

        partial void OnFilterKeyChanged(string? oldValue, string newValue)
        {
            MEMBER checkOutMember = memberSearchViewModel.MemberList.FirstOrDefault(e => e.CitizenID == FilterKey);

            if (checkOutMember != null)
            {
                BorrowMEMBER = checkOutMember;
            }
            else
            {
                BorrowMEMBER = null;
            }
        }

        [RelayCommand]
        public void CheckOutta()
        {
            int a = 0;

            foreach(BOOK bOOK in BorrowList) {
                a += bOOK.BorrowQuantity;
            }

            new MyMessageBox(a.ToString()).Show();
        }

        [RelayCommand]
        public void Delete()
        {
            BorrowFormSelectedBook.IsCheck = false;
            BorrowList.Remove(BorrowFormSelectedBook);
            
            if(BorrowList.Count() <= 0)
            {
                var box = App.AppHost!.Services.GetRequiredService<BorrowRegisterFormView>();
                box.Hide();
            }
        }
    }
}
