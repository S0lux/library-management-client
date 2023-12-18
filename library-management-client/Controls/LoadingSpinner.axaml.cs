using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Avalonia_DependencyInjection.Controls;

public class LoadingSpinner : TemplatedControl
{
    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<LoadingSpinner, bool>(
        "IsLoading", false);

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly StyledProperty<double> ThicknessProperty = AvaloniaProperty.Register<LoadingSpinner, double>(
        "Thickness", 2.0);

    public double Thickness
    {
        get => GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    public static readonly StyledProperty<IBrush> ColorProperty = AvaloniaProperty.Register<LoadingSpinner, IBrush>(
        "Color", Brushes.Black);

    public IBrush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly StyledProperty<double> DiameterProperty = AvaloniaProperty.Register<LoadingSpinner, double>(
        "Diameter", 50);

    public double Diameter
    {
        get => GetValue(DiameterProperty);
        set => SetValue(DiameterProperty, value);
    }
}