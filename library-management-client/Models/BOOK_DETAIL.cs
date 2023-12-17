using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.Models
{
    internal class BOOK_DETAIL
    {
        [MaxLength(4)]
        public int BookID { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }

        public int Quantity { get; set; }
    }
}
