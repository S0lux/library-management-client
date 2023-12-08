using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Avalonia_DependencyInjection.Models;

public class EMPLOYEE
{
    [Key]
    public int EmployeeId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Address { get; set; }

    [MaxLength(11)]
    public string PhoneNum { get; set; }

    public int Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public ICollection<MEMBER> Members { get; set; } = new List<MEMBER>();
}