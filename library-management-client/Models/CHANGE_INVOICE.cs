using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    internal class CHANGE_INVOICE
    {
        [Key]
        public int ChangeInvoiceID { get; set; }

        [MaxLength(4)]
        public string EmpID { get; set; }
        public EMPLOYEE Employee { get; set; }

        public DateTime ChangeDate { get; set; }

        public ICollection<CHANGE_DETAIL> ChangeDetails { get; set; } = new List<CHANGE_DETAIL>();

    }
}
