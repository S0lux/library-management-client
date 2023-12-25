using Avalonia.Controls;
using Avalonia_DependencyInjection.ViewModels;

namespace Avalonia_DependencyInjection.Views
{
    public partial class BorrowRegisterFormView : Window
    {
        public BorrowRegisterFormView(/*BorrowRegisterFormViewModel dataContext*/)
        {
            InitializeComponent();
            //this.DataContext = dataContext;
        }

        private void StackPanel_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
        }
    }
}
