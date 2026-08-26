using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Domain.Entities;

public class Importacao
{
    public long Id { get; private set; }
    public string NomeArquivo { get; private set; }

    public DateTime DataRecebimento { get; private set; }

    public StatusImportacao Status { get; private set; }

    public int TotalRegistros { get; private set; }

    public int TotalSucessos { get; private set; }

    public int TotalErros { get; private set; }

    public int TotalDuplicados { get; private set; }

    public Importacao(
        string nomeArquivo,
        DateTime dataRecebimento)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
        {
            throw new ArgumentException(
                "O nome do arquivo é obrigatório.",
                nameof(nomeArquivo));
        }

        if (dataRecebimento == default)
        {
            throw new ArgumentException(
                "A data de recebimento é obrigatória.",
                nameof(dataRecebimento));
        }

        NomeArquivo = nomeArquivo.Trim();
        DataRecebimento = dataRecebimento;
        Status = StatusImportacao.Recebida;
    }

    public void IniciarProcessamento()
    {
        if (Status != StatusImportacao.Recebida)
        {
            throw new InvalidOperationException(
                "A importação só pode iniciar o processamento quando estiver com status Recebida.");
        }

        Status = StatusImportacao.Processando;
    }

public void Concluir()
{
    ValidarSeEstaProcessando();

    Status = TotalErros > 0
        ? StatusImportacao.ConcluidaComErros
        : StatusImportacao.Concluida;
}

    public void Falhar()
    {
        ValidarSeEstaProcessando();

        Status = StatusImportacao.Falhou;
    }

    private void ValidarSeEstaProcessando()
    {
        if (Status != StatusImportacao.Processando)
        {
            throw new InvalidOperationException(
                "A importação deve estar com status Processando para realizar esta operação.");
        }
    }

    public void RegistrarSucesso()
    {
        ValidarSeEstaProcessando();

        TotalRegistros++;
        TotalSucessos++;
    }

    public void RegistrarErro()
    {
        ValidarSeEstaProcessando();

        TotalRegistros++;
        TotalErros++;
    }

    public void RegistrarDuplicado()
    {
        ValidarSeEstaProcessando();

        TotalRegistros++;
        TotalDuplicados++;
    }

    internal Importacao(
    long id,
    string nomeArquivo,
    DateTime dataRecebimento,
    StatusImportacao status,
    int totalRegistros,
    int totalSucessos,
    int totalErros,
    int totalDuplicados)
    {
        Id = id;
        NomeArquivo = nomeArquivo;
        DataRecebimento = dataRecebimento;
        Status = status;
        TotalRegistros = totalRegistros;
        TotalSucessos = totalSucessos;
        TotalErros = totalErros;
        TotalDuplicados = totalDuplicados;
    }

    public static Importacao Restaurar(
    long id,
    string nomeArquivo,
    DateTime dataRecebimento,
    StatusImportacao status,
    int totalRegistros,
    int totalSucessos,
    int totalErros,
    int totalDuplicados)
    {
        return new Importacao(
            id,
            nomeArquivo,
            dataRecebimento,
            status,
            totalRegistros,
            totalSucessos,
            totalErros,
            totalDuplicados);
    }
}