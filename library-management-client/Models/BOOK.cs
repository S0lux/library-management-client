using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.Models
{
    public class BOOK
    {
        [JsonPropertyName("isbn_10")]
        public string ISBN10 { get; set; }
        
        [JsonPropertyName("isbn_13")]
        public string ISBN13 { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("publish_date")]
        public DateTime PublishDate { get; set; }

    }
}
