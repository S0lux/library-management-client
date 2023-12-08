using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class SidebarScreenView : UserControl
{
    public SidebarScreenView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is SidebarScreenViewModel)
        {
            var viewModel = DataContext as SidebarScreenViewModel;
            viewModel!.IsExpanded = !viewModel.IsExpanded;

            if (viewModel.ViewModel is not null)
            {
                var type = viewModel.ViewModel!;
                App.AppHost.Services.GetRequiredService<DashboardViewModel>().ActiveViewModel = App.AppHost?.Services.GetRequiredService(type) as ViewModelBase;
            }
        }
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        e.Handled = true ;
    }
}