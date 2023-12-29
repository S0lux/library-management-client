using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json;
using SkiaSharp;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class HomeViewModel: ViewModelBase
{
    private readonly AuthenticationService _authenticationService;
    [ObservableProperty] private AuthUser? _currentUser;
    [ObservableProperty] private ObservableCollection<MEMBER> _members;
    [ObservableProperty] private ObservableCollection<string> _labels = new ObservableCollection<string>();
    [ObservableProperty] private int _availableBooks;
    [ObservableProperty] private int _borrowedBooks;
    [ObservableProperty] private int _damagedBooks;
    [ObservableProperty] private int _lostBooks;
    [ObservableProperty] private ObservableCollection<Axis> _xAxis = new ObservableCollection<Axis>
    {
        new Axis
        {
            Labels = new ObservableCollection<string>()
        }
    };
    [ObservableProperty] private ObservableCollection<ISeries> _series = new ObservableCollection<ISeries> {
        new LineSeries<int>
        {
            Values = new ObservableCollection<int>(),
            Fill = null
        }
    };

    public HomeViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        CurrentUser = authenticationService.CurrentUser;
        FetchMembers();
        UpdateCounts();
    }

    public void UpdateChart()
    {
        var sortedMembers = Members.OrderBy(member => member.JoinDate);
        
        foreach (var member in sortedMembers)
        {
            if (!Labels.Contains(member.JoinDate.Date.ToString("dd/MM/yyyy")))
            {
                Labels.Add(member.JoinDate.Date.ToString("dd/MM/yyyy"));
            }
        }

        var groupByDate = sortedMembers.GroupBy(member => member.JoinDate.Date);
        ObservableCollection<int> values = new ObservableCollection<int>();

        foreach (var grouping in groupByDate)
        {
            values.Add(values.LastOrDefault() + grouping.Count());
        }
        
        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<int>
            {
                Values = values,
                Stroke = new SolidColorPaint(SKColors.RoyalBlue, 2.5F),
                Fill = new LinearGradientPaint(SKColors.RoyalBlue, SKColors.RoyalBlue.WithAlpha(0), new SKPoint(0.5F,0), new SKPoint(0.5F,1)),
                GeometryFill = new SolidColorPaint(SKColors.DodgerBlue),
                GeometryStroke = new SolidColorPaint(SKColors.RoyalBlue, 2.5F)
            }
        };
        
        XAxis = new ObservableCollection<Axis>
        {
            new Axis
            {
                Labels = Labels, 
            }
        };
    }

    public async Task FetchMembers()
    {
        var response = await _authenticationService.GetAsync("/api/members");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<ApiResponseMember>(jsonString);
            Members = deserialized.data;
            UpdateChart();
        }
    }

    public async Task UpdateCounts()
    {
        var response = await _authenticationService.GetAsync("/api/book_details");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var deserializedObj = JsonConvert.DeserializeObject<ApiResBookDetail>(responseJson);
            var data = deserializedObj.data;

            AvailableBooks = BorrowedBooks = DamagedBooks = LostBooks = 0;

            foreach (var bookDetail in data)
            {
                if (bookDetail.Status == "normal") AvailableBooks += bookDetail.Quantity is int value ? value : 0;
                if (bookDetail.Status == "borrowed") BorrowedBooks += bookDetail.Quantity is int value ? value : 0;
                if (bookDetail.Status == "damaged") DamagedBooks += bookDetail.Quantity is int value ? value : 0;
                if (bookDetail.Status == "lost") LostBooks += bookDetail.Quantity is int value ? value : 0;
            }
        }
    }
}