using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Avalonia_DependencyInjection;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<LoginView>();
                    services.AddSingleton<LoginViewModel>();
                    services.AddSingleton<DashboardView>();
                    services.AddSingleton<DashboardViewModel>();
                    services.AddSingleton<HomeView>();
                    services.AddSingleton<HomeViewModel>();
                    services.AddSingleton<AuthenticationService>();
                    services.AddSingleton<SidebarView>();
                    services.AddSingleton<SidebarViewModel>();
                    services.AddSingleton<SidebarScreenView>();
                    services.AddSingleton<SidebarScreenViewModel>();
                    services.AddSingleton<MemberRegistryViewModel>();
                    services.AddSingleton<MemberListViewModel>();
                    services.AddSingleton<TitlebarViewModel>();
                    services.AddSingleton<MemberRegistryFormViewModel>();
                    services.AddSingleton<MemberRegistryForm>();
                    services.AddSingleton<BookView>();
                    services.AddSingleton<BookViewModel>();
                    services.AddSingleton<AddBookWindow>();
                    services.AddSingleton<AddBookWindowViewModel>();
                    services.AddSingleton<AddByISBNView>();
                    services.AddSingleton<AddByTitleView>();
                    services.AddSingleton<ManualAddingView>();
                    services.AddSingleton<AddByISBNViewModel>();
                    services.AddSingleton<AddByTitleViewModel>();
                    services.AddSingleton<ManualAddingViewModel>();
                    services.AddSingleton<BorrowRegisterFormView>();
                    services.AddSingleton<BorrowRegisterFormViewModel>();
                    services.AddSingleton<BookInfoView>();
                    services.AddSingleton<BookInfoViewModel>();
                    services.AddSingleton<BorrowRegisterFormView>();
                    services.AddSingleton<BorrowRegisterFormViewModel>();
                    services.AddSingleton<BorrowView>();
                    services.AddSingleton<BorrowViewModel>();
                    services.AddTransient<InvoiceWindow>();
                    services.AddTransient<InvoiceWindowViewModel>();
                    services.AddSingleton<EmployeeListView>();
                    services.AddSingleton<EmployeeListViewModel>();
                    services.AddSingleton<EmployeeRegisterFormViewModel>();
                    services.AddSingleton<EmployeeRegisterFormView>();

                    services.AddHttpClient("main", options =>
                    {
                        options.BaseAddress = new Uri("https://library-management-api-five.vercel.app");
                    });
                })
                .Build();

            await AuthenticateAsync();

            desktop.ShutdownRequested += OnExit;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private async void OnExit(object sender, ShutdownRequestedEventArgs e)
    {
        e.Cancel = true;
        var _authService = AppHost.Services.GetRequiredService<AuthenticationService>();
        var currentUser = _authService.CurrentUser;

        if (currentUser is not null && currentUser.remember == false)
        {
            await _authService.GetAsync("/api/logout");
        }
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.Shutdown();
    } 

    private async Task AuthenticateAsync()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = AppHost!.Services.GetRequiredService<AuthenticationService>();
            var currentViewModel = AppHost!.Services.GetRequiredService<MainWindowViewModel>();

            if (await authService.VerifyTokenAsync())
            {
                desktop.MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                currentViewModel.ContentViewModel = AppHost!.Services.GetRequiredService<DashboardViewModel>();
                desktop.MainWindow.CanResize = true;
                desktop.MainWindow.MinWidth = 400;
                desktop.MainWindow.Show();
            }
            else
            {
                desktop.MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                currentViewModel.ContentViewModel = AppHost!.Services.GetRequiredService<LoginViewModel>();
                desktop.MainWindow.Show();
            } 
        }
        
    }
}