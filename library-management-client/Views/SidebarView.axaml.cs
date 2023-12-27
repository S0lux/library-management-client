using System;
using System.IO;
using Avalonia;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia_DependencyInjection.Controls;

namespace Avalonia_DependencyInjection.Views;

public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
    }

    /*private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        var targetControl = (Grid)sender!;
        var framerate = TimeSpan.FromSeconds(1 / 90.0);
        var animationSecs = TimeSpan.FromSeconds(0.25);
        var easing = new CubicEaseIn();
        var totalTicks = animationSecs.TotalSeconds / framerate.TotalSeconds;
        var currentTick = 0;

        var translateTransform = new TranslateTransform(0, 0);
        targetControl.RenderTransform = translateTransform;

        var timer = new DispatcherTimer();
        timer.Interval = framerate;
        timer.Tick += (o, args) =>
        {
            currentTick++;

            if (currentTick > totalTicks)
            {
                timer.Stop();
                translateTransform.X = 0;
                return;
            }

            var currentProgress = currentTick / totalTicks;
            translateTransform.X = -240 + 240 * easing.Ease(currentProgress);
        };

        translateTransform.X = -240;
        timer.Start();
    }*/

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        //var box = MessageBoxManager
        //    .GetMessageBoxStandard("Logging out", "Do you want to log out?",
        //        ButtonEnum.YesNo, Icon.Question);

        var box = new MyMessageBox("Do you want to log out?", "Logging out", MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Question);

        await box.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());

        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            File.Delete("userToken.txt");
            var win=App.AppHost!.Services.GetRequiredService<MainWindowViewModel>();
            win.ContentViewModel = App.AppHost.Services.GetRequiredService<LoginViewModel>();
        }
    }
}