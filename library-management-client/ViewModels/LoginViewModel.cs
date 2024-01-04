using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class LoginViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _username = String.Empty;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string? _password = String.Empty;
    [ObservableProperty] private bool _isRemember = false;
    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy = false;
    
    public delegate void EventHandler();
    public event EventHandler Authenticated;

    public LoginViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute=nameof(CheckUAndPNotNull))]
    async Task Login()
    {
        if (Username!.Length == 0 || Password!.Length == 0) return ;

        IsBusy = true;

        try
        {
            // API call to authenticate user
            var user = await Task.Run(() => _authService.LoginAsync(Username, Password, IsRemember));
            Authenticated.Invoke();
            await Task.Delay(1000);
            
            // Clear existing error (if there are any)
            HasError = false;
            
            // Switch main window to dashboard
            var currentViewModel = App.AppHost!.Services.GetRequiredService<MainWindowViewModel>();
            var currentWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();

            currentViewModel.ContentViewModel = App.AppHost!.Services.GetRequiredService<DashboardViewModel>();
            currentWindow.CanResize = true;
            
            // Update Homepage to display new logged in user
            var homeViewModel = App.AppHost!.Services.GetService<HomeViewModel>();
            homeViewModel.CurrentUser = _authService.CurrentUser;
        }
        catch (Exception e)
        {
            HasError = true;
            ErrorMessage = e.Message;
        }

        IsBusy = false;
    }


    private bool CheckUAndPNotNull()
    {
        return !string.IsNullOrEmpty(Username)&&!string.IsNullOrEmpty(Password);
    }
}