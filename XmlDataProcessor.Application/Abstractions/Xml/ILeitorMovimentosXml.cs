using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.Abstractions.Xml;

public interface ILeitorMovimentosXml
{
    Task<IReadOnlyCollection<Movimento>> LerAsync(
        string nomeArquivo);
}