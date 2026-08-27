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

    public async Task<long> ExecutarAsync(
     string nomeArquivo,
     DateTime dataRecebimento)
    {
        if (!nomeArquivo.EndsWith(
    ".xml",
    StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "O arquivo informado deve possuir extensão .xml.",
                nameof(nomeArquivo));
        }
        var importacao = new Importacao(
            nomeArquivo,
            dataRecebimento);

        var id = await _repository.AdicionarAsync(importacao);

        return id;
    }
}