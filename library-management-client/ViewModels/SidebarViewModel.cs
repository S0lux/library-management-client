using System.Collections.ObjectModel;
using System.IO;
using Avalonia_DependencyInjection.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class SidebarViewModel: ViewModelBase
{
    [ObservableProperty] private SidebarScreenViewModel _sidebarScreen;
    [ObservableProperty] private SidebarScreenViewModel _selectedScreen;

    public SidebarViewModel()
    {
        SidebarScreen = new SidebarScreenViewModel("root", null, null, new ObservableCollection<SidebarScreenViewModel>()
        {
            new("Home", typeof(HomeViewModel), "/Assets/SVGs/house-solid.svg"),

            new("Member", typeof(MemberListViewModel), "/Assets/SVGs/user-group-solid.svg"),

            new("Book", null, "/Assets/SVGs/book-open-cover.svg", new ObservableCollection<SidebarScreenViewModel>()
            {
                new("List", typeof(BookViewModel), "/Assets/SVGs/books.svg"),
                new("Borrowed", typeof(BorrowViewModel), "/Assets/SVGs/hand-holding-box.svg")
            }),
        });
    }
}