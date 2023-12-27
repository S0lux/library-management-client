using Avalonia.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace Avalonia_DependencyInjection.Views
{
    public partial class BorrowRegisterFormView : Window
    {
        public BorrowRegisterFormView(BorrowRegisterFormViewModel dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext;
        }

        private void StackPanel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }

        private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void CloseButton_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
