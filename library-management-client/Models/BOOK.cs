using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.Models
{
    public partial class BOOK:ObservableValidator
    {
        
        [JsonPropertyName("isbn_13")]
        public string ISBN13 { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("publish_date")]
        public DateTime PublishDate { get; set; }

        [ObservableProperty]
        public bool _isCheck;
    }
}
