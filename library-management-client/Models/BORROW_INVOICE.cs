using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    public class BORROW_INVOICE
    {
        [Key]
        public int BorrowInvoiceID { get; set; }

        [MaxLength(4)]
        public string MemberID { get; set; }

        [MaxLength(4)]
        public string EmployeeID { get; set; }

        public DateTime BorrowingDate { get; set; }
    }
}
