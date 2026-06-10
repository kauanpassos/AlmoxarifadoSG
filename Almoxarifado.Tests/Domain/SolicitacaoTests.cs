using System;
using FluentAssertions;
using Xunit;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Constants;

namespace Almoxarifado.Tests.Domain;

public class SolicitacaoTests
{
    [Fact]
    public void Cancelar_SolicitacaoAprovada_DeveLancarExcecao()
    {

        var solicitacao = new Solicitacao("1", "user123", "Teste");
        solicitacao.Aprovar();


        Action acao = () => solicitacao.Cancelar();


        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("Apenas solicitações pendentes ou em análise podem ser canceladas.");
    }

    [Fact]
    public void Aprovar_SolicitacaoPendente_DeveMudarStatusParaAprovada()
    {

        var solicitacao = new Solicitacao("1", "user123", "Teste");


        solicitacao.Aprovar();


        solicitacao.Status.Should().Be(StatusSolicitacao.Aprovada);
    }
}
