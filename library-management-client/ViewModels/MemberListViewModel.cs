using Avalonia_DependencyInjection.Converters;
using Avalonia_DependencyInjection.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

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
        memberList.Add(new MEMBER
        {
            MemberId = 1,
            CitizenID = "1",
            Credit = 1,
            Name = "Trng Nguyn Trung Khang",
            Address = "A",
            PhoneNum = "1",
            DateOfBirth = DateTime.Now,
            Gender = 0,
            EmployeeId = 1
        });
    }
}