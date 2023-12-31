using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_DependencyInjection.ViewModels
{
   public partial class EmployeeListViewModel:ViewModelBase
    {
        private readonly AuthenticationService _authService;

        public EMPLOYEE selectedEmployee { get; set; }

        [ObservableProperty] private ObservableCollection<EMPLOYEE> _employeeList = new ObservableCollection<EMPLOYEE>();
        [ObservableProperty] private ObservableCollection<EMPLOYEE> _employeeFindList = new ObservableCollection<EMPLOYEE>();
        [ObservableProperty] private ObservableCollection<EMPLOYEE> _showingList;

        [ObservableProperty] private bool _isBusy = false;

        [ObservableProperty] private string? _filterKey;
        [ObservableProperty] private string _filterBy;

        [ObservableProperty]
        private ObservableCollection<string> _filterByOptions = new ObservableCollection<string>()
        { "Name", "Employee ID", "Citizen ID" };


        public EmployeeListViewModel(AuthenticationService authService)
        {

            _authService = authService;
            GetData();
            FilterBy = FilterByOptions.FirstOrDefault();
        }

        partial void OnFilterKeyChanged(string? oldValue, string newValue)
        {
            EmployeeFindList.Clear();
            IsBusy = true;
            switch (FilterBy)
            {
                case "Name":
                    {
                        foreach (EMPLOYEE mem in EmployeeList)
                        {
                            if (string.IsNullOrEmpty(mem.Name.ToString()))
                            {
                                ShowingList = EmployeeList;
                            }

                            if (mem.Name.Contains(newValue.ToString()))
                            {
                                EmployeeFindList.Add(mem);
                            }
                        }

                        break;
                    }
                case "Employee ID":
                    {
                        foreach (EMPLOYEE mem in EmployeeList)
                        {
                            if (string.IsNullOrEmpty(newValue))
                            {
                                ShowingList = EmployeeList;
                            }

                            if (mem.EmployeeID.ToString().Contains(newValue.ToString()))
                            {
                                EmployeeFindList.Add(mem);
                            }
                        }

                        break;
                    }

                case "Citizen ID":
                    {
                        foreach (EMPLOYEE mem in EmployeeList)
                        {
                            if (string.IsNullOrEmpty(newValue))
                            {
                                ShowingList = EmployeeList;
                            }

                            if (mem.CitizenID.Contains(newValue.ToString()))
                            {
                                EmployeeFindList.Add(mem);
                            }
                        }

                        break;
                    }
            }

            IsBusy = false;
            ShowingList = EmployeeFindList;
        }

        [RelayCommand]
        public async Task GetData()
        {
            IsBusy = true;
            EmployeeList.Clear();
            try
            {
                var response = await _authService.GetAsync(@"/api/employees");
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var apiResponseEmployee = JsonConvert.DeserializeObject<ApiResponseEmployee>(body);

                EmployeeList = new ObservableCollection<EMPLOYEE>(apiResponseEmployee!.data.Where(e => e.Deleted == false)
                    .Select(e => new EMPLOYEE
                    {
                        Name = e.Name,
                        Gender = e.Gender,
                        Account = e.Account,
                        Address = e.Address,
                        PhoneNumber = e.PhoneNumber,
                        CitizenID = e.CitizenID,
                        DateOfBirth = e.DateOfBirth,
                        Deleted = e.Deleted,
                        EmployeeID = e.EmployeeID,
                        Email = e.Email,
                    }));

                ShowingList = EmployeeList;
            }
            catch (HttpRequestException e)
            {
                var box = MessageBoxManager
                    .GetMessageBoxStandard("Error", "Fetch Employeee Failed!",
                        ButtonEnum.YesNo);

                var result = await box.ShowAsync();
            }

            IsBusy = false;
        }

        [RelayCommand]
        public void Add()
        {
            var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormViewModel>();
            infoBoxViewModel.AlertBoxOff();

            infoBoxViewModel.InputedEmployee = new EMPLOYEE() { DateOfBirth = DateTime.Today, Gender = 0,Deleted = false };
            infoBoxViewModel.InputedEmployee.Account = new ACCOUNT();
            infoBoxViewModel.Title = "Employee Info";
            infoBoxViewModel.Ico = @"/Assets/SVGs/user-tie-solid.svg";

            infoBoxViewModel.InputedEmployee.PropertyChanged += (sender, args) =>
            {
                infoBoxViewModel.SubmitCommand.NotifyCanExecuteChanged();
            };

            infoBoxViewModel.InputedEmployee.Account.PropertyChanged += (sender, args) =>
            { 
                infoBoxViewModel.SubmitCommand.NotifyCanExecuteChanged();
            };
            var employeeRegistryForm = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>();
            employeeRegistryForm.Show();
        }

        [RelayCommand]
        public async void Delete()
        {
            MyMessageBox myMessageBox = new MyMessageBox("Are you sure you want to delete this staff?", "Confirm",
                MyMessageBox.MessageBoxButton.YesNo,
                MyMessageBox.MessageBoxImage.Question
            );

            await myMessageBox.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());

            if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
            {
                var response = await _authService.DeleteAsync($"/api/employees/{selectedEmployee.EmployeeID}");
                GetData();
            }
        }

        [RelayCommand]
        public async void Info()
        {
            var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormViewModel>();
            infoBoxViewModel.AlertBoxOff();

            infoBoxViewModel.Title = "Employee Info";
            infoBoxViewModel.InputedEmployee.CitizenID = selectedEmployee.CitizenID;
            infoBoxViewModel.InputedEmployee.Address = selectedEmployee.Address;
            infoBoxViewModel.InputedEmployee.Name = selectedEmployee.Name;
            infoBoxViewModel.InputedEmployee.PhoneNumber = selectedEmployee.PhoneNumber;
            infoBoxViewModel.InputedEmployee.Gender = selectedEmployee.Gender;
            infoBoxViewModel.InputedEmployee.EmployeeID = selectedEmployee.EmployeeID;
            infoBoxViewModel.InputedEmployee.Email = selectedEmployee.Email;
            infoBoxViewModel.InputedEmployee.Account = selectedEmployee.Account;
            infoBoxViewModel.InputedEmployee.DateOfBirth = selectedEmployee.DateOfBirth;

            var infoBox = App.AppHost!.Services.GetRequiredService<EmployeeRegisterFormView>();

            infoBox.Show();
        }
    }

    public class ApiResponseEmployee
    {
        public ObservableCollection<EMPLOYEE> data { get; set; }
    }
}

