using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryFormViewModel : ViewModelBase
{
    private readonly AuthenticationService _authService;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0 };

    [ObservableProperty] private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };

    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private string? _errorMessage="something";

    public MemberRegistryFormViewModel(AuthenticationService authService)
    {
        _authService = authService;

        InputedMember.PropertyChanged += (sender, args) => { SubmitCommand.NotifyCanExecuteChanged(); };
    }

    bool checksubmit()
    {
        return !string.IsNullOrEmpty(InputedMember.Name)&&
               !string.IsNullOrEmpty(InputedMember.PhoneNumber)&&
               !string.IsNullOrEmpty(InputedMember.CitizenID)&&
               !string.IsNullOrEmpty(InputedMember.Address)&&
               UInt32.TryParse((ReadOnlySpan<char>)InputedMember.PhoneNumber, out UInt32 temp)&&
               UInt32.TryParse((ReadOnlySpan<char>)InputedMember.CitizenID,out temp);
    }

    [RelayCommand(CanExecute = nameof(checksubmit))]
    async Task Submit()
    {
        var createdMember = new
        {
            Credit = 0,
            Name = InputedMember.Name,
            PhoneNumber = InputedMember.PhoneNumber,
            CitizenID = InputedMember.CitizenID,
            Gender = InputedMember.Gender,
            DateOfBirth = InputedMember.DateOfBirth.ToString("o"),
            Address = InputedMember.Address,
            EmployeeID = _authService.CurrentUser!.EmployeeID
        };
        
        var loginData = new
        {
            data = createdMember
        };
        
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _authService.PostAsync("/api/members", content);
        if(response.StatusCode==HttpStatusCode.Unauthorized) HasError=true;
        if (response.StatusCode == HttpStatusCode.ServiceUnavailable) HasError = true;
    }


    [RelayCommand]
    void Cancel()
    {
        var win = App.AppHost.Services.GetRequiredService<MemberRegistryForm>();
        win.Hide();
    }

    [RelayCommand]
    void AlertBoxOff()
    {
        HasError = false;
    }
}