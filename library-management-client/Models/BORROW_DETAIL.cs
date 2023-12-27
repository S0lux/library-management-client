using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    public class BORROW_DETAIL
    {
        public int BorrowInvoiceID { get; set; }

        
        public string ISBN13 { get; set; }

        public uint Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException();
                }
                _quantity = value;
            }
        }

        public DateTime DueDate { get; set; }
        
        public bool HasReturned { get; set; }


        private uint _borrowDuration;
        private uint _quantity;

        public uint BorrowDuration { get 
            { 
                return _borrowDuration;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException();
                }
                _borrowDuration = value;
            }
        }
        public string BookTitle { get; set; }
    }
}
