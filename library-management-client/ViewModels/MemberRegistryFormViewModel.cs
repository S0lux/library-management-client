using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
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
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _buttonString;
    [ObservableProperty] private string _ico;

    private readonly AuthenticationService _authService;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private MEMBER _inputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0 };

    [ObservableProperty] private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Nam", "Nữ" };

    [ObservableProperty] private string _iconPathExit = "/Assets/SVGs/xmark-royalblue.svg";
    
    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private string? _errorMessage;

    public MemberRegistryFormViewModel(AuthenticationService authService)
    {
        _authService = authService;

        InputedMember.PropertyChanged += (sender, args) => { SubmitCommand.NotifyCanExecuteChanged(); };
    }

    bool checkSubmit()
    {
        return !string.IsNullOrEmpty(InputedMember.Name) &&
               !string.IsNullOrEmpty(InputedMember.PhoneNumber) &&
               !string.IsNullOrEmpty(InputedMember.CitizenID) &&
               !string.IsNullOrEmpty(InputedMember.Address) &&
               UInt64.TryParse((ReadOnlySpan<char>)InputedMember.PhoneNumber, out var temp) &&
               UInt64.TryParse((ReadOnlySpan<char>)InputedMember.CitizenID, out temp);
    }

    [RelayCommand(CanExecute = nameof(checkSubmit))]
    async Task Submit()
    {
        ErrorMessage = string.Empty;
        var addMemberWindow = App.AppHost!.Services.GetRequiredService<MemberListViewModel>();
        
        if (addMemberWindow.MemberList.All(e => e.MemberID != InputedMember.MemberID))
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
                EmployeeID = _authService.CurrentUser!.EmployeeID,
                Deleted = false
            };
            
            var payload = new
            {
                data = createdMember
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _authService.PostAsync("/api/members", content);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HasError = true;
                ErrorMessage = "Chưa được cấp quyền";
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                HasError = true;
                ErrorMessage = "Đã có lỗi xảy ra";
                return;
            }
            else
            {
                MyMessageBox mess = new MyMessageBox("Tạo mới thành công", "Thành công",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information,350,150);
                await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<MemberRegistryForm>());
            }
            addMemberWindow.GetData();
        }
        else
        {
            var createdMember = new
            {
                MemberID = InputedMember.MemberID,
                Credit = 0,
                Name = InputedMember.Name,
                PhoneNumber = InputedMember.PhoneNumber,
                CitizenID = InputedMember.CitizenID,
                Gender = InputedMember.Gender,
                DateOfBirth = InputedMember.DateOfBirth.ToString("o"),
                Address = InputedMember.Address,
                EmployeeID = _authService.CurrentUser!.EmployeeID,
                Deleted = InputedMember.Deleted
            };
            
            var payload = new
            {
                data = createdMember
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _authService.PutAsync("/api/members", content);
            addMemberWindow.GetData();
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HasError = true;
                ErrorMessage = "Chưa được cấp quyền";
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                HasError = true;
                ErrorMessage = "Đã có lỗi xảy ra";
                return;
            }
            else
            {
                MyMessageBox mess = new MyMessageBox("Cập nhật thành công", "Thành công",
                    MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information,350,150);
                await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<MemberRegistryForm>());
            }
        }

        await Task.Run(() => Thread.Sleep(500));
        AlertBoxOff();
        await Task.Run(() => Thread.Sleep(50));
        var win = App.AppHost.Services.GetRequiredService<MemberRegistryForm>();
        win.Close();
    }


    [RelayCommand]
    void Cancel()
    {
        var win = App.AppHost!.Services.GetRequiredService<MemberRegistryForm>();
        win.Close();
    }

    [RelayCommand]
    public void AlertBoxOff()
    {
        HasError = false;
    }
}