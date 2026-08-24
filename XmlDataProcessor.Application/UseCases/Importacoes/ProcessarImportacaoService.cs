using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Application.Abstractions.Xml;
using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.UseCases.Importacoes;

public class ProcessarImportacaoService
{
    private readonly IImportacaoRepository _repository;
    private readonly ILeitorMovimentosXml _leitorXml;
    private readonly IMovimentoRepository _movimentoRepository;

    public ProcessarImportacaoService(
        IImportacaoRepository repository,
        ILeitorMovimentosXml leitorXml,
        IMovimentoRepository movimentoRepository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        _leitorXml = leitorXml
            ?? throw new ArgumentNullException(nameof(leitorXml));

        _movimentoRepository = movimentoRepository
            ?? throw new ArgumentNullException(nameof(movimentoRepository));
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

        IReadOnlyCollection<Movimento> movimentos;

        try
        {
            movimentos = await _leitorXml.LerAsync(
                importacao.NomeArquivo);
        }
        catch
        {
            importacao.Falhar();

            await _repository.AtualizarAsync(importacao);

            throw;
        }

        foreach (var movimento in movimentos)
        {
            var existe =
                await _movimentoRepository.ExistePorIdExternoAsync(
                    movimento.IdExterno);

            if (existe)
            {
                importacao.RegistrarDuplicado();
                continue;
            }

            try
            {
                await _movimentoRepository.AdicionarAsync(movimento);

                importacao.RegistrarSucesso();
            }
            catch
            {
                importacao.RegistrarErro();
            }
        }

        importacao.Concluir();

        await _repository.AtualizarAsync(importacao);
    }
}