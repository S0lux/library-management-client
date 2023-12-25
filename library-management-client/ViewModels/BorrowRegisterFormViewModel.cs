using CommunityToolkit.Mvvm.ComponentModel;
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
        private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };
    }
}
