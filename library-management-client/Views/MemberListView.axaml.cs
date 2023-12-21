using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class MemberListView : UserControl
{
    public MemberListView()
    {
        InitializeComponent();
    }


    private void SearchBox_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        SearchBox.Padding = new Thickness(10, 0, 0, 0);
    }

    private void SearchBox_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        SearchBox.Padding = new Thickness(30, 0, 0, 0);
    }
}