using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Tests.Domain;

public class ImportacaoTests
{
    [Fact]
    public void DeveCriarImportacaoComStatusRecebida()
    {
        var importacao = new Importacao(
    "movimentos-2026-08-18.xml",
    new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(
            StatusImportacao.Recebida,
            importacao.Status);
    }

    [Fact]
    public void DeveCriarImportacaoComNomeArquivoInformado()
    {
        var nomeArquivo = "movimentos-2026-08-18.xml";

        var importacao = new Importacao(nomeArquivo, new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(nomeArquivo, importacao.NomeArquivo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveLancarExcecaoQuandoNomeArquivoForInvalido(string? nomeArquivo)
    {
        Assert.Throws<ArgumentException>(
            () => new Importacao(nomeArquivo!, new DateTime(2026, 8, 18, 14, 30, 0)));
    }

    [Fact]
    public void DeveRemoverEspacosDoNomeArquivo()
    {
        var importacao = new Importacao(
        "   movimentos-2026-08-18.xml   ",
        new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(
            "movimentos-2026-08-18.xml",
            importacao.NomeArquivo);
    }

    [Fact]
    public void DeveAlterarStatusParaProcessandoAoIniciarProcessamento()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();

        Assert.Equal(
            StatusImportacao.Processando,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoIniciarProcessamentoQuandoJaEstiverProcessando()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();

        Assert.Throws<InvalidOperationException>(
            () => importacao.IniciarProcessamento());
    }

    [Fact]
    public void DeveAlterarStatusParaConcluidaAoConcluirProcessamento()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();

        importacao.Concluir();

        Assert.Equal(
            StatusImportacao.Concluida,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoConcluirQuandoNaoEstiverProcessando()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Throws<InvalidOperationException>(
            () => importacao.Concluir());
    }

    [Fact]
    public void DeveAlterarStatusParaFalhouAoFalharProcessamento()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();

        importacao.Falhar();

        Assert.Equal(
            StatusImportacao.Falhou,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoFalharQuandoNaoEstiverProcessando()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Throws<InvalidOperationException>(
            () => importacao.Falhar());
    }

    [Fact]
    public void DeveAlterarStatusParaConcluidaComErrosAoConcluirComErros()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();

        importacao.ConcluirComErros();

        Assert.Equal(
            StatusImportacao.ConcluidaComErros,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoConcluirComErrosQuandoNaoEstiverProcessando()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Throws<InvalidOperationException>(
            () => importacao.ConcluirComErros());
    }

    [Fact]
    public void DeveCriarImportacaoComDataRecebimentoInformada()
    {
        var dataRecebimento = new DateTime(
            2026, 8, 18, 14, 30, 0);

        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            dataRecebimento);

        Assert.Equal(
            dataRecebimento,
            importacao.DataRecebimento);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoDataRecebimentoForInvalida()
    {
        Assert.Throws<ArgumentException>(
            () => new Importacao(
                "movimentos-2026-08-18.xml",
                default));
    }

    [Fact]
    public void DeveCriarImportacaoComTotalRegistrosZero()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(0, importacao.TotalRegistros);
    }

    [Fact]
    public void DeveIncrementarTotalRegistrosAoRegistrarRegistroProcessado()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();
        importacao.RegistrarRegistroProcessado();

        Assert.Equal(1, importacao.TotalRegistros);
    }

    [Fact]
    public void DeveAcumularTotalRegistrosAoRegistrarMultiplosRegistros()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        importacao.IniciarProcessamento();
        importacao.RegistrarRegistroProcessado();
        importacao.RegistrarRegistroProcessado();
        importacao.RegistrarRegistroProcessado();

        Assert.Equal(3, importacao.TotalRegistros);
    }

    [Fact]
    public void DeveLancarExcecaoAoRegistrarRegistroQuandoNaoEstiverProcessando()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Throws<InvalidOperationException>(
            () => importacao.RegistrarRegistroProcessado());
    }
}