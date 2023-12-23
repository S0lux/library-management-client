using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Controls
{
    public partial class MyMessageBox : Window
    {
        public MyMessageBox()
        {
            InitializeComponent();
        }

        public enum MessageBoxButton
        {
            YesNoCancel,
            OK,
            YesNo,
            OkCancel
        }

        public enum ButtonResult
        {
            NULL,
            YES,
            NO,
            CANCEL,
            OK
        }

        public enum MessageBoxImage
        {
            Information,
            Question,
            Warning,
            Error
        }

        public static ButtonResult buttonResultClicked;

        public MyMessageBox(string message, string title, MessageBoxButton button, MessageBoxImage img)
        {
            InitializeComponent();
            MessageBoxContent.Text = message;
            MessageBoxTitle.Content = title;
            buttonResultClicked = ButtonResult.NULL;
            setButton(button);
            setIcon(img);
            this.Owner = App.AppHost!.Services.GetRequiredService<MainWindow>();
        }

        private void setButton(MessageBoxButton button)
        {
            if (button == MessageBoxButton.YesNoCancel)
            {
                ButtonList.Children.Remove(btnOk);

            }
            else if (button == MessageBoxButton.OK)
            {
                ButtonList.Children.Remove(btnCancel);
                ButtonList.Children.Remove(btnNo);
                ButtonList.Children.Remove(btnYes);

            }
            else if (button == MessageBoxButton.YesNo)
            {
                ButtonList.Children.Remove(btnCancel);
                ButtonList.Children.Remove(btnOk);

            }
            else if (button == MessageBoxButton.OkCancel)
            {
                ButtonList.Children.Remove(btnYes);
                ButtonList.Children.Remove(btnNo);
            }
        }

        private void setIcon(MessageBoxImage img)
        {
            switch (img)
            {
                case MessageBoxImage.Information:
                    MessageBoxIcon.Path = "/Assets/SVGs/info-solid.svg";
                    MessageBoxIcon.Height = 50;
                    break;
                case MessageBoxImage.Warning:
                    MessageBoxIcon.Path = "/Assets/SVGs/exclamation-solid.svg";
                    break;
                case MessageBoxImage.Error:
                    MessageBoxIcon.Path = "/Assets/SVGs/triangle-exclamation-solid.svg";
                    MessageBoxIcon.Height = 50;
                    break;
                case MessageBoxImage.Question:
                    MessageBoxIcon.Path = "/Assets/SVGs/question-solid.svg";
                    MessageBoxIcon.Height = 50;
                    break;
            }
        }

        private void MessageBoxCloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.YES;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.OK;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.NO;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.CANCEL;
            this.Close();
        }

    }
}
