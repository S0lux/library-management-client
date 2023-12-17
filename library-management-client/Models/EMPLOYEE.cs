using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Avalonia_DependencyInjection.Models;

public class EMPLOYEE
{
    [Key]
    public int EmployeeID { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Address { get; set; }

    [MaxLength(11)]
    public string PhoneNumber { get; set; }
    
    public string CitizenID { get; set; }

    public string Email { get; set; }

    public int Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

}