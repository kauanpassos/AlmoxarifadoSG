using CommunityToolkit.Maui.Views;

namespace Almoxarifado.App.Popups;

public partial class QuantidadePopup : Popup
{
    private int _quantidadeAtual = 1;
    private readonly long _estoqueMaximo;

    // Recebemos o nome e o limite de estoque quando o popup é aberto
    public QuantidadePopup(string nomePeca, long estoqueDisponivel)
    {
        InitializeComponent();

        _estoqueMaximo = estoqueDisponivel;

        LblNomePeca.Text = nomePeca;
        LblEstoqueDisponivel.Text = $"Estoque disponível: {estoqueDisponivel} un";

        AtualizarLabelQuantidade();
    }

    private void OnDiminuirClicked(object sender, EventArgs e)
    {
        if (_quantidadeAtual > 1)
        {
            _quantidadeAtual--;
            AtualizarLabelQuantidade();
        }
    }

    private void OnAumentarClicked(object sender, EventArgs e)
    {
        // Só deixa aumentar se tiver em estoque
        if (_quantidadeAtual < _estoqueMaximo)
        {
            _quantidadeAtual++;
            AtualizarLabelQuantidade();
        }
    }

    private void AtualizarLabelQuantidade()
    {
        LblQuantidade.Text = _quantidadeAtual.ToString();
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        // Retorna 0 (ou null) para indicar que o usuário cancelou
        Close(0);
    }

    private void OnConfirmarClicked(object sender, EventArgs e)
    {
        // Retorna a quantidade que o usuário escolheu
        Close(_quantidadeAtual);
    }
}