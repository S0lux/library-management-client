using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    internal class BORROW_DETAIL
    {
        public int BorrowInvoiceID { get; set; }

        [MaxLength(4)]
        public int BookID { get; set; }

        public int Quantity { get; set; }

        public DateTime DueDate { get; set; }
    }
}
