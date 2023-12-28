using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class BookInfoViewModel:ViewModelBase
{
    private readonly AuthenticationService _authService;
    [ObservableProperty] private string text="hello";
    [ObservableProperty] private string _iconPathExit = "/Assets/SVGs/xmark-royalblue.svg";
    [ObservableProperty] private BOOK _book;
    [ObservableProperty] private int? _normal;
    [ObservableProperty] private int? _damaged;
    [ObservableProperty] private int? _lost;
    [ObservableProperty] private int? _borrowed;
    [ObservableProperty] private string _imageUrl;

    public ApiResBookDetail? apiResponseMember;

    public BookInfoViewModel(AuthenticationService authenticationService)
    {
        _authService = authenticationService;
    }
    
    [RelayCommand]
    void Cancel()
    {
        var win = App.AppHost!.Services.GetRequiredService<BookInfoView>();
        win.Hide();
    }

    public async Task GetData()
    {
        try
        {
            //var response = await _authService.GetAsync(@"/api/book_details");
            //response.EnsureSuccessStatusCode();

            //var body = await response.Content.ReadAsStringAsync();
            //apiResponseMember = JsonConvert.DeserializeObject<ApiResBookDetail>(body);

            //var retrievedBookDetail = new ObservableCollection<BOOK_DETAIL>(apiResponseMember!.data.Where(e=> e.ISBN13==Book.ISBN13));
            //foreach (BOOK_DETAIL bt in retrievedBookDetail)
            //{
            //    switch (bt.Status)
            //    {
            //        case "normal": Normal = bt.Quantity;
            //            break;
            //        case "damaged": Damaged = bt.Quantity;
            //            break;
            //        case "lost": Lost = bt.Quantity;
            //            break;
            //        case "borrowed": Borrowed = bt.Quantity;
            //            break;
            //    }
            //}

            Normal = Book.BOOK_DETAILs.First(e => e.Status == "normal").Quantity;
            Damaged = Book.BOOK_DETAILs.First(e => e.Status == "damaged").Quantity;
            Lost = Book.BOOK_DETAILs.First(e => e.Status == "lost").Quantity;
            Borrowed = Book.BOOK_DETAILs.First(e => e.Status == "borrowed").Quantity;
        }
        catch (HttpRequestException e)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Error", "Add Member Failed!",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();
        }
    }
}

public class ApiResBookDetail
{
    public ObservableCollection<BOOK_DETAIL> data { get; set; }
}