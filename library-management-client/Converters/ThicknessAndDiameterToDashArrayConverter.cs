using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia_DependencyInjection.Converters;

public class ThicknessAndDiameterToDashArrayConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || !double.TryParse(values[0]?.ToString(), out double thickness)
                             || !double.TryParse(values[1]?.ToString(), out double diameter))
        {
            return new BindingNotification(new InvalidCastException(), 
                BindingErrorType.Error);
        }

        double circumference = Math.PI * diameter;
        double lineLength = circumference * 0.75;
        double gapLength = circumference * 0.25;
        
        return new AvaloniaList<double>(lineLength / thickness, gapLength / thickness);
    }
}