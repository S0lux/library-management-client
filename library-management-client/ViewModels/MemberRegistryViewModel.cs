

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class MemberRegistryViewModel: ViewModelBase
{
    public MEMBER member { get; set; }

    [ObservableProperty] private string _firstName;
    [ObservableProperty] private string _lastName;
    [ObservableProperty] private string _phoneNumber;
    [ObservableProperty] private string _citizenID;
    [ObservableProperty] private DateTime _birthDate=DateTime.Today;
    [ObservableProperty] private string _gender="";
    [ObservableProperty] private string _fullAddress;

    public ObservableCollection<string> Genders
    {
        get
        {
            var gender = new ObservableCollection<string>() { "Male", "Female" };
            Gender = gender.FirstOrDefault();
            return gender;
        }
    }

    public MemberRegistryViewModel()
    {
        
    }

    [RelayCommand]
    void Submit()
    {
        member = new MEMBER(CitizenID,FirstName+" "+LastName ,FullAddress,PhoneNumber,Gender,BirthDate);
        Console.WriteLine(FirstName+LastName+PhoneNumber+CitizenID+BirthDate.ToString("dd/MM/yyyy")+Gender+FullAddress);
    }
}