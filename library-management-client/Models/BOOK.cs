using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Avalonia_DependencyInjection.Models
{
    internal class BOOK
    {
        [Key]
        public int BookID { get; set; }

        [MaxLength(13)]
        public string ISBN { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Author { get; set; }

        [MaxLength(30)]
        public string Genre { get; set; }

        public double Price { get; set; }

        public DateTime ReleaseDate { get; set; }

    }
}
