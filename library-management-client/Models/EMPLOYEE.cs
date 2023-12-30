using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models;

public partial class EMPLOYEE: ObservableValidator
{
    [Key]
    public int EmployeeID { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    private string _name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    private string _address;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    [Phone(ErrorMessage = "Characters are not allowed")]
    private string _phoneNumber;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    [Phone(ErrorMessage = "Characters are not allowed")]
    private string _citizenID;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    private string _email;

    [ObservableProperty] private int _gender;

    [ObservableProperty] private DateTime _dateOfBirth;

    [ObservableProperty] private bool _deleted;
}