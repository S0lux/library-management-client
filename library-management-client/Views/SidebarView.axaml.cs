using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

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
}