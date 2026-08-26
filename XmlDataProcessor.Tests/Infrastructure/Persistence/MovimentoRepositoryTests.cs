using Xunit;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;
using XmlDataProcessor.Infrastructure.Persistence.Connection;
using XmlDataProcessor.Infrastructure.Persistence.Repositories;

namespace XmlDataProcessor.Tests.Infrastructure.Persistence;

public class MovimentoRepositoryTests
{
    [Fact]
    public async Task DeveAdicionarMovimentoEEncontrarPorIdExterno()
    {
        const string connectionString =
            "Server=.\\SQLEXPRESS;Database=XmlDataProcessor;Trusted_Connection=True;TrustServerCertificate=True;";

        var connectionFactory =
            new SqlConnectionFactory(connectionString);

        var repository =
            new MovimentoRepository(connectionFactory);

        var idExterno = $"MOV-TEST-{Guid.NewGuid()}";

        var movimento = new Movimento(
            idExterno,
            TipoMovimento.Entrada,
            250.75m,
            new DateTime(2026, 8, 25),
            "DOC-TEST");

        var existiaAntes =
            await repository.ExistePorIdExternoAsync(idExterno);

        await repository.AdicionarAsync(movimento);

        var existeDepois =
            await repository.ExistePorIdExternoAsync(idExterno);

        Assert.False(existiaAntes);
        Assert.True(existeDepois);
    }
}