using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia_DependencyInjection.Converters;

public class DateTimeToAgeStringConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return $" {DateTime.Now.Year - date.Year} years old";
        }
        
        return new BindingNotification(new InvalidCastException(), 
            BindingErrorType.Error);
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new BindingNotification(new InvalidCastException(), 
            BindingErrorType.Error);
    }
}