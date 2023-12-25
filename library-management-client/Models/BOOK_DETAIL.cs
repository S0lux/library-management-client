using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models
{
    public partial class BOOK_DETAIL:ObservableObject
    {
        [ObservableProperty] private string _ISBN13;

        [ObservableProperty] private string _status;

        [ObservableProperty] private int? _quantity;
    }
}
