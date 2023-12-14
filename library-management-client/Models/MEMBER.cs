using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models
{
    public class MEMBER: ObservableObject
    {
        [Key]
        public int MemberId { get; set; }

        [MaxLength(12)]
        public string CitizenID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(11)]
        public string PhoneNum { get; set; }

        public int Credit { get; set; }

        public int Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(4)]
        public int EmployeeId { get; set; }
    }
}
