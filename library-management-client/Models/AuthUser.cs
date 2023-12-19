using System;

namespace Avalonia_DependencyInjection.Models;

public class AuthUser
{
    // Session properties
    public string token { get; set; }
    public bool remember { get; set; }
    
    // Employee properties
    public int EmployeeID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Gender { get; set; }
    public string Email { get; set; }
    public string CitizenID { get; set; }
}
