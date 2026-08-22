using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;
using Xunit;

namespace XmlDataProcessor.Tests.Application.UseCases.Importacoes;

public class ProcessarImportacaoServiceTests
{
    private class ImportacaoRepositoryFake : IImportacaoRepository
    {
        public long? IdConsultado { get; private set; }

        public Importacao? ImportacaoRetornada { get; set; }

        public Task AdicionarAsync(Importacao importacao)
        {
            return Task.CompletedTask;
        }

        public Task<Importacao?> ObterPorIdAsync(long id)
        {
            IdConsultado = id;

            return Task.FromResult(ImportacaoRetornada);
        }
    }

    [Fact]
    public async Task DeveBuscarImportacaoPeloIdInformado()
    {
        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = new Importacao(
                "movimentos-2026-08-21.xml",
                new DateTime(2026, 8, 21, 10, 30, 0))
        };

        var service = new ProcessarImportacaoService(repository);

        await service.ExecutarAsync(15);

        Assert.Equal(15, repository.IdConsultado);
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoImportacaoNaoForEncontrada()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new ProcessarImportacaoService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecutarAsync(15));
    }

    [Fact]
    public async Task DeveIniciarProcessamentoDaImportacaoEncontrada()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-21.xml",
            new DateTime(2026, 8, 21, 10, 30, 0));

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var service = new ProcessarImportacaoService(repository);

        await service.ExecutarAsync(15);

        Assert.Equal(
            StatusImportacao.Processando,
            importacao.Status);
    }
}