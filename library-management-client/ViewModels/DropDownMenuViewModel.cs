using System.Collections.Generic;
using Avalonia_DependencyInjection.Models;
using Avalonia.Controls;

namespace Avalonia_DependencyInjection.ViewModels;

public class DropDownMenuViewModel
{
    public DropDownMenuViewModel(string header, List<SubItem> subItems)
    {
        Header = header;
        SubItems = subItems;
    }

    public DropDownMenuViewModel(string header, UserControl screen)
    {
        Header = header;
        Screen = screen;
    }

    public string Header { get; private set; }
    public List<SubItem> SubItems { get; private set; }
    public UserControl Screen { get; private set; }
}