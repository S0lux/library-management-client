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
                

                services.AddHttpClient("main", options =>
                {
                    options.BaseAddress = new Uri("https://library-management-api-five.vercel.app");
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