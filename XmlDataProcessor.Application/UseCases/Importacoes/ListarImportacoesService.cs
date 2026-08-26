using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.UseCases.Importacoes;

public class ListarImportacoesService
{
    private readonly IImportacaoRepository _repository;

    public ListarImportacoesService(
        IImportacaoRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    public async Task<IReadOnlyCollection<Importacao>> ExecutarAsync()
    {
        return await _repository.ListarAsync();
    }
}