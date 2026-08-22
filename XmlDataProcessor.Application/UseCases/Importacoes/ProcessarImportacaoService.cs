using XmlDataProcessor.Application.Abstractions.Repositories;

namespace XmlDataProcessor.Application.UseCases.Importacoes;

public class ProcessarImportacaoService
{
    private readonly IImportacaoRepository _repository;

    public ProcessarImportacaoService(IImportacaoRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }
    public async Task ExecutarAsync(long id)
    {
        var importacao = await _repository.ObterPorIdAsync(id);

        if (importacao is null)
        {
            throw new InvalidOperationException(
                "Importação não encontrada.");
        }

        importacao.IniciarProcessamento();
    }
}