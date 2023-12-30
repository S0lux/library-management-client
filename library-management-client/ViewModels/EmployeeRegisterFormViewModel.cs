using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.ViewModels
{
    public partial class EmployeeRegisterFormViewModel:ViewModelBase
    {
        [ObservableProperty] private string _title = "Member registration";

        [ObservableProperty] private string _ico = @"/Assets/SVGs/user-plus-black.svg";

        private readonly AuthenticationService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private EMPLOYEE _inputedEmployee = new EMPLOYEE() { DateOfBirth = DateTime.Today, Gender = 0 };

        [ObservableProperty]
        private ObservableCollection<string> _genders = new ObservableCollection<string>()
        { "Male", "Female" };

        [ObservableProperty] private string _iconPathExit = "/Assets/SVGs/xmark-royalblue.svg";

        [ObservableProperty] private bool _hasError = false;
        [ObservableProperty] private string? _errorMessage;

        public EmployeeRegisterFormViewModel(AuthenticationService authService)
        {
            _authService = authService;

            InputedEmployee.PropertyChanged += (sender, args) => { SubmitCommand.NotifyCanExecuteChanged(); };
        }

        bool checkSubmit()
        {
            return !string.IsNullOrEmpty(InputedEmployee.Name) &&
                   !string.IsNullOrEmpty(InputedEmployee.PhoneNumber) &&
                   !string.IsNullOrEmpty(InputedEmployee.CitizenID) &&
                   !string.IsNullOrEmpty(InputedEmployee.Email) &&
                   !string.IsNullOrEmpty(InputedEmployee.Address) &&
                   UInt64.TryParse((ReadOnlySpan<char>)InputedEmployee.PhoneNumber, out var temp) &&
                   UInt64.TryParse((ReadOnlySpan<char>)InputedEmployee.CitizenID, out temp);
        }

        [RelayCommand(CanExecute = nameof(checkSubmit))]
        async Task Submit()
        {
            ErrorMessage = string.Empty;
            var addEmployeeWindow = App.AppHost!.Services.GetRequiredService<EmployeeListViewModel>();

            if (addEmployeeWindow.EmployeeList.All(e => e.EmployeeID != InputedEmployee.EmployeeID))
            {
                var createdMember = new
                {
                    Name = InputedEmployee.Name,
                    PhoneNumber = InputedEmployee.PhoneNumber,
                    CitizenID = InputedEmployee.CitizenID,
                    Gender = InputedEmployee.Gender,
                    DateOfBirth = InputedEmployee.DateOfBirth.ToString("o"),
                    Address = InputedEmployee.Address,
                    Email = InputedEmployee.Email,
                    Deleted = InputedEmployee.Deleted,
                };

                var payload = new
                {
                    data = createdMember
                };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _authService.PostAsync("/api/employees", content);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HasError = true;
                    ErrorMessage = "Unauthorized action";
                    return;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    HasError = true;
                    ErrorMessage = "An error has occured";
                    return;
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("New member created", "Success",
                        MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information, 350, 150);
                    await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>());
                }

                addEmployeeWindow.GetData();

            }
            else
            {
                var createdMember = new
                {
                    EmployeeID = InputedEmployee.EmployeeID,
                    Name = InputedEmployee.Name,
                    PhoneNumber = InputedEmployee.PhoneNumber,
                    CitizenID = InputedEmployee.CitizenID,
                    Gender = InputedEmployee.Gender,
                    DateOfBirth = InputedEmployee.DateOfBirth.ToString("o"),
                    Address = InputedEmployee.Address,
                    Deleted = InputedEmployee.Deleted,
                };

                var payload = new
                {
                    data = createdMember
                };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _authService.PutAsync("/api/employees", content);

                addEmployeeWindow.GetData();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HasError = true;
                    ErrorMessage = "Unauthorized action";
                    return;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    HasError = true;
                    ErrorMessage = "An error has occured";
                    return;
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Selected employee updated", "Success",
                        MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Information, 350, 150);
                    await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>());
                }
                Console.WriteLine(response.StatusCode);
            }

            await Task.Run(() => Thread.Sleep(500));
            AlertBoxOff();
            await Task.Run(() => Thread.Sleep(50));
            var win = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>();
            win.Close();
        }


        [RelayCommand]
        void Cancel()
        {
            var win = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>();
            win.Close();
        }

        [RelayCommand]
        public void AlertBoxOff()
        {
            HasError = false;
        }
    }
}
