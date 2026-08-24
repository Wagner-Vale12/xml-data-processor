using System.Globalization;
using System.Xml.Linq;
using XmlDataProcessor.Application.Abstractions.Xml;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Infrastructure.Xml;

public class LeitorMovimentosXml : ILeitorMovimentosXml
{
    public async Task<IReadOnlyCollection<Movimento>> LerAsync(
        string nomeArquivo)
    {
        var xml = await File.ReadAllTextAsync(nomeArquivo);

        var documento = XDocument.Parse(xml);

        var movimentos = documento
            .Descendants("Movimento")
            .Select(elemento => new Movimento(
                ObterValorObrigatorio(
                    elemento,
                    "IdExterno"),

                ObterTipoMovimentoObrigatorio(
    elemento,
    "Tipo"),

                ObterDecimalObrigatorio(
    elemento,
    "Valor"),

                ObterDataObrigatoria(
                    elemento,
                    "DataMovimento"),

                elemento.Element("Documento")?.Value))
            .ToList();

        return movimentos;
    }

    private static string ObterValorObrigatorio(
        XElement elemento,
        string nomeElemento)
    {
        var valor = elemento
            .Element(nomeElemento)?
            .Value;

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidDataException(
                $"O elemento {nomeElemento} é obrigatório.");
        }

        return valor;
    }

    private static decimal ObterDecimalObrigatorio(
    XElement elemento,
    string nomeElemento)
    {
        var valor = ObterValorObrigatorio(
            elemento,
            nomeElemento);

        if (!decimal.TryParse(
            valor,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var resultado))
        {
            throw new InvalidDataException(
                $"O elemento {nomeElemento} possui um valor inválido.");
        }

        return resultado;
    }

    private static TipoMovimento ObterTipoMovimentoObrigatorio(
    XElement elemento,
    string nomeElemento)
    {
        var valor = ObterValorObrigatorio(
            elemento,
            nomeElemento);

        if (!Enum.TryParse<TipoMovimento>(
            valor,
            ignoreCase: true,
            out var resultado))
        {
            throw new InvalidDataException(
                $"O elemento {nomeElemento} possui um valor inválido.");
        }

        return resultado;
    }

    private static DateTime ObterDataObrigatoria(
    XElement elemento,
    string nomeElemento)
    {
        var valor = ObterValorObrigatorio(
            elemento,
            nomeElemento);

        if (!DateTime.TryParse(
            valor,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var resultado))
        {
            throw new InvalidDataException(
                $"O elemento {nomeElemento} possui um valor inválido.");
        }

        return resultado;
    }

}