using System.Collections.ObjectModel;
using System.IO;
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
            new("Home", typeof(HomeViewModel), "/Assets/SVGs/house.svg"),
            new("Member", null, "/Assets/SVGs/user-white.svg", new ObservableCollection<SidebarScreenViewModel>
            {
                new("List", typeof(MemberListViewModel), "/Assets/SVGs/users.svg"),
                new("Add", typeof(MemberRegistryViewModel), "/Assets/SVGs/user-plus.svg"),
                new("Remove", null, "/Assets/SVGs/user-minus.svg"),
            }),

            new("Book", null, "/Assets/SVGs/books.svg"),
        });
    }
}