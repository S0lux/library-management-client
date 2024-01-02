using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    async Task Update()
    {
        MyMessageBox error = new MyMessageBox(
            "Are you sure you want to update this book?",
            "Confirmation",
            MyMessageBox.MessageBoxButton.YesNo,
            MyMessageBox.MessageBoxImage.Question
        );
        await error.ShowDialog(App.AppHost!.Services.GetRequiredService<BookInfoView>());
        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            var addvm = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
            var isbnvm=App.AppHost.Services.GetRequiredService<AddByISBNViewModel>();
            isbnvm.BookQuantity = Normal;
            isbnvm.ShelfNumber = Book.Shelf;
            addvm.AddBy = "ISBN";
            addvm.FindKey = Book.ISBN13;
            App.AppHost.Services.GetRequiredService<BookInfoView>().Hide();
            App.AppHost.Services.GetRequiredService<AddBookWindow>().Show();
            await addvm.Find();
        }
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