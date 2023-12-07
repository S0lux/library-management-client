using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class TitlebarView : UserControl
{
    public TitlebarView()
    {
        InitializeComponent();
    }

    private void Close_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathExit = "/Assets/SVGs/xmark-white.svg";
    }

    private void Close_OnPointerExited(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathExit = "/Assets/SVGs/xmark.svg";
    }

    private void Maximize_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathMaximize = "/Assets/SVGs/window-maximize-white.svg";
    }

    private void Maximize_OnPointerExited(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathMaximize = "/Assets/SVGs/window-maximize.svg";
    }
    
    private void Minimize_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathMinimize = "/Assets/SVGs/window-minimize-white.svg";
    }

    private void Minimize_OnPointerExited(object? sender, PointerEventArgs e)
    {
        var viewModel = DataContext as TitlebarViewModel;
        viewModel.IconPathMinimize = "/Assets/SVGs/window-minimize.svg";
    }
}