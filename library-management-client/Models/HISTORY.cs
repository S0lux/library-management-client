using System;
using System.Runtime.InteropServices.JavaScript;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_DependencyInjection.Models;

public partial class HISTORY: ObservableObject
{
    [ObservableProperty] private int _historyID;
    [ObservableProperty] private DateTime _date;
    [ObservableProperty] private string _action;
    [ObservableProperty] private string _actionDetails;
    [ObservableProperty] private string _accountUsername;
}