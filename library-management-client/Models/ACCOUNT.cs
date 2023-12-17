using System.ComponentModel.DataAnnotations;

namespace Avalonia_DependencyInjection.Models;

public class ACCOUNT
{
    [Key]
    public int AccountID { get; set; }

    [MaxLength(20)]
    public string Username { get; set; }

    [MaxLength(20)]
    public string Password { get; set; }

    public int AccessLevel { get; set; }

    public int OwnerID { get; set; }
}