using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    public class CHANGE_DETAIL
    {
        [MaxLength(6)]
        public int ChangeInvoiceID { get; set; }
        
        public string ISBN13 { get; set; }

        [MaxLength(50)]
        public string Partner { get; set; }

        public int Quantity { get; set; }

        public int ChangeType { get; set; }
    }
}
