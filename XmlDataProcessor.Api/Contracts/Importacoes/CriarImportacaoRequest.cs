namespace XmlDataProcessor.Api.Contracts.Importacoes;

public class CriarImportacaoRequest
{
    public string NomeArquivo { get; set; } = string.Empty;

    public DateTime DataRecebimento { get; set; }
}