using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Controls;

public partial class HeaderControl : ContentView
{
    public static readonly BindableProperty TituloProperty =
        BindableProperty.Create(nameof(Titulo), typeof(string), typeof(HeaderControl), string.Empty);

    public string Titulo
    {
        get => (string)GetValue(TituloProperty);
        set => SetValue(TituloProperty, value);
    }

    public static readonly BindableProperty MostrarBotaoVoltarProperty =
        BindableProperty.Create(nameof(MostrarBotaoVoltar), typeof(bool), typeof(HeaderControl), true);

    public bool MostrarBotaoVoltar
    {
        get => (bool)GetValue(MostrarBotaoVoltarProperty);
        set => SetValue(MostrarBotaoVoltarProperty, value);
    }

    public HeaderControl()
    {
        InitializeComponent();
    }
}