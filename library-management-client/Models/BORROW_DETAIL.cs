using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia_DependencyInjection.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Models
{
    public partial class BORROW_DETAIL: ObservableValidator
    {
        public int BorrowInvoiceID { get; set; }
        
        public string ISBN13 { get; set; }
        
        [ObservableProperty] [Range(0, 100)] [NotifyDataErrorInfo] 
        [CustomValidation(typeof(BORROW_DETAIL), nameof(ValidateSum))]
        private uint _returned;
        
        [ObservableProperty] [Range(0, 100)] [NotifyDataErrorInfo] 
        [CustomValidation(typeof(BORROW_DETAIL), nameof(ValidateSum))]
        private uint _damaged;
        
        [ObservableProperty] [Range(0, 100)] [NotifyDataErrorInfo] 
        [CustomValidation(typeof(BORROW_DETAIL), nameof(ValidateSum))]
        private uint _lost;

        [ObservableProperty] [NotifyDataErrorInfo] [Range(0, 100)]
        [CustomValidation(typeof(BORROW_DETAIL), nameof(ValidateQuantity))]
        private int _quantity;
        
        [ObservableProperty] [Range(1, 90)] [NotifyDataErrorInfo]
        private uint _borrowDuration;

        public DateTime DueDate { get; set; }
        
        public bool HasReturned { get; set; }

        public string BookTitle { get; set; }

        private string _status;

        public string Status
        {
            get
            {
                UpdateStatus();
                return _status;
            }
        }

        private SolidColorBrush _statusColor;
        public SolidColorBrush StatusColor
        {
            get
            {
                UpdateStatus();
                return _statusColor;
            }
        }
        
        private void UpdateStatus()
        {
            DateTime today = DateTime.Today;
            
            if (HasReturned)
            {
                _status = "Returned";
                _statusColor = new SolidColorBrush(Colors.LimeGreen);
            }
            else if (DueDate < today)
            {
                _status = "Overdue";
                _statusColor = new SolidColorBrush(Colors.Red);
            }
            else
            {
                _status = "Ongoing";
                _statusColor = new SolidColorBrush(Colors.Orange);
            }
        }
        
        public static ValidationResult ValidateQuantity(string value, ValidationContext context)
        {
            var isParsed = int.TryParse(value, out int quantity);
            if (!isParsed) return new("Input must be a number.");
            
            var borrowDetail = (BORROW_DETAIL)context.ObjectInstance;

            var dashboardViewModel = App.AppHost!.Services.GetRequiredService<DashboardViewModel>();
            if (dashboardViewModel.ActiveViewModel is BookViewModel)
            {
                var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
                var bookList = bookViewModel.BookList;

                var correspondingBook = bookList.FirstOrDefault(book => book.ISBN13 == borrowDetail.ISBN13);
                if (correspondingBook?.BOOK_DETAILs.First(e => e.Status == "normal").Quantity < borrowDetail.Quantity)
                {
                    return new("The number of borrowed books must not be higher than the available quantity");
                }
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateSum(string value, ValidationContext context)
        {
            var borrowDetail = (BORROW_DETAIL)context.ObjectInstance;
            if (borrowDetail.Quantity < borrowDetail.Returned + borrowDetail.Lost + borrowDetail.Damaged)
            {
                return new("The sum of returned, damaged, and lost books must not be greater than amount");
            }
            
            return ValidationResult.Success;
        }
    }
}
