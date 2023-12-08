using System.ComponentModel.DataAnnotations;

namespace Avalonia_DependencyInjection.Models;

public class ACCOUNT
{
    [Key]
    public int AccountId { get; set; }

    [MaxLength(20)]
    public string UserName { get; set; }

    [MaxLength(20)]
    public string Password { get; set; }

    public int AccessLevel { get; set; }

    public int EmployeeId { get; set; }
    public EMPLOYEE Employee { get; set; }
}