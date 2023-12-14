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
using Tmds.DBus.Protocol;
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
        // Prepare string content
        var loginData = new
        {
            username = username,
            password = password,
        };
        
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var httpClient = _factory.CreateClient("main");
            var response = await httpClient.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            CurrentUser = JsonSerializer.Deserialize<AuthUser>(body);
            CurrentUser!.remember = remember;

            // Save the received token locally
            const string filePath = "userToken.txt";
            await File.WriteAllTextAsync(filePath, CurrentUser.token);

            return CurrentUser;
        }
        catch (HttpRequestException e)
        {
            switch (e.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new Exception("Incorrect username/password");
                default:
                    throw new Exception("Unable to connect to server");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Unhandled error: " + e.Message);
        }
    }

    public async Task<bool> VerifyTokenAsync()
    {
        // Request to /api/session/verify
        var response = await GetAsync("/api/session/verify/");
        
        var body = await response.Content.ReadAsStringAsync();
        CurrentUser = JsonSerializer.Deserialize<AuthUser>(body);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }
        
        return true;
    }

    public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
    {
        // Check for token saved locally
        if (!File.Exists("userToken.txt")) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        
        var httpClient = _factory.CreateClient("main");
        
        // Set the authorization header
        var localToken = await File.ReadAllTextAsync("userToken.txt");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", localToken);

        try
        {
            var response = await httpClient.PostAsync(uri, content);

            // To handle redirects
            // Due to the authorization header being lost on redirection
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var finalUri = response.RequestMessage!.RequestUri;
                var res = await httpClient.PostAsync(finalUri, content);

                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Delete userToken.txt
                    File.Delete("userToken.txt");
                }

                return res;
            }

            return response;
        }
        catch (HttpRequestException e)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        }
        catch (Exception)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string uri)
    {
        // Check for token saved locally
        if (!File.Exists("userToken.txt")) return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        
        var httpClient = _factory.CreateClient("main");
        
        // Set the authorization header
        var localToken = await File.ReadAllTextAsync("userToken.txt");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", localToken);
        
        try
        {
            var response = await httpClient.GetAsync(uri);

            // To handle redirects
            // Due to the authorization header being lost on redirection
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var finalUri = response.RequestMessage!.RequestUri;
                var res = await httpClient.GetAsync(finalUri);

                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Delete userToken.txt
                    File.Delete("userToken.txt");
                }

                return res;
            }

            return response;
        }
        catch (HttpRequestException e)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        }
        catch (Exception)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}