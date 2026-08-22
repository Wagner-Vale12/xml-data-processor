using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.UseCases.Importacoes;

public class IniciarImportacaoService
{
    private readonly IImportacaoRepository _repository;

    public IniciarImportacaoService(IImportacaoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task ExecutarAsync(
        string nomeArquivo,
        DateTime dataRecebimento)
    {
        var importacao = new Importacao(
            nomeArquivo,
            dataRecebimento);

        await _repository.AdicionarAsync(importacao);
    }
}