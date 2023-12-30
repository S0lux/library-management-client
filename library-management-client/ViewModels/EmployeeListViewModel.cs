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

                EmployeeList = new ObservableCollection<EMPLOYEE>(apiResponseEmployee!.data);
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
            var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<MemberRegistryFormViewModel>();
            infoBoxViewModel.AlertBoxOff();

            infoBoxViewModel.InputedMember = new MEMBER() { DateOfBirth = DateTime.Today, Gender = 0, Deleted = false };
            infoBoxViewModel.Title = "Employee Info";

            infoBoxViewModel.InputedMember.PropertyChanged += (sender, args) =>
            {
                infoBoxViewModel.SubmitCommand.NotifyCanExecuteChanged();
            };

            var memberRegistryForm = App.AppHost!.Services.GetRequiredService<MemberRegistryForm>();
            memberRegistryForm.Show();
        }

        //[RelayCommand]
        //public async void Delete()
        //{
        //    MyMessageBox myMessageBox = new MyMessageBox("Are you sure you want to delete this member?", "Confirm",
        //        MyMessageBox.MessageBoxButton.YesNo,
        //        MyMessageBox.MessageBoxImage.Question
        //    );

        //    await myMessageBox.ShowDialog(App.AppHost!.Services.GetRequiredService<MainWindow>());

        //    if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        //    {
        //        var response = await _authService.DeleteAsync($"/api/members/{selectedMember.MemberID}");
        //        GetData();
        //    }
        //}

        [RelayCommand]
        public async void Info()
        {
            var infoBoxViewModel = App.AppHost!.Services.GetRequiredService<MemberRegistryFormViewModel>();
            infoBoxViewModel.AlertBoxOff();

            infoBoxViewModel.Title = "Employee Info";
            infoBoxViewModel.InputedMember.CitizenID = selectedEmployee.CitizenID;
            infoBoxViewModel.InputedMember.Address = selectedEmployee.Address;
            infoBoxViewModel.InputedMember.Name = selectedEmployee.Name;
            infoBoxViewModel.InputedMember.PhoneNumber = selectedEmployee.PhoneNumber;
            infoBoxViewModel.InputedMember.Gender = selectedEmployee.Gender;
            infoBoxViewModel.InputedMember.EmployeeID = selectedEmployee.EmployeeID;

            var infoBox = App.AppHost!.Services.GetRequiredService<MemberRegistryForm>();

            infoBox.Show();
        }
    }

    public class ApiResponseEmployee
    {
        public ObservableCollection<EMPLOYEE> data { get; set; }
    }
}

