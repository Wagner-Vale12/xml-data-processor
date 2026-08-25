using Microsoft.AspNetCore.Mvc;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Api.Contracts.Importacoes;

namespace XmlDataProcessor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportacoesController : ControllerBase
{
    private readonly IniciarImportacaoService _service;

    public ImportacoesController(
        IniciarImportacaoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
    [FromBody] CriarImportacaoRequest request)
    {
        await _service.ExecutarAsync(
            request.NomeArquivo,
            request.DataRecebimento);

        return Ok();
    }
}