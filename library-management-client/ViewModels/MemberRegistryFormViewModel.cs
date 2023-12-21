using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryFormViewModel : ViewModelBase
{
    private readonly AuthenticationService _authService;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0 };

    [ObservableProperty] private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };
    
    [ObservableProperty] private string _iconPathExit = "/Assets/SVGs/xmark-royalblue.svg";

    [ObservableProperty] private bool _hasError=false;
    [ObservableProperty] private string? _errorMessage="something";
    [ObservableProperty] private bool _notifySuccess=false;

    public MemberRegistryFormViewModel(AuthenticationService authService)
    {
        _authService = authService;

        InputedMember.PropertyChanged += (sender, args) => { SubmitCommand.NotifyCanExecuteChanged(); };
    }

    bool checkSubmit()
    {
        return !string.IsNullOrEmpty(InputedMember.Name)&&
               !string.IsNullOrEmpty(InputedMember.PhoneNumber)&&
               !string.IsNullOrEmpty(InputedMember.CitizenID)&&
               !string.IsNullOrEmpty(InputedMember.Address)&&
               UInt32.TryParse((ReadOnlySpan<char>)InputedMember.PhoneNumber, out UInt32 temp)&&
               UInt32.TryParse((ReadOnlySpan<char>)InputedMember.CitizenID,out temp);
    }

    [RelayCommand(CanExecute = nameof(checkSubmit))]
    async Task Submit()
    {
        var addMemberWindow = App.AppHost!.Services.GetRequiredService<MemberListViewModel>();

        if (!addMemberWindow.MemberList.Any(e => e.MemberID == InputedMember.MemberID))
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
            if (response.StatusCode == HttpStatusCode.Unauthorized) HasError = true;
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable) HasError = true;

            addMemberWindow.GetData();
        }
        else
        {
            try
            {
                var createdMember = new
                {
                    MemberID = InputedMember.MemberID,
                    Credit = InputedMember.Credit,
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

                var response = await _authService.PutAsync("/api/members", content);

                addMemberWindow.GetData();
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException(e.ToString());
            }
        }

        var win = App.AppHost.Services.GetRequiredService<MemberRegistryForm>();
        win.Hide();
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
        NotifySuccess = false;
    }
}
