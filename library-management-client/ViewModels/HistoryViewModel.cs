using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class HistoryViewModel: ViewModelBase
{
    private AuthenticationService _authService;
    
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _filterKey;
    [ObservableProperty] private ObservableCollection<HISTORY> _historyList;

    public HistoryViewModel(AuthenticationService authService)
    {
        _authService = authService;
        GetData();
    }

    [RelayCommand]
    async Task GetData()
    {
        var response = await _authService.GetAsync("/api/history");
        if (!response.IsSuccessStatusCode)
        {
            // Show error dialog and stop function
            var mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
            var alertBox = new MyMessageBox("Đã có lỗi xảy ra khi kết nối đến server", "Đã gặp sự cố!",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await alertBox.ShowDialog(mainWindow);
            return;
        }
        
        // Success status code then deserialize and set HistoryList
        var responseString = await response.Content.ReadAsStringAsync();
        var deserializedObject = JsonConvert.DeserializeObject<APIHistoryResponse>(responseString);

        if (deserializedObject?.data.Length > 0)
        {
            HistoryList = new ObservableCollection<HISTORY>(deserializedObject.data);
        }
    }
}

public class APIHistoryResponse
{
    public HISTORY[] data;
}