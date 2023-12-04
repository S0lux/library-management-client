using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;

namespace Avalonia_DependencyInjection.Interfaces;

public interface IAuthService
{
    Task<AuthUser> LoginAsync(string username, string password, bool remember);
    Task<bool> VerifyTokenAsync();
}