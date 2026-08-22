using Xunit;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Tests.Application.UseCases.Importacoes;

public class IniciarImportacaoServiceTests
{
    private class ImportacaoRepositoryFake : IImportacaoRepository
    {
        public Importacao? ImportacaoAdicionada { get; private set; }

        public Task AdicionarAsync(Importacao importacao)
        {
            ImportacaoAdicionada = importacao;

            return Task.CompletedTask;
        }

        public Task<Importacao?> ObterPorIdAsync(long id)
        {
            return Task.FromResult<Importacao?>(null);
        }
    }

    [Fact]
    public async Task DeveAdicionarImportacaoNoRepository()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new IniciarImportacaoService(repository);

        await service.ExecutarAsync(
            "movimentos-2026-08-21.xml",
            new DateTime(2026, 8, 21, 10, 30, 0));

        Assert.NotNull(repository.ImportacaoAdicionada);
    }

    [Fact]
    public async Task DeveAdicionarImportacaoComDadosInformados()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new IniciarImportacaoService(repository);

        var nomeArquivo = "movimentos-2026-08-21.xml";
        var dataRecebimento = new DateTime(
            2026, 8, 21, 10, 30, 0);

        await service.ExecutarAsync(
            nomeArquivo,
            dataRecebimento);

        Assert.NotNull(repository.ImportacaoAdicionada);

        Assert.Equal(
            nomeArquivo,
            repository.ImportacaoAdicionada.NomeArquivo);

        Assert.Equal(
            dataRecebimento,
            repository.ImportacaoAdicionada.DataRecebimento);
    }

    [Fact]
    public async Task NaoDeveAdicionarImportacaoQuandoNomeArquivoForInvalido()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new IniciarImportacaoService(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ExecutarAsync(
                "",
                new DateTime(2026, 8, 21, 10, 30, 0)));

        Assert.Null(repository.ImportacaoAdicionada);
    }

    [Fact]
    public async Task NaoDeveAdicionarImportacaoQuandoDataRecebimentoForInvalida()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new IniciarImportacaoService(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ExecutarAsync(
                "movimentos-2026-08-21.xml",
                default));

        Assert.Null(repository.ImportacaoAdicionada);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoRepositoryForNulo()
    {
        Assert.Throws<ArgumentNullException>(
            () => new IniciarImportacaoService(null!));
    }

    [Fact]
public void DeveCriarImportacaoComIdZero()
{
    var importacao = new Importacao(
        "movimentos-2026-08-21.xml",
        new DateTime(2026, 8, 21, 10, 30, 0));

    Assert.Equal(0, importacao.Id);
}
}