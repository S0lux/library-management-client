using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        
        private uint _returned;
        public uint Returned
        {
            get => _returned;
            set
            {
                ValidateSum(value, Damaged, Lost);
                _returned = value;
            }
        }
        
        private uint _damaged;
        public uint Damaged
        {
            get => _damaged;
            set
            {
                ValidateSum(Returned, value, Lost);
                _damaged = value;
            }
        }
        
        private uint _lost;
        public uint Lost
        {
            get => _lost;
            set
            {
                ValidateSum(Returned, Damaged, value);
                _lost = value;
            }
        }

        [ObservableProperty] [NotifyDataErrorInfo] [Range(1, 100)] 
        [CustomValidation(typeof(BORROW_DETAIL), nameof(ValidateQuantity))]
        private int _quantity;

        public static ValidationResult ValidateQuantity(string value, ValidationContext context)
        {
            var isParsed = int.TryParse(value, out int quantity);
            if (!isParsed) return new("Input must be a number.");
            
            var borrowDetail = (BORROW_DETAIL)context.ObjectInstance;
            
            var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
            var bookList = bookViewModel.BookList;

            var correspondingBook = bookList.FirstOrDefault(book => book.ISBN13 == borrowDetail.ISBN13);
            if (correspondingBook.BOOK_DETAILs.First(e => e.Status == "normal").Quantity < borrowDetail.Quantity)
            {
                return new("The number of borrowed books must not be higher than the available quantity");
            }
            return ValidationResult.Success;
        }

        public DateTime DueDate { get; set; }
        
        public bool HasReturned { get; set; }

        [ObservableProperty] [Range(1, 90)] [NotifyDataErrorInfo]
        private uint _borrowDuration;

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

        private void ValidateSum(uint returned, uint damaged, uint lost)
        {
            
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
    }
}
