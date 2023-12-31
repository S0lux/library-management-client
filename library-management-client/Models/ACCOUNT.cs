using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Avalonia_DependencyInjection.Models;

public partial class ACCOUNT: ObservableValidator
{
    [Key]
    public int AccountID { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    private string _username;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "This information is required")]
    private string _password;

    public int AccessLevel { get; set; }

    public int OwnerID { get; set; }
}