using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddByTitleViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;
    
    [ObservableProperty] private ObservableCollection<BOOK>? _books;
    [ObservableProperty] private BOOK _selectedBook;

    public AddByTitleViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }
    
    public async Task RetrieveBooksByTitle(string title)
    {
        var response = await _authService.GetAsync($"/api/books/title/{title}");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            // Notify user
            // 404 -> No book found
            // 500 -> Unknown error
            return;
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<apiResponse>(responseBody);

        if (responseObj != null) Books = responseObj.data;
    }
}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}