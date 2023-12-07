using System.Collections.Generic;
using Avalonia;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia_DependencyInjection.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        var menuRegister = new List<SubItem>();
        menuRegister.Add(new SubItem("Customer"));
        menuRegister.Add(new SubItem("Providers"));
        menuRegister.Add(new SubItem("Employees"));
        menuRegister.Add(new SubItem("Products"));
        var item6 = new DropDownMenuViewModel("Register", menuRegister);

        var menuSchedule = new List<SubItem>();
        menuSchedule.Add(new SubItem("Services"));
        menuSchedule.Add(new SubItem("Meetings"));
        var item1 = new DropDownMenuViewModel("Appointments", menuSchedule);

        var menuReports = new List<SubItem>();
        menuReports.Add(new SubItem("Customers"));
        menuReports.Add(new SubItem("Providers"));
        menuReports.Add(new SubItem("Products"));
        menuReports.Add(new SubItem("Stock"));
        menuReports.Add(new SubItem("Sales"));
        var item2 = new DropDownMenuViewModel("Reports", menuReports);

        var menuExpenses = new List<SubItem>();
        menuExpenses.Add(new SubItem("Fixed"));
        menuExpenses.Add(new SubItem("Variable"));
        var item3 = new DropDownMenuViewModel("Expenses", menuExpenses);

        var menuFinancial = new List<SubItem>();
        menuFinancial.Add(new SubItem("Cash flow"));
        var item4 = new DropDownMenuViewModel("Financial", menuFinancial);

        var item0 = new DropDownMenuViewModel("Dashboard", new UserControl());

        Menu.Children.Add(new DropDownMenuView(item0));
        Menu.Children.Add(new DropDownMenuView(item6));
        Menu.Children.Add(new DropDownMenuView(item1));
        Menu.Children.Add(new DropDownMenuView(item2));
        Menu.Children.Add(new DropDownMenuView(item3));
        Menu.Children.Add(new DropDownMenuView(item4));
    }
}