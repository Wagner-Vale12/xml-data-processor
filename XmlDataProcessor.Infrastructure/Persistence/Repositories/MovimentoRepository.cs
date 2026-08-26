using Dapper;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Infrastructure.Persistence.Connection;

namespace XmlDataProcessor.Infrastructure.Persistence.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public MovimentoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(
                nameof(connectionFactory));
    }

    public async Task<bool> ExistePorIdExternoAsync(
     string idExterno)
    {
        const string sql = """
        SELECT COUNT(1)
        FROM Movimentos
        WHERE IdExterno = @IdExterno;
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        var quantidade = await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                IdExterno = idExterno
            });

        return quantidade > 0;
    }

    public async Task AdicionarAsync(Movimento movimento)
    {
        const string sql = """
        INSERT INTO Movimentos
        (
            IdExterno,
            Tipo,
            Valor,
            DataMovimento,
            Documento
        )
        VALUES
        (
            @IdExterno,
            @Tipo,
            @Valor,
            @DataMovimento,
            @Documento
        );
        """;

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                movimento.IdExterno,
                Tipo = (int)movimento.Tipo,
                movimento.Valor,
                movimento.DataMovimento,
                movimento.Documento
            });
    }
}