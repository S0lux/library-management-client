using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    internal class BORROW_INVOICE
    {
        [Key]
        public int BorrowInvoiceID { get; set; }

        [MaxLength(4)]
        public string BookID { get; set; }
        public BOOK Book { get; set; }

        [MaxLength(4)]
        public string EmpID { get; set; }
        public EMPLOYEE Employee { get; set; }

        public DateTime BorrowingDate { get; set; }

        public ICollection<BORROW_DETAIL> BorrowDetails { get; set; } = new List<BORROW_DETAIL>();

    }
}
