using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Tests.Domain.ImportacaoTests;

public class ImportacaoProcessamentoTests
{
    [Fact]
    public void DeveRegistrarSucessoEIncrementarContadores()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarSucesso();

        Assert.Equal(1, importacao.TotalRegistros);
        Assert.Equal(1, importacao.TotalSucessos);
    }

    [Fact]
    public void DeveLancarExcecaoAoRegistrarSucessoQuandoNaoEstiverProcessando()
    {
        var importacao = CriarImportacao();

        Assert.Throws<InvalidOperationException>(
            () => importacao.RegistrarSucesso());
    }

    [Fact]
    public void DeveAcumularContadoresAoRegistrarMultiplosSucessos()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarSucesso();
        importacao.RegistrarSucesso();
        importacao.RegistrarSucesso();

        Assert.Equal(3, importacao.TotalRegistros);
        Assert.Equal(3, importacao.TotalSucessos);
    }

    [Fact]
    public void DeveRegistrarErroEIncrementarContadores()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarErro();

        Assert.Equal(1, importacao.TotalRegistros);
        Assert.Equal(1, importacao.TotalErros);
    }

    [Fact]
    public void DeveLancarExcecaoAoRegistrarErroQuandoNaoEstiverProcessando()
    {
        var importacao = CriarImportacao();

        Assert.Throws<InvalidOperationException>(
            () => importacao.RegistrarErro());
    }

    [Fact]
    public void DeveAcumularContadoresAoRegistrarMultiplosErros()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarErro();
        importacao.RegistrarErro();
        importacao.RegistrarErro();

        Assert.Equal(3, importacao.TotalRegistros);
        Assert.Equal(3, importacao.TotalErros);
    }

    [Fact]
    public void DeveRegistrarDuplicadoEIncrementarContadores()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarDuplicado();

        Assert.Equal(1, importacao.TotalRegistros);
        Assert.Equal(1, importacao.TotalDuplicados);
    }

    [Fact]
    public void DeveLancarExcecaoAoRegistrarDuplicadoQuandoNaoEstiverProcessando()
    {
        var importacao = CriarImportacao();

        Assert.Throws<InvalidOperationException>(
            () => importacao.RegistrarDuplicado());
    }

    [Fact]
    public void DeveAcumularContadoresAoRegistrarMultiplosDuplicados()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarDuplicado();
        importacao.RegistrarDuplicado();
        importacao.RegistrarDuplicado();

        Assert.Equal(3, importacao.TotalRegistros);
        Assert.Equal(3, importacao.TotalDuplicados);
    }

    [Fact]
    public void DeveManterTotalRegistrosConsistenteComResultadosProcessados()
    {
        var importacao = CriarImportacaoProcessando();

        importacao.RegistrarSucesso();
        importacao.RegistrarSucesso();
        importacao.RegistrarErro();
        importacao.RegistrarDuplicado();

        Assert.Equal(4, importacao.TotalRegistros);
        Assert.Equal(2, importacao.TotalSucessos);
        Assert.Equal(1, importacao.TotalErros);
        Assert.Equal(1, importacao.TotalDuplicados);
    }

    private static Importacao CriarImportacao()
    {
        return new Importacao(
            "movimentos-2026-08-18.xml",
            new DateTime(2026, 8, 18, 14, 30, 0));
    }

    private static Importacao CriarImportacaoProcessando()
    {
        var importacao = CriarImportacao();

        importacao.IniciarProcessamento();

        return importacao;
    }
}