using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.Abstractions.Repositories;

public interface IImportacaoRepository
{
    Task<long> AdicionarAsync(Importacao importacao);

    Task<Importacao?> ObterPorIdAsync(long id);

    Task<IReadOnlyCollection<Importacao>> ListarAsync();

    Task AtualizarAsync(Importacao importacao);
}