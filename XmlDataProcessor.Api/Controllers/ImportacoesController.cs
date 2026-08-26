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

    private readonly ListarImportacoesService _listarImportacoesService;

    public ImportacoesController(
     IniciarImportacaoService iniciarImportacaoService,
     ProcessarImportacaoService processarImportacaoService,
     ObterImportacaoPorIdService obterImportacaoPorIdService,
     ListarImportacoesService listarImportacoesService)
    {
        _iniciarImportacaoService = iniciarImportacaoService;
        _processarImportacaoService = processarImportacaoService;
        _obterImportacaoPorIdService = obterImportacaoPorIdService;
        _listarImportacoesService = listarImportacoesService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar(
    [FromBody] CriarImportacaoRequest request)
    {
        var id = await _iniciarImportacaoService.ExecutarAsync(
            request.NomeArquivo,
            request.DataRecebimento);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id },
            new { id });
    }

    [HttpPost("{id:long}/processar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Processar(long id)
    {
        await _processarImportacaoService.ExecutarAsync(id);

        return Ok();
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var importacoes =
            await _listarImportacoesService.ExecutarAsync();

        return Ok(importacoes);
    }
}