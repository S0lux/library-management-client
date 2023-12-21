using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models
{
    public partial class MEMBER: ObservableValidator
    {
        [Key] 
        public int MemberID { get; set; }

        [ObservableProperty] [NotifyDataErrorInfo] [Required] [Phone] [MaxLength(10)] private string _citizenID;

        [ObservableProperty] [NotifyDataErrorInfo] [Required] private string _name;

        [ObservableProperty] [NotifyDataErrorInfo] [Required] private string _address;

        [ObservableProperty] [NotifyDataErrorInfo] [Required] [Phone] private string _phoneNumber;

        [ObservableProperty] private int _credit;

        [ObservableProperty] private DateTime _dateOfBirth;

        [ObservableProperty] private int _gender;

        [ObservableProperty] private int _employeeID;
    }
}
