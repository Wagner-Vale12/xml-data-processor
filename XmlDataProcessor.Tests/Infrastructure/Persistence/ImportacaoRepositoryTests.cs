using Dapper;
using Xunit;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Infrastructure.Persistence.Connection;
using XmlDataProcessor.Infrastructure.Persistence.Repositories;

namespace XmlDataProcessor.Tests.Infrastructure.Persistence;

public class ImportacaoRepositoryTests
{
    private const string ConnectionString =
        "Server=.\\SQLEXPRESS;Database=XmlDataProcessor;Trusted_Connection=True;TrustServerCertificate=True;";

    [Fact]
    public async Task DeveObterImportacaoPorId()
    {
        var connectionFactory =
            new SqlConnectionFactory(ConnectionString);

        var repository =
            new ImportacaoRepository(connectionFactory);

        var nomeArquivo =
            $"movimentos-teste-{Guid.NewGuid()}.xml";

        var importacao = new Importacao(
            nomeArquivo,
            new DateTime(2026, 8, 25, 10, 30, 0));

        await repository.AdicionarAsync(importacao);

        var id = await ObterIdPorNomeArquivoAsync(
            connectionFactory,
            nomeArquivo);

        var importacaoObtida =
            await repository.ObterPorIdAsync(id);

        Assert.NotNull(importacaoObtida);
        Assert.Equal(id, importacaoObtida.Id);
        Assert.Equal(
            nomeArquivo,
            importacaoObtida.NomeArquivo);
    }

    [Fact]
    public async Task DeveAtualizarImportacao()
    {
        var connectionFactory =
            new SqlConnectionFactory(ConnectionString);

        var repository =
            new ImportacaoRepository(connectionFactory);

        var nomeArquivo =
            $"movimentos-teste-{Guid.NewGuid()}.xml";

        var novaImportacao = new Importacao(
            nomeArquivo,
            new DateTime(2026, 8, 25, 10, 30, 0));

        await repository.AdicionarAsync(novaImportacao);

        var id = await ObterIdPorNomeArquivoAsync(
            connectionFactory,
            nomeArquivo);

        var importacao =
            await repository.ObterPorIdAsync(id);

        Assert.NotNull(importacao);

        importacao.IniciarProcessamento();
        importacao.RegistrarSucesso();
        importacao.RegistrarErro();
        importacao.RegistrarDuplicado();
        importacao.Concluir();

        await repository.AtualizarAsync(importacao);

        var importacaoAtualizada =
            await repository.ObterPorIdAsync(id);

        Assert.NotNull(importacaoAtualizada);

        Assert.Equal(
            importacao.Status,
            importacaoAtualizada.Status);

        Assert.Equal(
            importacao.TotalRegistros,
            importacaoAtualizada.TotalRegistros);

        Assert.Equal(
            importacao.TotalSucessos,
            importacaoAtualizada.TotalSucessos);

        Assert.Equal(
            importacao.TotalErros,
            importacaoAtualizada.TotalErros);

        Assert.Equal(
            importacao.TotalDuplicados,
            importacaoAtualizada.TotalDuplicados);
    }

    private static async Task<long> ObterIdPorNomeArquivoAsync(
        ISqlConnectionFactory connectionFactory,
        string nomeArquivo)
    {
        const string sql = """
            SELECT TOP 1 Id
            FROM Importacoes
            WHERE NomeArquivo = @NomeArquivo
            ORDER BY Id DESC;
            """;

        using var connection =
            connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<long>(
            sql,
            new
            {
                NomeArquivo = nomeArquivo
            });
    }
}