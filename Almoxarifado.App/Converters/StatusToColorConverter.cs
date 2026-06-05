using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Almoxarifado.App.Converters;

public sealed class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string status)
            return Colors.Transparent;

        return status switch
        {
            "Aprovada" => Color.FromArgb("#2E7D32"),
            "Recusada" => Color.FromArgb("#D32F2F"),
            _ => Color.FromArgb("#F57C00")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
