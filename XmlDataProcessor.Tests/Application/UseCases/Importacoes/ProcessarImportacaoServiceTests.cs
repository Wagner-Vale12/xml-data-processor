using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Domain.Entities;
using XmlDataProcessor.Domain.Enums;
using XmlDataProcessor.Application.Abstractions.Xml;
using Xunit;

namespace XmlDataProcessor.Tests.Application.UseCases.Importacoes;

public class ProcessarImportacaoServiceTests
{
    private class ImportacaoRepositoryFake : IImportacaoRepository
    {
        public long? IdConsultado { get; private set; }

        public Importacao? ImportacaoRetornada { get; set; }

        public Importacao? ImportacaoAtualizada { get; private set; }

        public Task AdicionarAsync(Importacao importacao)
        {
            return Task.CompletedTask;
        }

        public Task<Importacao?> ObterPorIdAsync(long id)
        {
            IdConsultado = id;

            return Task.FromResult(ImportacaoRetornada);
        }

        public Task<IReadOnlyCollection<Importacao>> ListarAsync()
        {
            IReadOnlyCollection<Importacao> importacoes =
                Array.Empty<Importacao>();

            return Task.FromResult(importacoes);
        }

        public Task AtualizarAsync(Importacao importacao)
        {
            ImportacaoAtualizada = importacao;

            return Task.CompletedTask;
        }
    }

    private class LeitorMovimentosXmlFake : ILeitorMovimentosXml
    {
        public string? NomeArquivoRecebido { get; private set; }
        public bool DeveFalhar { get; set; }

        public IReadOnlyCollection<Movimento> MovimentosRetornados { get; set; }
            = Array.Empty<Movimento>();

        public Task<IReadOnlyCollection<Movimento>> LerAsync(
     string nomeArquivo)
        {
            NomeArquivoRecebido = nomeArquivo;

            if (DeveFalhar)
            {
                throw new InvalidOperationException(
                    "Falha simulada na leitura do XML.");
            }

            return Task.FromResult(MovimentosRetornados);
        }
    }

    private class MovimentoRepositoryFake : IMovimentoRepository
    {
        public string? IdExternoConsultado { get; private set; }

        public bool MovimentoExiste { get; set; }

        public Movimento? MovimentoAdicionado { get; private set; }

        public string? IdExternoQueDeveFalhar { get; set; }

        public Task<bool> ExistePorIdExternoAsync(string idExterno)
        {
            IdExternoConsultado = idExterno;

            return Task.FromResult(MovimentoExiste);
        }

        public Task AdicionarAsync(Movimento movimento)
        {
            if (movimento.IdExterno == IdExternoQueDeveFalhar)
            {
                throw new InvalidOperationException(
                    "Falha simulada ao adicionar movimento.");
            }

            MovimentoAdicionado = movimento;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DeveBuscarImportacaoPeloIdInformado()
    {
        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = new Importacao(
                "movimentos-2026-08-21.xml",
                new DateTime(2026, 8, 21, 10, 30, 0))
        };

        var leitorXml = new LeitorMovimentosXmlFake();
        var movimentoRepository = new MovimentoRepositoryFake();
        var service = new ProcessarImportacaoService(repository, leitorXml, movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(15, repository.IdConsultado);
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoImportacaoNaoForEncontrada()
    {
        var repository = new ImportacaoRepositoryFake();
        var leitorXml = new LeitorMovimentosXmlFake();
        var movimentoRepository = new MovimentoRepositoryFake();
        var service = new ProcessarImportacaoService(repository, leitorXml, movimentoRepository);

            await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecutarAsync(15));
    }

    [Fact]
    public async Task DeveSolicitarLeituraDoArquivoDaImportacao()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake();

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            new MovimentoRepositoryFake());

        await service.ExecutarAsync(15);

        Assert.Equal(
            "movimentos-2026-08-24.xml",
            leitorXml.NomeArquivoRecebido);
    }

    [Fact]
    public async Task DeveVerificarDuplicidadeDosMovimentosLidos()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[] { movimento }
        };

        var movimentoRepository = new MovimentoRepositoryFake();

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(
            "MOV-001",
            movimentoRepository.IdExternoConsultado);
    }

    [Fact]
    public async Task DeveRegistrarDuplicadoQuandoMovimentoJaExistir()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[] { movimento }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            MovimentoExiste = true
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(1, importacao.TotalDuplicados);
    }

    [Fact]
    public async Task DeveAdicionarMovimentoQuandoNaoForDuplicado()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[] { movimento }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            MovimentoExiste = false
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Same(
            movimento,
            movimentoRepository.MovimentoAdicionado);
    }

    [Fact]
    public async Task DeveRegistrarSucessoQuandoMovimentoForAdicionado()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[] { movimento }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            MovimentoExiste = false
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(1, importacao.TotalSucessos);
    }

    [Fact]
    public async Task DeveRegistrarErroQuandoFalharAoAdicionarMovimento()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[] { movimento }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            MovimentoExiste = false,
            IdExternoQueDeveFalhar = "MOV-001"
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(1, importacao.TotalErros);
        Assert.Equal(0, importacao.TotalSucessos);
    }

    [Fact]
    public async Task DeveContinuarProcessamentoQuandoUmMovimentoFalhar()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimentoComErro = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var movimentoValido = new Movimento(
            "MOV-002",
            TipoMovimento.Saida,
            200m,
            new DateTime(2026, 8, 24),
            "DOC-002");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[]
            {
            movimentoComErro,
            movimentoValido
        }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            IdExternoQueDeveFalhar = "MOV-001"
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(2, importacao.TotalRegistros);
        Assert.Equal(1, importacao.TotalErros);
        Assert.Equal(1, importacao.TotalSucessos);
    }

    [Fact]
    public async Task DeveConcluirImportacaoQuandoTodosMovimentosForemProcessadosComSucesso()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[]
            {
            movimento
        }
        };

        var movimentoRepository = new MovimentoRepositoryFake();

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(
            StatusImportacao.Concluida,
            importacao.Status);
    }

    [Fact]
    public async Task DeveConcluirImportacaoComErrosQuandoAlgumMovimentoFalhar()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimentoComErro = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var movimentoValido = new Movimento(
            "MOV-002",
            TipoMovimento.Saida,
            200m,
            new DateTime(2026, 8, 24),
            "DOC-002");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[]
            {
            movimentoComErro,
            movimentoValido
        }
        };

        var movimentoRepository = new MovimentoRepositoryFake
        {
            IdExternoQueDeveFalhar = "MOV-001"
        };

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Equal(
            StatusImportacao.ConcluidaComErros,
            importacao.Status);

        Assert.Equal(1, importacao.TotalErros);
        Assert.Equal(1, importacao.TotalSucessos);
    }

    [Fact]
    public async Task DeveAtualizarImportacaoAposProcessamento()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var movimento = new Movimento(
            "MOV-001",
            TipoMovimento.Entrada,
            100m,
            new DateTime(2026, 8, 24),
            "DOC-001");

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            MovimentosRetornados = new[]
            {
            movimento
        }
        };

        var movimentoRepository = new MovimentoRepositoryFake();

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await service.ExecutarAsync(15);

        Assert.Same(
            importacao,
            repository.ImportacaoAtualizada);
    }

    [Fact]
    public async Task DeveMarcarImportacaoComErroQuandoLeituraXmlFalhar()
    {
        var importacao = new Importacao(
            "movimentos-2026-08-24.xml",
            new DateTime(2026, 8, 24, 10, 30, 0));

        var repository = new ImportacaoRepositoryFake
        {
            ImportacaoRetornada = importacao
        };

        var leitorXml = new LeitorMovimentosXmlFake
        {
            DeveFalhar = true
        };

        var movimentoRepository = new MovimentoRepositoryFake();

        var service = new ProcessarImportacaoService(
            repository,
            leitorXml,
            movimentoRepository);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecutarAsync(15));

        Assert.Equal(
     StatusImportacao.Falhou,
     importacao.Status);
    }
}