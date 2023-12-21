using System;
using System.Globalization;
using System.Reactive.PlatformServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class LoginView : UserControl
{
    private readonly LoginViewModel _loginViewModel;

    public LoginView()
    {
        InitializeComponent();
        _loginViewModel = App.AppHost!.Services.GetRequiredService<LoginViewModel>();
        _loginViewModel.Authenticated += () =>
        {
            LoginTransition();
        };
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
        mainWindow.BeginMoveDrag(e);
    }

    public async Task LoginTransition()
    {
        LeftImage.Classes.Add("FadeOut");
        SpinnerGrid.Classes.Add("FadeOut");
        
        var targetColumn = MainGrid.ColumnDefinitions[0];
        var framerate = TimeSpan.FromSeconds(1 / 90.0);
        var animationSecs = TimeSpan.FromSeconds(0.75);
        var easing = new CubicEaseIn();
        var totalTicks = animationSecs.TotalSeconds / framerate.TotalSeconds;
        var currentTick = 0;

        var timer = new DispatcherTimer();
        timer.Interval = framerate;
        timer.Tick += (o, args) =>
        {
            currentTick++;

            if (currentTick > totalTicks || targetColumn.ActualWidth <= 240)
            {
                timer.Stop();
                targetColumn.Width = new GridLength(240);
                return;
            }

            var currentProgress = currentTick / totalTicks;
            targetColumn.Width = new GridLength(240 + (targetColumn.ActualWidth - 240) * (1 - easing.Ease(currentProgress)));
        };

        targetColumn.Width = new GridLength(targetColumn.ActualWidth, GridUnitType.Pixel);
        timer.Start();
    }
}