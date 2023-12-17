using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class MemberRegistryForm : Window
{
    public MemberRegistryForm()
    {
        InitializeComponent();
        DataContext = new MemberRegistryFormViewModel(this);

    }
}