using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Infrastructure.Persistence.Connection;
using Dapper;

namespace XmlDataProcessor.Infrastructure.Persistence.Repositories;

public class ImportacaoRepository : IImportacaoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ImportacaoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(
                nameof(connectionFactory));
    }

    public async Task AdicionarAsync(Importacao importacao)
    {
        const string sql = """
        INSERT INTO Importacoes
        (
            NomeArquivo,
            DataRecebimento,
            Status,
            TotalRegistros,
            TotalSucessos,
            TotalErros,
            TotalDuplicados
        )
        VALUES
        (
            @NomeArquivo,
            @DataRecebimento,
            @Status,
            @TotalRegistros,
            @TotalSucessos,
            @TotalErros,
            @TotalDuplicados
        );
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                importacao.NomeArquivo,
                importacao.DataRecebimento,
                Status = (int)importacao.Status,
                importacao.TotalRegistros,
                importacao.TotalSucessos,
                importacao.TotalErros,
                importacao.TotalDuplicados
            });
    }

    public Task<Importacao?> ObterPorIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task AtualizarAsync(Importacao importacao)
    {
        throw new NotImplementedException();
    }
}