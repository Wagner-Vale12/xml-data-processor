using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Tests.Domain.ImportacaoTests;

public class ImportacaoCicloVidaTests
{
    [Fact]
    public void DeveAlterarStatusParaProcessandoAoIniciarProcessamento()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();

        Assert.Equal(
            StatusImportacao.Processando,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoIniciarProcessamentoQuandoJaEstiverProcessando()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();

        Assert.Throws<InvalidOperationException>(
            () => importacao.IniciarProcessamento());
    }

    [Fact]
    public void DeveAlterarStatusParaConcluidaAoConcluirProcessamento()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();

        importacao.Concluir();

        Assert.Equal(
            StatusImportacao.Concluida,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoConcluirQuandoNaoEstiverProcessando()
    {
        var importacao = CriarImportacao();

        Assert.Throws<InvalidOperationException>(
            () => importacao.Concluir());
    }

    [Fact]
    public void DeveAlterarStatusParaFalhouAoFalharProcessamento()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();

        importacao.Falhar();

        Assert.Equal(
            StatusImportacao.Falhou,
            importacao.Status);
    }

    [Fact]
    public void DeveLancarExcecaoAoFalharQuandoNaoEstiverProcessando()
    {
        var importacao = CriarImportacao();

        Assert.Throws<InvalidOperationException>(
            () => importacao.Falhar());
    }

    [Fact]
    public void DeveConcluirComErrosQuandoExistiremErrosRegistrados()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();
        importacao.RegistrarErro();

        importacao.Concluir();

        Assert.Equal(
            StatusImportacao.ConcluidaComErros,
            importacao.Status);
    }

    [Fact]
    public void DeveConcluirSemErrosQuandoExistiremApenasDuplicados()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();
        importacao.RegistrarSucesso();
        importacao.RegistrarDuplicado();

        importacao.Concluir();

        Assert.Equal(
            StatusImportacao.Concluida,
            importacao.Status);
    }

    private static Importacao CriarImportacao()
    {
        return new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));
    }
}