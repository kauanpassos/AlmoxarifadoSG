using System;
using System.Threading;
using System.Threading.Tasks;
using Almoxarifado.Application.Commands;
using Almoxarifado.Application.Handlers;
using Almoxarifado.Domain.Constants;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Almoxarifado.Tests.Application;

public class AtualizarStatusSolicitacaoHandlerTests
{
    private readonly IReadOnlyRepository<Solicitacao> _readRepoMock;
    private readonly IWriteOnlyRepository<Solicitacao> _writeRepoMock;
    private readonly IEstoqueTransactionService _estoqueServiceMock;
    private readonly AtualizarStatusSolicitacaoHandler _handler;

    public AtualizarStatusSolicitacaoHandlerTests()
    {
        _readRepoMock = Substitute.For<IReadOnlyRepository<Solicitacao>>();
        _writeRepoMock = Substitute.For<IWriteOnlyRepository<Solicitacao>>();
        _estoqueServiceMock = Substitute.For<IEstoqueTransactionService>();

        _handler = new AtualizarStatusSolicitacaoHandler(
            _readRepoMock,
            _writeRepoMock,
            _estoqueServiceMock);
    }

    [Fact]
    public async Task Handle_StatusAprovada_DeveAcionarEstoqueTransactionService()
    {

        var command = new AtualizarStatusSolicitacaoCommand("sol_1", StatusSolicitacao.Aprovada);
        var solicitacaoDb = new Solicitacao("sol_1", "user_1", "teste");
        
        _readRepoMock.GetByIdAsync("sol_1").Returns(solicitacaoDb);
        
        var solicitacaoAprovada = new Solicitacao("sol_1", "user_1", "teste");
        solicitacaoAprovada.Aprovar();

        _estoqueServiceMock.ProcessarAprovacaoSolicitacaoAsync("sol_1")
            .Returns(Task.FromResult(solicitacaoAprovada));


        var result = await _handler.Handle(command, CancellationToken.None);


        result.Should().NotBeNull();
        result.Status.Should().Be(StatusSolicitacao.Aprovada);
        await _estoqueServiceMock.Received(1).ProcessarAprovacaoSolicitacaoAsync("sol_1");
        await _writeRepoMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    [Fact]
    public async Task Handle_StatusRecusada_DeveApenasAtualizarStatusNoBancoSemChamarEstoqueService()
    {

        var command = new AtualizarStatusSolicitacaoCommand("sol_1", StatusSolicitacao.Recusada);
        var solicitacaoDb = new Solicitacao("sol_1", "user_1", "teste");
        
        _readRepoMock.GetByIdAsync("sol_1").Returns(solicitacaoDb);


        var result = await _handler.Handle(command, CancellationToken.None);


        result.Should().NotBeNull();
        result.Status.Should().Be(StatusSolicitacao.Recusada);
        await _estoqueServiceMock.DidNotReceiveWithAnyArgs().ProcessarAprovacaoSolicitacaoAsync(default!);
        await _writeRepoMock.Received(1).UpdateAsync(Arg.Is<Solicitacao>(s => s.Status == StatusSolicitacao.Recusada));
    }
}
