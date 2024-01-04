using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia_DependencyInjection.Converters;

public class GenderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            if (intValue == 0)
                return "Nam";
            
            if (intValue == 1)
                return "Nữ";
        }

        return new BindingNotification(new InvalidCastException(), 
            BindingErrorType.Error);
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string gender)
        {
            if (gender == "Nam")
                return 0;
            
            if (gender == "Nữ")
                return 1;
        }

        return new BindingNotification(new InvalidCastException(), 
            BindingErrorType.Error);
    }
}
