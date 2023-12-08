using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
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
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
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

                services.AddHttpClient("main", options =>
                {
                    options.BaseAddress = new Uri("https://library-management-api-five.vercel.app/");
                });
            })
            .Build();
        
        await AuthenticateAsync();

        base.OnFrameworkInitializationCompleted();
    }

    private async Task AuthenticateAsync()
    {
        var authService = AppHost!.Services.GetRequiredService<AuthenticationService>();
        var currentViewModel = AppHost!.Services.GetRequiredService<MainWindowViewModel>();

        if (await authService.VerifyTokenAsync())
        {
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            currentViewModel.ContentViewModel = AppHost!.Services.GetRequiredService<DashboardViewModel>();
            mainWindow.CanResize = true;
            mainWindow.MinWidth = 400;
            mainWindow.Show();
        }
        else
        {
            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            currentViewModel.ContentViewModel = AppHost!.Services.GetRequiredService<LoginViewModel>();
            mainWindow.Show();
        }
    }
}