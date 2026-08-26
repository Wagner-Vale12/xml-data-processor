using Microsoft.AspNetCore.Mvc;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Api.Contracts.Importacoes;

namespace XmlDataProcessor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportacoesController : ControllerBase
{
    private readonly IniciarImportacaoService _iniciarImportacaoService;
    private readonly ProcessarImportacaoService _processarImportacaoService;

    private readonly ObterImportacaoPorIdService _obterImportacaoPorIdService;

    public ImportacoesController(
     IniciarImportacaoService iniciarImportacaoService,
     ProcessarImportacaoService processarImportacaoService,
     ObterImportacaoPorIdService obterImportacaoPorIdService)
    {
        _iniciarImportacaoService = iniciarImportacaoService;
        _processarImportacaoService = processarImportacaoService;
        _obterImportacaoPorIdService = obterImportacaoPorIdService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
    [FromBody] CriarImportacaoRequest request)
    {
        await _iniciarImportacaoService.ExecutarAsync(
         request.NomeArquivo,
         request.DataRecebimento);

        return Ok();
    }

    [HttpPost("{id:long}/processar")]
    public async Task<IActionResult> Processar(long id)
    {
        await _processarImportacaoService.ExecutarAsync(id);

        return Ok();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObterPorId(long id)
    {
        var importacao =
            await _obterImportacaoPorIdService.ExecutarAsync(id);

        if (importacao is null)
        {
            return NotFound();
        }

        return Ok(importacao);
    }
}