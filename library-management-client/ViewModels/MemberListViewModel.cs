using Avalonia_DependencyInjection.Converters;
using Avalonia_DependencyInjection.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using Avalonia_DependencyInjection.Views;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberListViewModel:ViewModelBase
{
    public ObservableCollection<MEMBER> memberList { get; set; }

    public MemberListViewModel() 
    {
        memberList = new ObservableCollection<MEMBER>();
    }

    [RelayCommand]
    public void Add()
    {
        var a = new MemberRegistryForm();
        a.Show();
        memberList.Add(new MEMBER
        {
            MemberID = 1,
            CitizenID = "1",
            Credit = 1,
            Name = "Trng Nguyn Trung Khang",
            Address = "A",
            PhoneNumber = "1",
            DateOfBirth = DateTime.Now,
            Gender = 0,
            EmployeeID = 1
        });
    }
}