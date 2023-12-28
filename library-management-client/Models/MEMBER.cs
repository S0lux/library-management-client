using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models
{
    public partial class MEMBER: ObservableValidator
    {
        [Key] 
        public int MemberID { get; set; }

        [ObservableProperty] [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "This information is required")] 
        [Phone(ErrorMessage = "Characters are not allowed")] 
        private string _citizenID;

        [ObservableProperty] [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "This information is required")]  
        private string _name;

        [ObservableProperty] [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "This information is required")]
        private string _address;

        [ObservableProperty] [NotifyDataErrorInfo]
        [Required(ErrorMessage = "This information is required")] 
        [Phone(ErrorMessage = "Characters are not allowed")] 
        private string _phoneNumber;

        [ObservableProperty] private int _credit;

        [ObservableProperty] private DateTime _dateOfBirth;

        [ObservableProperty] private int _gender;

        [ObservableProperty] private int _employeeID;
        
        [ObservableProperty] private bool _deleted;
    }
}
