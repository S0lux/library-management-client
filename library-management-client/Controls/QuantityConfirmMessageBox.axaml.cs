using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Controls
{
    public partial class QuantityConfirmMessageBox : Window
    {
        public QuantityConfirmMessageBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public enum MessageBoxButton
        {
            OkCancel
        }

        public enum ButtonResult
        {
            NULL,
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

        public QuantityConfirmMessageBox(string message, string title, MessageBoxImage img)
        {
            InitializeComponent();
            MessageBoxContent.Text = message;
            MessageBoxTitle.Content = title;
            buttonResultClicked = ButtonResult.NULL;
            setIcon(img);
            this.Owner = App.AppHost!.Services.GetRequiredService<MainWindow>();
            this.Width = 400;
            this.Height = 200;
            
        }

        public QuantityConfirmMessageBox(string message, string title, MessageBoxImage img, double width, double height)
        {
            InitializeComponent();
            MessageBoxContent.Text = message;
            MessageBoxTitle.Content = title;
            buttonResultClicked = ButtonResult.NULL;
            setIcon(img);
            this.Owner = App.AppHost!.Services.GetRequiredService<MainWindow>();
            this.Width = width;
            this.Height = height;

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
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.OK;
            var temp = App.AppHost.Services.GetRequiredService<AddByTitleViewModel>();
            if (Quantity.Value == null) temp.BookQuantity = 1;
            if (Location.Value == null) temp.ShelfNumber = 0;
            else
            {
                temp.BookQuantity = (int)Quantity.Value;
                temp.ShelfNumber = (int)Location.Value;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            buttonResultClicked = ButtonResult.CANCEL;
            this.Close();
        }
        
    }
}
