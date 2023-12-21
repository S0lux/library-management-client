using System;
using System.IO;
using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Hello");
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        File.Delete("userToken.txt");
        var win=App.AppHost.Services.GetRequiredService<MainWindowViewModel>();
        win.ContentViewModel = App.AppHost.Services.GetRequiredService<LoginViewModel>();
    }
}