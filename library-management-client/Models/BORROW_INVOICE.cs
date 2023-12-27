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
        public DateTime BorrowingDate { get; set; }
        
        public List<BORROW_DETAIL> BorrowDetails { get; set; }
        public List<MEMBER> Member { get; set; }
        public List<EMPLOYEE> Employee { get; set; }
    }
}
