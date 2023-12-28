using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;

namespace Avalonia_DependencyInjection.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}