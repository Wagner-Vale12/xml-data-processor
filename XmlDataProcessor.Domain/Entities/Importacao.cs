using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Domain.Entities;

public class Importacao
{
    public StatusImportacao Status { get; private set; }

    public Importacao()
    {
        Status = StatusImportacao.Recebida;
    }
}