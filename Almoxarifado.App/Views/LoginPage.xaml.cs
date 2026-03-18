using System;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly Supabase.Client _supabase;

        public LoginPage(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabase = supabaseClient;
        }

        // Se você tinha a função de quando o botão de login é clicado, ela entra aqui!
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // O seu código do botão de login fica aqui dentro
        }
    }
}