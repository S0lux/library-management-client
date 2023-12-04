using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Interfaces;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Avalonia_DependencyInjection.Services;

public class AuthenticationService: IAuthService
{
    public AuthUser? CurrentUser { get; private set; }
    private readonly IHttpClientFactory _factory;

    public AuthenticationService(IHttpClientFactory httpClientFactory)
    {
        _factory = httpClientFactory;
    }

    public async Task<AuthUser> LoginAsync(string username, string password, bool remember)
    {
        var httpClient = _factory.CreateClient("main");
        
        var loginData = new
        {
            username = username,
            password = password,
        };
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("/api/login/", content);
            var body = await response.Content.ReadAsStringAsync();

            CurrentUser = response.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer.Deserialize<AuthUser>(body)!,
                HttpStatusCode.BadRequest => throw new Exception("Bad request"),
                HttpStatusCode.Unauthorized => throw new Exception("Invalid credentials"),
                _ => throw new Exception(response.StatusCode.ToString())
            };

            CurrentUser.remember = remember;

            // Save the received token locally
            const string filePath = "userToken.txt";
            await File.WriteAllTextAsync(filePath, CurrentUser.token);
            
            return CurrentUser;
        }
        catch (HttpRequestException)
        {
            throw new Exception("Unable to connect to database");

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    public async Task<bool> VerifyTokenAsync()
    {
        if (!File.Exists("userToken.txt")) return false;
        
        var httpClient = _factory.CreateClient("main");
        
        // Set the authorization header
        var localToken = await File.ReadAllTextAsync("userToken.txt");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", localToken);
        
        // Request to /api/session/verify
        var response = await httpClient.GetAsync("/api/session/verify/");

        // Request will be redirected
        // Leading to the loss of Authorization header
        // This will resend the request to the redirected Uri
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var finalUri = response.RequestMessage!.RequestUri;
            var res = await httpClient.GetAsync(finalUri);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Delete userToken.txt
                File.Delete("userToken.txt");
                SwitchToLoginScreen();
                return false;
            }
            
            var body = await res.Content.ReadAsStringAsync();
            
            CurrentUser = JsonSerializer.Deserialize<AuthUser>(body);
            return true;
        }
        
        return false;
    }

    private void SwitchToLoginScreen()
    {
        var currentViewModel = App.AppHost!.Services.GetRequiredService<MainWindowViewModel>();
        currentViewModel.ContentViewModel = App.AppHost!.Services.GetRequiredService<LoginViewModel>();
    }
}