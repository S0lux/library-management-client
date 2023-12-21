using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void TitleBarContainer_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var _mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
        _mainWindow.BeginMoveDrag(e);
    }

    private void AvaloniaObject_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ContentProperty)
        {
            var contentControl = (ContentControl) sender!;
            var framerate = TimeSpan.FromSeconds(1 / 90.0);
            var animationSeconds = TimeSpan.FromSeconds(0.25);
            var totalTicks = animationSeconds.TotalSeconds / framerate.TotalSeconds;
            var currentTick = 0;
            var timer = new DispatcherTimer();
            
            timer.Interval = framerate;
            timer.Tick += (o, args) =>
            {
                currentTick++;
                
                if (currentTick >= totalTicks)
                {
                    timer.Stop();
                    return;
                }

                var animationProgress = (double)currentTick / totalTicks;
                contentControl.Margin = new Thickness(0, 25.0 - (25.0 * animationProgress), 0, 0);
                contentControl.Opacity = animationProgress;
            };

            contentControl.Margin = new Thickness(0, 25, 0, 0);
            contentControl.Opacity = 0;
            timer.Start();
        }
    }
}