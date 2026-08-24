using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.Abstractions.Repositories;

public interface IImportacaoRepository
{
    Task AdicionarAsync(Importacao importacao);

    Task<Importacao?> ObterPorIdAsync(long id);

    Task AtualizarAsync(Importacao importacao);
}