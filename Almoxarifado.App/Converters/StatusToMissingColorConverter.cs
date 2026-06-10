using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Almoxarifado.App.Converters;

public sealed class StatusToMissingColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string estado)
            return Color.FromArgb("#BDBDBD"); // Cinza (Inativo)

        return estado == "EmFalta"
            ? Color.FromArgb("#DA0004") // Vermelho (Ativo)
            : Color.FromArgb("#BDBDBD"); // Cinza (Inativo)
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}