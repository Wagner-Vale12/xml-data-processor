using Xunit;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Tests.Application.UseCases.Importacoes;

public class IniciarImportacaoServiceTests
{
    private class ImportacaoRepositoryFake : IImportacaoRepository
    {
        public Task<IReadOnlyCollection<Importacao>> ListarAsync()
        {
            IReadOnlyCollection<Importacao> importacoes =
                Array.Empty<Importacao>();

            return Task.FromResult(importacoes);
        }
        public Importacao? ImportacaoAdicionada { get; private set; }

        public long IdGerado { get; set; } = 1;

        public Task<long> AdicionarAsync(Importacao importacao)
        {
            ImportacaoAdicionada = importacao;

            return Task.FromResult(IdGerado);
        }

        public Task<Importacao?> ObterPorIdAsync(long id)
        {
            return Task.FromResult<Importacao?>(null);
        }

        public Task AtualizarAsync(Importacao importacao)
        {
            return Task.CompletedTask;
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

    [Fact]
    public async Task DeveRetornarIdGeradoPeloRepository()
    {
        var repository = new ImportacaoRepositoryFake
        {
            IdGerado = 123
        };

        var service = new IniciarImportacaoService(repository);

        var id = await service.ExecutarAsync(
            "movimentos-2026-08-26.xml",
            new DateTime(2026, 8, 26, 10, 0, 0));

        Assert.Equal(123, id);
    }

    [Fact]
    public async Task NaoDeveCriarImportacaoQuandoArquivoNaoForXml()
    {
        var repository = new ImportacaoRepositoryFake();

        var service = new IniciarImportacaoService(repository);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ExecutarAsync(
                "arquivo.txt",
                new DateTime(2026, 8, 26, 10, 0, 0)));
    }
}