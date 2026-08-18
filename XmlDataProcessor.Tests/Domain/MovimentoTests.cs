using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;

namespace XmlDataProcessor.Tests.Domain;

public class MovimentoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveLancarExcecaoQuandoIdExternoForInvalido(string? idExterno)
    {
        Assert.Throws<ArgumentException>(
            () => new Movimento(
                idExterno!,
                TipoMovimento.Entrada,
                100m, new DateTime(2026, 8, 17), "DOC-001"));
    }

    [Fact]
    public void DeveCriarMovimentoQuandoIdExternoForValido()
    {
        var idExterno = "MOV-001";

        var movimento = new Movimento(
            idExterno,
            TipoMovimento.Entrada,
            100m, new DateTime(2026, 8, 17), "DOC-001");

        Assert.Equal(idExterno, movimento.IdExterno);
    }

    [Fact]
    public void DeveCriarMovimentoComTipoInformado()
    {
        var movimento = CriarMovimentoValido();

        Assert.Equal(TipoMovimento.Entrada, movimento.Tipo);
    }

    [Fact]
    public void DeveCriarMovimentoComValorInformado()
    {
        var valor = 150.75m;

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            valor, new DateTime(2026, 8, 17), "DOC-001");

        Assert.Equal(valor, movimento.Valor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeveLancarExcecaoQuandoValorForInvalido(int valor)
    {
        Assert.Throws<ArgumentException>(
            () => new Movimento(
                "MOV-001",
                TipoMovimento.Entrada,
                valor, new DateTime(2026, 8, 17), "DOC-001"));
    }

    [Fact]
    public void DeveLancarExcecaoQuandoDataMovimentoForInvalida()
    {
        Assert.Throws<ArgumentException>(
            () => new Movimento(
                "MOV-001",
                TipoMovimento.Entrada,
                100m,
                default(DateTime),
                "DOC-001"));
    }

    [Fact]
    public void DeveCriarMovimentoComDataMovimentoInformada()
    {
        var dataMovimento = new DateTime(2026, 8, 17);

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            dataMovimento,
            "DOC-001");

        Assert.Equal(dataMovimento, movimento.DataMovimento);
    }

    [Fact]
    public void DeveRemoverEspacosDoDocumento()
    {
        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 17),
            "   DOC-123   ");

        Assert.Equal("DOC-123", movimento.Documento);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveDefinirDocumentoComoNuloQuandoNaoForInformado(string? documento)
    {
        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 17),
            documento);

        Assert.Null(movimento.Documento);
    }

    private static Movimento CriarMovimentoValido()
    {
        return new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m, new DateTime(2026, 8, 17), "DOC-001");
    }
}