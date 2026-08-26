using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.UseCases.Importacoes;

public class ObterImportacaoPorIdService
{
    private readonly IImportacaoRepository _repository;

    public ObterImportacaoPorIdService(
        IImportacaoRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    public async Task<Importacao?> ExecutarAsync(long id)
    {
        return await _repository.ObterPorIdAsync(id);
    }
}