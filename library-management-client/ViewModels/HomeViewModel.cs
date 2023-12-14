using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly AuthenticationService _authenticationService;
    [ObservableProperty] private AuthUser? _currentUser;

    public HomeViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        CurrentUser = authenticationService.CurrentUser;
    }
}