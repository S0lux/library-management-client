using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class SidebarViewModel: ViewModelBase
{
    [ObservableProperty] private SidebarScreenViewModel _sidebarScreen;
    [ObservableProperty] private SidebarScreenViewModel _selectedScreen;
    private AuthenticationService _authService;
    HttpResponseMessage response = new();

    public SidebarViewModel(AuthenticationService authenticationService)
    {
        _authService = authenticationService;
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

        CheckValid();
    }

    public async Task CheckValid()
    {
        response = await _authService.GetAsync(@"/api/employees");

        SidebarScreenViewModel model = new("Employee", typeof(EmployeeListViewModel), "/Assets/SVGs/house-solid.svg");
        if (response.StatusCode == System.Net.HttpStatusCode.OK && SidebarScreen.Screens.Count == 3)
        {
            SidebarScreen.Screens.Add(model);
        }
        
        if(response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            if(SidebarScreen.Screens[3] == null)
            {
                return;
            }

            if (SidebarScreen.Screens[3].ViewModel == typeof(EmployeeListViewModel))
            {
                SidebarScreen.Screens.Remove(SidebarScreen.Screens[3]);
            }
        }
    }
}