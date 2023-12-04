using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
        mainWindow.BeginMoveDrag(e);
    }
}