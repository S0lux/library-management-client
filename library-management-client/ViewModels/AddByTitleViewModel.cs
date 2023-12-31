using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia_DependencyInjection.Controls;
using Avalonia_DependencyInjection.Models;
using Avalonia_DependencyInjection.Services;
using Avalonia_DependencyInjection.Views;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avalonia_DependencyInjection.ViewModels;

public partial class AddByTitleViewModel: ViewModelBase
{
    private readonly AuthenticationService _authService;
    
    [ObservableProperty] private ObservableCollection<BOOK>? _books;
    [ObservableProperty] private BOOK _selectedBook;
    [ObservableProperty] private string _imageUrl;
    public int BookQuantity { get; set; }
    public int ShelfNumber { get; set; }

    public AddByTitleViewModel(AuthenticationService authService)
    {
        _authService = authService;
    }
    
    public async Task RetrieveBooksByTitle(string title)
    {
        var response = await _authService.GetAsync($"/api/books/title/{title}");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            // Notify user
            // 404 -> No book found
            // 500 -> Unknown error

            var messageBoxString = "";
            var messageBoxIcon = MyMessageBox.MessageBoxImage.Error;

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    messageBoxString = "Không thể tìm thấy tựa sách.";
                    break;
                default:
                    messageBoxString =
                        "Đã có lỗi xảy ra.\nVui lòng liên hệ đến bộ phận IT.";
                    break;
            }
            
            var messageBox = new MyMessageBox(messageBoxString, "Không thể hiển thị được sách", 
                MyMessageBox.MessageBoxButton.OK, messageBoxIcon);
            messageBox.Show();
            
            return;
        }
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObj = JsonConvert.DeserializeObject<apiResponse>(responseBody);

        if (responseObj != null) Books = responseObj.data;
    }

    // For updating the book cover preview
    partial void OnSelectedBookChanged(BOOK value)
    {
        if (value == null) return;
        ImageUrl = $"https://covers.openlibrary.org/b/isbn/{value.ISBN13}-L.jpg?default=false";
    }


    [RelayCommand]
    private async Task RegisterBook(BOOK book)
    {
        var loadingSpinner = App.AppHost!.Services.GetRequiredService<AddBookWindowViewModel>();
        var confirmationResult = await ShowQuantityConfirmMessageBox(book);

        if (confirmationResult == QuantityConfirmMessageBox.ButtonResult.OK)
        {
            loadingSpinner.IsBusy = true;

            var newBook = new
            {
                ISBN13 = book.ISBN13,
                Title = book.Title,
                Author = book.Author,
                Shelf = ShelfNumber,
                PublishDate = book.PublishDate.ToString("O")
            };

            var postResult = await PostBookAsync(newBook);
            var resultContentString = GetResultContentString(postResult.StatusCode);
            var resultBoxIcon = GetResultBoxIcon(postResult.StatusCode);

            if (postResult.IsSuccessStatusCode)
            {
                await RefreshBookViewModel();
                await RegisterBookDetailAsync(book.ISBN13);
            }

            if (postResult.StatusCode == HttpStatusCode.Conflict)
            {
                var resultBox = new MyMessageBox("Tụa sách này đã tồn tại hoặc đã bị xóa.\n" +
                                                 "Sử dụng chức năng thêm bằng ISBN để cập nhật sách?", "Xác nhận",
                    MyMessageBox.MessageBoxButton.OkCancel, MyMessageBox.MessageBoxImage.Information);
                await resultBox.ShowDialog(App.AppHost.Services.GetRequiredService<AddBookWindow>());
                if (MyMessageBox.buttonResultClicked == MyMessageBox.ButtonResult.OK)
                {
                    var addvm = App.AppHost.Services.GetRequiredService<AddBookWindowViewModel>();
                    var isbnvm=App.AppHost.Services.GetRequiredService<AddByISBNViewModel>();
                    isbnvm.BookQuantity = BookQuantity;
                    addvm.AddBy = "ISBN";
                    addvm.FindKey = book.ISBN13;
                    await addvm.Find();
                }
                loadingSpinner.IsBusy = false;
                return;

            }

            ShowResultMessageBox(resultContentString, resultBoxIcon);
            
            var box =  App.AppHost!.Services.GetRequiredService<BookViewModel>();
            box.BookCheckedList.Clear();
            box.CheckedAmount = 0;
            box.GetData();
        }


        loadingSpinner.IsBusy = false;
    }

    private async Task<QuantityConfirmMessageBox.ButtonResult> ShowQuantityConfirmMessageBox(BOOK book)
    {
        var messageBoxContentString = $"Số lượng thêm đối với sách có:\n" +
                                      $"Tựa:          {book.Title}\n" +
                                      $"ISBN:         {book.ISBN13}\n" +
                                      $"Tác giả:      {book.Author}";

        var mess = new QuantityConfirmMessageBox(
            messageBoxContentString,
            "Xác nhận",
            QuantityConfirmMessageBox.MessageBoxImage.Question,
            400, 270);

        await mess.ShowDialog(App.AppHost!.Services.GetRequiredService<AddBookWindow>());

        return QuantityConfirmMessageBox.buttonResultClicked;
    }

    private async Task RegisterBookDetailAsync(string isbn13)
    {
        var createdBookDetail = new
        {
            ISBN13 = isbn13,
            Status = "normal",
            Quantity = BookQuantity
        };
        
        var payload = new
        {
            data = createdBookDetail
        };
        
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _authService.PutAsync("/api/book_details", content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ShowResultMessageBox("Đã có lỗi xảy ra.\nVui lòng liên hệ với bộ phận IT.",
                                 MyMessageBox.MessageBoxImage.Error);
        }
    }

    private string GetResultContentString(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.ServiceUnavailable => "Không thể kết nối với máy chủ.\nVui lòng kiểm tra lại kết nối.",
            HttpStatusCode.BadRequest => "Đã có lỗi xảy ra.\nVui lòng liên hệ với bộ phận IT.",
            _ => "Thêm mới thành công"
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

    private Task<HttpResponseMessage> PostBookAsync(object book)
    {
        var payload = new
        {
            data = book
        };

        var contentString = JsonConvert.SerializeObject(payload);
        var requestContent = new StringContent(contentString, Encoding.UTF8, "application/json");

        return _authService.PostAsync("/api/books", requestContent);
    }

    private async Task RefreshBookViewModel()
    {
        var bookViewModel = App.AppHost!.Services.GetRequiredService<BookViewModel>();
        await bookViewModel.GetData();
    }

    public void reset()
    {
        ImageUrl=string.Empty;
        Books.Clear();
    }
}

public class apiResponse
{
    public ObservableCollection<BOOK> data;
}