using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    [ObservableProperty] private ObservableCollection<string> _filterOptions = new ObservableCollection<string>()
        { "ALL","UPDATE", "CREATE", "DELETE" };

    [ObservableProperty] private ObservableCollection<HISTORY> _historyList=new();
    [ObservableProperty] private ObservableCollection<HISTORY> _historyFindList=new();
    [ObservableProperty] private ObservableCollection<HISTORY> _showingList=new();

    public HistoryViewModel(AuthenticationService authService)
    {
        _authService = authService;
        FilterKey = FilterOptions.FirstOrDefault();
        GetData();
    }

    partial void OnFilterKeyChanged(string value)
    {
        HistoryFindList.Clear();
        switch (value)
        {
            case "ALL":
            {
                ShowingList = HistoryList;
                return;
            }
            case "UPDATE":
            {
                foreach (HISTORY his in HistoryList)
                {
                    if(his.Action=="UPDATE") HistoryFindList.Add(his);
                }

                break;
            }
            case "CREATE":
            {
                foreach (HISTORY his in HistoryList)
                {
                    if(his.Action=="CREATE") HistoryFindList.Add(his);
                }

                break;
            }
            case "DELETE":
            {
                foreach (HISTORY his in HistoryList)
                {
                    if(his.Action=="DELETE") HistoryFindList.Add(his);
                }
                break;
            }
        }

        ShowingList = HistoryFindList;
    }

    [RelayCommand]
    async Task GetData()
    {
        var response = await _authService.GetAsync("/api/history_logs");
        if (!response.IsSuccessStatusCode)
        {
            // Show error dialog and stop function
            var mainWindow = App.AppHost!.Services.GetRequiredService<MainWindow>();
            var alertBox = new MyMessageBox("Đã có lỗi xảy ra khi kết nối đến máy chủ", "Lỗi!",
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
            ShowingList = HistoryList;
        }
    }
}

public class APIHistoryResponse
{
    public HISTORY[] data;
}