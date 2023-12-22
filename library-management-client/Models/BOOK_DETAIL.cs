using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    public class BOOK_DETAIL
    {
        public string ISBN13 { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }

        public int Quantity { get; set; }
    }
}
