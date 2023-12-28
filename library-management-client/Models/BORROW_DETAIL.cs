using System;
using System.Linq;
using Avalonia.Media;
using Avalonia_DependencyInjection.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_DependencyInjection.Models
{
    public class BORROW_DETAIL
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

        private uint _quantity;
        public uint Quantity
        {
            get => _quantity;
            set
            {
                var box = App.AppHost!.Services.GetRequiredService<BookViewModel>();

                BOOK temp = box.BookList.FirstOrDefault(e => e.ISBN13 == ISBN13);

                if (value > temp.BOOK_DETAILs.First(e => e.Status == "normal").Quantity && temp != null)
                {
                    throw new ArgumentException("Quantity can't be greater than the available amount.",);
                }

                if (value == 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }
                ValidateSum(Returned, Damaged, Lost);
                _quantity = value;
            }
        }

        public DateTime DueDate { get; set; }
        
        public bool HasReturned { get; set; }

        private uint _borrowDuration;
        public uint BorrowDuration
        {
            get => _borrowDuration;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Borrow duration must be greater than 0.");
                }
                _borrowDuration = value;
            }
        }

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
            if (returned + damaged + lost > Quantity)
            {
                throw new ArgumentException("The sum of Returned, Damaged, and Lost must not be greater than Quantity.");
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
    }
}
