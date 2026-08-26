using Dapper;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;
using XmlDataProcessor.Infrastructure.Persistence.Connection;

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

    public async Task<long> AdicionarAsync(Importacao importacao)
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
        OUTPUT INSERTED.Id
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

        return await connection.ExecuteScalarAsync<long>(
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

    public async Task<Importacao?> ObterPorIdAsync(long id)
    {
        const string sql = """
        SELECT
            Id,
            NomeArquivo,
            DataRecebimento,
            Status,
            TotalRegistros,
            TotalSucessos,
            TotalErros,
            TotalDuplicados
        FROM Importacoes
        WHERE Id = @Id;
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        var registro = await connection
            .QuerySingleOrDefaultAsync<ImportacaoDbModel>(
                sql,
                new { Id = id });

        if (registro is null)
        {
            return null;
        }

        return Importacao.Restaurar(
            registro.Id,
            registro.NomeArquivo,
            registro.DataRecebimento,
            (StatusImportacao)registro.Status,
            registro.TotalRegistros,
            registro.TotalSucessos,
            registro.TotalErros,
            registro.TotalDuplicados);
    }

    public async Task AtualizarAsync(Importacao importacao)
    {
        const string sql = """
        UPDATE Importacoes
        SET
            NomeArquivo = @NomeArquivo,
            DataRecebimento = @DataRecebimento,
            Status = @Status,
            TotalRegistros = @TotalRegistros,
            TotalSucessos = @TotalSucessos,
            TotalErros = @TotalErros,
            TotalDuplicados = @TotalDuplicados
        WHERE Id = @Id;
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                importacao.Id,
                importacao.NomeArquivo,
                importacao.DataRecebimento,
                Status = (int)importacao.Status,
                importacao.TotalRegistros,
                importacao.TotalSucessos,
                importacao.TotalErros,
                importacao.TotalDuplicados
            });
    }

    private class ImportacaoDbModel
    {
        public long Id { get; set; }

        public string NomeArquivo { get; set; } = string.Empty;

        public DateTime DataRecebimento { get; set; }

        public int Status { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalSucessos { get; set; }

        public int TotalErros { get; set; }

        public int TotalDuplicados { get; set; }
    }

    public async Task<IReadOnlyCollection<Importacao>> ListarAsync()
    {
        const string sql = """
        SELECT
            Id,
            NomeArquivo,
            DataRecebimento,
            Status,
            TotalRegistros,
            TotalSucessos,
            TotalErros,
            TotalDuplicados
        FROM Importacoes
        ORDER BY Id DESC;
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        var registros =
            await connection.QueryAsync<ImportacaoDbModel>(sql);

        var importacoes = registros
            .Select(registro => Importacao.Restaurar(
                registro.Id,
                registro.NomeArquivo,
                registro.DataRecebimento,
                (StatusImportacao)registro.Status,
                registro.TotalRegistros,
                registro.TotalSucessos,
                registro.TotalErros,
                registro.TotalDuplicados))
            .ToList();

        return importacoes;
    }
}