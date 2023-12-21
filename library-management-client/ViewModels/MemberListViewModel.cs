using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using DynamicData;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberListViewModel : ViewModelBase
{
    private readonly AuthenticationService _authService;
    
    public MEMBER selectedMember {get; set;}

    [ObservableProperty] private ObservableCollection<MEMBER> _memberList = new ObservableCollection<MEMBER>();
    [ObservableProperty] private bool _isBusy = false;

    public MemberListViewModel(AuthenticationService authService)
    {
        _authService = authService;
        GetData();
    }

    public async void GetData()
    {
        IsBusy = true;

        MemberList.Clear();

        await Task.Delay(1000);

        try
        {
            var response = await _authService.GetAsync(@"/api/members");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiResponseMember = JsonConvert.DeserializeObject<ApiResponseMember>(body);
            
            MemberList = apiResponseMember!.data;
        }
        catch (HttpRequestException e)
        {
            var box = MessageBoxManager
            .GetMessageBoxStandard("Error", "Add Member Failed!",
                ButtonEnum.YesNo);

            var result = await box.ShowAsync();
        }
        IsBusy = false;
    }

    [RelayCommand]
    public void Add()
    {
        var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<MemberRegistryFormViewModel>();

        infoBoxViewModel.InputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0 };

        infoBoxViewModel.InputedMember.PropertyChanged += (sender, args) => { infoBoxViewModel.SubmitCommand.NotifyCanExecuteChanged(); };

        var memberRegistryForm = App.AppHost!.Services.GetRequiredService<MemberRegistryForm>();
        memberRegistryForm.Show();
    }

    [RelayCommand]
    public async void Delete()
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Confirm", "Are you sure you want to delete this member?",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        
    }

    [RelayCommand]
    public async void Info()
    {
        var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<MemberRegistryFormViewModel>();

        infoBoxViewModel.InputedMember.CitizenID = selectedMember.CitizenID;
        infoBoxViewModel.InputedMember.Address = selectedMember.Address;
        infoBoxViewModel.InputedMember.Name = selectedMember.Name;
        infoBoxViewModel.InputedMember.PhoneNumber = selectedMember.PhoneNumber;
        infoBoxViewModel.InputedMember.Gender = selectedMember.Gender;
        infoBoxViewModel.InputedMember.Credit = selectedMember.Credit;
        infoBoxViewModel.InputedMember.MemberID = selectedMember.MemberID;
        infoBoxViewModel.InputedMember.EmployeeID = selectedMember.EmployeeID;

        var infoBox = App.AppHost!.Services.GetRequiredService<MemberRegistryForm>();

        infoBox.Show();
    }
}

public class ApiResponseMember
{
    public ObservableCollection<MEMBER> data { get; set; }
}