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
            "Pendente" => Color.FromArgb("#F57C00"), // Laranja
            "Em análise" => Color.FromArgb("#1976D2"), // Azul
            "Aprovada" => Color.FromArgb("#2E7D32"), // Verde
            "Recusada" => Color.FromArgb("#D32F2F"), // Vermelho
            "Entregue" => Color.FromArgb("#7B1FA2"), // Roxo
            "Cancelada" => Color.FromArgb("#757575"), // Cinza
            _ => Color.FromArgb("#616161")  // Fallback
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}