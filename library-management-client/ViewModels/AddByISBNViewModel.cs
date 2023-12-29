using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddByISBNViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;

    [ObservableProperty] private BOOK _book;

    [ObservableProperty] private string _releaseDate;
    [ObservableProperty] private string _imageUrl;
    [ObservableProperty] private bool _isCoverLoading;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddBookCommand))] private int? _bookQuantity;

    public AddByISBNViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        //BookDetail.PropertyChanged += ((sender, args) => { AddBookCommand.NotifyCanExecuteChanged(); });
    }

    partial void OnBookChanged(BOOK? oldValue, BOOK newValue)
    {
        if (Book != null)
        {
            ReleaseDate = Book.PublishDate.ToString("dd/MM/yyyy");
            ImageUrl = $"https://covers.openlibrary.org/b/isbn/{Book.ISBN13}-L.jpg";
        }
    }

    [RelayCommand(CanExecute = nameof(CheckAdd))]
    public async Task AddBook()
    {
        MyMessageBox confirmation = new MyMessageBox(
            "Are you sure about adding this book", "Confirmation",
            MyMessageBox.MessageBoxButton.YesNo, MyMessageBox.MessageBoxImage.Question,350,150);
        await confirmation.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());

        if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.YES)
        {
            var loadspiner = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
            loadspiner.IsBusy = true;
            //Attempt to register book by isbn 
            var createdBook = new
            {
                Title = Book.Title,
                Author = Book.Author,
                PublishDate = Book.PublishDate,
                ISBN13 = Book.ISBN13,
            };

            var response = await PostBookAsync(createdBook);
            var resultContentString = GetResultContentString(response.StatusCode);
            var resultBoxIcon = GetResultBoxIcon(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                await RefreshBookViewModel();
                await RegisterBookDetailAsync(Book.ISBN13,BookQuantity);
            }
            
            loadspiner.IsBusy = false;
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var resultBox = new MyMessageBox($"This book has already existed or has been deleated.\n" +
                                                 $"Do you want to add the additional {BookQuantity} books?\n" +
                                                 $"(The total will be {BookQuantity+Book.BOOK_DETAILs[0].Quantity})","Result",
                    MyMessageBox.MessageBoxButton.OkCancel, MyMessageBox.MessageBoxImage.Question,450,250);
                await resultBox.ShowDialog(App.AppHost.Services.GetRequiredService<AddBookWindow>());
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.OK)
                {
                    loadspiner.IsBusy = true;
                    var putResponse = await PutBookAsync(Book.ISBN13, false);
                    resultContentString = GetResultContentString(putResponse.StatusCode);
                    resultBoxIcon = GetResultBoxIcon(putResponse.StatusCode);
                    
                    if (putResponse.IsSuccessStatusCode)
                    {
                        await RefreshBookViewModel();
                        await RegisterBookDetailAsync(Book.ISBN13,Book.BOOK_DETAILs[0].Quantity,BookQuantity);
                    }
                    
                    ShowResultMessageBox(resultContentString, resultBoxIcon);
                    loadspiner.IsBusy = false;
                    var box1 = App.AppHost!.Services.GetRequiredService<BookViewModel>();
                    box1.BookCheckedList.Clear();
                    box1.CheckedAmount = 0;
                    box1.GetData();
                    return;
                }
            }
            ShowResultMessageBox(resultContentString, resultBoxIcon);

            var box = App.AppHost!.Services.GetRequiredService<BookViewModel>();
            box.BookCheckedList.Clear();
            box.CheckedAmount = 0;
            box.GetData();
        }
    }

    public async Task<int> RetrieveBookByISBN(string isbn)
    {
        try
        {
            var response = await _authenticationService.GetAsync($@"/api/books/isbn/{isbn}");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var apiRespondedBook = JsonConvert.DeserializeObject<ApiRespondedBook>(body);
            Book = apiRespondedBook.Data;
            
            response = await _authenticationService.GetAsync(@"/api/book_details");
            response.EnsureSuccessStatusCode();

            body = await response.Content.ReadAsStringAsync();
            var apiResponseMember = JsonConvert.DeserializeObject<ApiResBookDetail>(body);

            var retrievedBookDetail = new ObservableCollection<BOOK_DETAIL>(apiResponseMember!.data);

            foreach (BOOK_DETAIL bt in retrievedBookDetail)
            {
                if (bt.ISBN13 == Book.ISBN13&&bt.Status=="normal")
                {
                    Book.BOOK_DETAILs.Add(bt);
                    break;
                }
            }
            
            return 1;
        }
        catch (HttpRequestException e)
        {
            MyMessageBox error = new MyMessageBox("Unable to find the book", "Error",
                MyMessageBox.MessageBoxButton.OK, MyMessageBox.MessageBoxImage.Error);
            await error.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());
            return 0;
        }
    }

    public bool CheckAdd()
    {
        return (BookQuantity >= 0) && (BookQuantity != null);
    }
    
    private Task<HttpResponseMessage> PostBookAsync(object book)
    {
        var payload = new
        {
            data = book
        };

        var contentString = JsonConvert.SerializeObject(payload);
        var requestContent = new StringContent(contentString, Encoding.UTF8, "application/json");

        return _authenticationService.PostAsync("/api/books", requestContent);
    }

    private async Task<HttpResponseMessage> PutBookAsync(string isbn, bool deleted)
    {
        var putBook = new
        {
            ISBN13 = isbn,
            Deleted = deleted
        };
        var payload = new
        {
            data = putBook
        };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _authenticationService.PutAsync("/api/books", content);
    }
    
    private async Task RegisterBookDetailAsync(string isbn13,int? originalQuantity,int? additionalQuantity=0)
    {
        var createdBookDetail = new
        {
            ISBN13 = isbn13,
            Status = "normal",
            Quantity = originalQuantity  + additionalQuantity
        };
        
        var payload = new
        {
            data = createdBookDetail
        };
        
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _authenticationService.PutAsync("/api/book_details", content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ShowResultMessageBox("An unexpected error has occurred.\nPlease report this issue to your IT department.",
                MyMessageBox.MessageBoxImage.Error);
        }
    }
    
    private string GetResultContentString(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Conflict => "This book is already registered in the database.",
            HttpStatusCode.ServiceUnavailable => "Unable to connect to the database.\nPlease check your internet connection and try again.",
            HttpStatusCode.BadRequest => "An unexpected error has occurred.\nPlease report this issue to your IT department.",
            _ => "Book successfully added to the database"
        };
    }

    private MyMessageBox.MessageBoxImage GetResultBoxIcon(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Conflict => MyMessageBox.MessageBoxImage.Error,
            HttpStatusCode.ServiceUnavailable => MyMessageBox.MessageBoxImage.Error,
            HttpStatusCode.BadRequest => MyMessageBox.MessageBoxImage.Error,
            _ => MyMessageBox.MessageBoxImage.Information
        };
    }

    private void ShowResultMessageBox(string contentString, MyMessageBox.MessageBoxImage boxIcon)
    {
        var resultBox = new MyMessageBox(contentString, "Result",
            MyMessageBox.MessageBoxButton.OK, boxIcon);
        resultBox.Show();
    }
    
    private async Task RefreshBookViewModel()
    {
        var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
        await bookViewModel.GetData();
    }

    public void reset()
    {
        Book = null;
        ReleaseDate=String.Empty;
        ImageUrl=String.Empty;
        IsCoverLoading = false;
    }
}

public class ApiRespondedBook
{
    public BOOK Data { get; set; }
}

public class ApiRespondedBookDetail
{
    public ObservableCollection<BOOK_DETAIL> DetailData { get; set; }
}