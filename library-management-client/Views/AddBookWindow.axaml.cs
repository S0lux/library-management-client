using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class AddBookWindow : Window
{
    public AddBookWindow(AddBookWindowViewModel addBookWindowViewModel)
    {
        InitializeComponent();
        DataContext = addBookWindowViewModel;
    }
    private void TitleBarContainer_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }
    
    private void Close_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        var viewModel = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
        viewModel.IconPathExit = "/Assets/SVGs/xmark-white.svg";
    }

    private void Close_OnPointerExited(object? sender, PointerEventArgs e)
    {
        var viewModel = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
        viewModel.IconPathExit = "/Assets/SVGs/xmark-royalblue.svg";
    }
}