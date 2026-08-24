using Xunit;
using XmlDataProcessor.Domain.Enums;
using XmlDataProcessor.Infrastructure.Xml;

namespace XmlDataProcessor.Tests.Infrastructure.Xml;

public class LeitorMovimentosXmlTests
{
    [Fact]
    public async Task DeveLerUmMovimentoDeXmlValido()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <Valor>150.50</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var movimentos = await leitor.LerAsync(
                caminhoArquivo);

            var movimento = Assert.Single(movimentos);

            Assert.Equal("MOV-001", movimento.IdExterno);
            Assert.Equal(TipoMovimento.Entrada, movimento.Tipo);
            Assert.Equal(150.50m, movimento.Valor);
            Assert.Equal(
                new DateTime(2026, 8, 24),
                movimento.DataMovimento);
            Assert.Equal("DOC-001", movimento.Documento);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLerMultiplosMovimentosDeXmlValido()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <Valor>150.50</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>

          <Movimento>
            <IdExterno>MOV-002</IdExterno>
            <Tipo>Saida</Tipo>
            <Valor>75.25</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-002</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var movimentos = await leitor.LerAsync(
                caminhoArquivo);

            Assert.Equal(2, movimentos.Count);

            Assert.Contains(
                movimentos,
                movimento => movimento.IdExterno == "MOV-001");

            Assert.Contains(
                movimentos,
                movimento => movimento.IdExterno == "MOV-002");
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoIdExternoNaoExistir()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <Tipo>Entrada</Tipo>
            <Valor>150.50</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "IdExterno",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoTipoNaoExistir()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Valor>150.50</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "Tipo",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoValorNaoExistir()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "Valor",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoDataMovimentoNaoExistir()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <Valor>150.50</Valor>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "DataMovimento",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoValorForInvalido()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <Valor>ABC</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "Valor",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoTipoForInvalido()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Transferencia</Tipo>
            <Valor>150.50</Valor>
            <DataMovimento>2026-08-24</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "Tipo",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoDataMovimentoForInvalida()
    {
        var caminhoArquivo = Path.GetTempFileName();

        try
        {
            var xml = """
        <Movimentos>
          <Movimento>
            <IdExterno>MOV-001</IdExterno>
            <Tipo>Entrada</Tipo>
            <Valor>150.50</Valor>
            <DataMovimento>DATA-ERRADA</DataMovimento>
            <Documento>DOC-001</Documento>
          </Movimento>
        </Movimentos>
        """;

            await File.WriteAllTextAsync(
                caminhoArquivo,
                xml);

            var leitor = new LeitorMovimentosXml();

            var exception =
                await Assert.ThrowsAsync<InvalidDataException>(
                    () => leitor.LerAsync(caminhoArquivo));

            Assert.Contains(
                "DataMovimento",
                exception.Message);
        }
        finally
        {
            File.Delete(caminhoArquivo);
        }
    }
}