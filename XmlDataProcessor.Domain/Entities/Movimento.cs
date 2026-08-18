using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Domain.Entities;

public class Movimento
{
    public string IdExterno { get; private set; }

    public TipoMovimento Tipo { get; private set; }

    public decimal Valor { get; private set; }

    public DateTime DataMovimento { get; private set; }

    public string? Documento { get; private set; }

    public Movimento(
        string idExterno,
        TipoMovimento tipo,
        decimal valor,
        DateTime dataMovimento,
        string? documento)
    {
        if (string.IsNullOrWhiteSpace(idExterno))
        {
            throw new ArgumentException(
                "O IdExterno é obrigatório.",
                nameof(idExterno));
        }

        if (valor <= 0)
        {
            throw new ArgumentException(
                "O valor deve ser maior que zero.",
                nameof(valor));
        }

        if (dataMovimento == default)
        {
            throw new ArgumentException(
                "A data do movimento é obrigatória.",
                nameof(dataMovimento));
        }

        IdExterno = idExterno;
        Tipo = tipo;
        Valor = valor;
        DataMovimento = dataMovimento;

        Documento = string.IsNullOrWhiteSpace(documento)
            ? null
            : documento.Trim();
    }
}