using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryFormViewModel:ViewModelBase
{
    private readonly AuthenticationService _authService;
    [ObservableProperty] private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0};
    [ObservableProperty]
    private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };
    
    public MemberRegistryFormViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }
    [RelayCommand]
    void Submit()
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
            EmployeeID = _authService.CurrentUser.EmployeeID
        };

        var loginData = new
        {
            data = createdMember
        };
        
        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response=_authService.PostAsync("/api/members", content);
        Console.WriteLine(_authService.CurrentUser.EmployeeID+"empl");
        
        //Win.Close();
    }

    [RelayCommand]
    void Cancel()
    {
        var win = App.AppHost.Services.GetRequiredService<MemberRegistryForm>();
        win.Hide();
    }
}