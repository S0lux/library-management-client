using Avalonia_DependencyInjection.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddByISBNViewModel:ViewModelBase
{

    [ObservableProperty] private string _test="TEST";
    [ObservableProperty] private BOOK _book;
    public AddByISBNViewModel()
    {
        
    }
}