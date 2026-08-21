using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Tests.Domain.ImportacaoTests;

public class ImportacaoCriacaoTests
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

        var importacao = new Importacao(
            nomeArquivo,
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(nomeArquivo, importacao.NomeArquivo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveLancarExcecaoQuandoNomeArquivoForInvalido(string? nomeArquivo)
    {
        Assert.Throws<ArgumentException>(
            () => new Importacao(
                nomeArquivo!,
                new DateTime(2026, 8, 18, 14, 30, 0)));
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
    public void DeveCriarImportacaoComTotalSucessosZero()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(0, importacao.TotalSucessos);
    }

    [Fact]
    public void DeveCriarImportacaoComTotalErrosZero()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(0, importacao.TotalErros);
    }

    [Fact]
    public void DeveCriarImportacaoComTotalDuplicadosZero()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));

        Assert.Equal(0, importacao.TotalDuplicados);
    }
}