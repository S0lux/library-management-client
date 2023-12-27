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

        public int Quantity { get; set; }

        public DateTime DueDate { get; set; }
        
        public bool HasReturned { get; set; }
    }
}
