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
        public string EmployeeID { get; set; }

        public DateTime ChangeDate { get; set; }


    }
}
