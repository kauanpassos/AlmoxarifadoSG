using System;
using FluentAssertions;
using Xunit;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Tests.Domain;

public class ProdutoTests
{
    [Fact]
    public void BaixarEstoque_ComQuantidadeValida_DeveSubtrairDoEstoque()
    {

        var produto = new Produto("1", 100, "Caneta", "Geral", "UNI", 10);
        produto.AdicionarEstoque(50);


        produto.BaixarEstoque(20);


        produto.QtdEstoque.Should().Be(30);
    }

    [Fact]
    public void BaixarEstoque_QuantidadeMaiorQueEstoque_DeveLancarExcecao()
    {

        var produto = new Produto("1", 100, "Caneta", "Geral", "UNI", 10);
        produto.AdicionarEstoque(10);


        Action acao = () => produto.BaixarEstoque(20);


        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Estoque insuficiente de material.");
    }
}
