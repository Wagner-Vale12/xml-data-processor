import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { ImportacoesService } from '../../../core/services/importacoes';
import { Importacao } from '../../../models/importacao';
import { ImportacaoForm } from '../importacao-form/importacao-form';
import { ImportacaoDetalhes } from '../importacao-detalhes/importacao-detalhes';

@Component({
  selector: 'app-importacoes-list',
  standalone: true,
  imports: [CommonModule, ImportacaoForm, ImportacaoDetalhes],
  templateUrl: './importacoes-list.html',
  styleUrl: './importacoes-list.css',
})
export class ImportacoesList {
  private readonly importacoesService = inject(ImportacoesService);

  readonly importacoes = signal<Importacao[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal<string | null>(null);
  readonly processandoId = signal<number | null>(null);
  readonly sucesso = signal<string | null>(null);
  readonly exibindoFormulario = signal(false);
  readonly importacaoSelecionada = signal<Importacao | null>(null);
  readonly termoBusca = signal('');
  readonly statusFiltro = signal<number | null>(null);

  verDetalhes(importacao: Importacao): void {
    this.erro.set(null);

    this.importacoesService.obterPorId(importacao.id).subscribe({
      next: (detalhes) => {
        this.importacaoSelecionada.set(detalhes);
      },

      error: (erro) => {
        console.error('Erro ao buscar detalhes da importação:', erro);

        const mensagem =
          erro?.error?.detail ?? 'Não foi possível carregar os detalhes da importação.';

        this.erro.set(mensagem);
      },
    });
  }

  fecharDetalhes(): void {
    this.importacaoSelecionada.set(null);
  }

  importacoesFiltradas(): Importacao[] {
    const termo = this.termoBusca().trim().toLowerCase();

    const status = this.statusFiltro();

    return this.importacoes().filter((importacao) => {
      const correspondeAoTermo =
        !termo ||
        importacao.nomeArquivo.toLowerCase().includes(termo) ||
        importacao.id.toString().includes(termo);

      const correspondeAoStatus = status === null || importacao.status === status;

      return correspondeAoTermo && correspondeAoStatus;
    });
  }

  quantidadeFiltrada(): number {
    return this.importacoesFiltradas().length;
  }

  constructor() {
    this.carregarImportacoes();
  }

  carregarImportacoes(): void {
    this.carregando.set(true);
    this.erro.set(null);

    this.importacoesService.listar().subscribe({
      next: (importacoes) => {
        this.importacoes.set(importacoes);
        this.carregando.set(false);
      },
      error: (erro) => {
        console.error('Erro ao carregar importações:', erro);

        this.erro.set('Não foi possível carregar as importações.');

        this.carregando.set(false);
      },
    });
  }

  statusTexto(status: number): string {
    switch (status) {
      case 0:
        return 'Recebida';

      case 1:
        return 'Processando';

      case 2:
        return 'Concluída';

      case 3:
        return 'Concluída com erros';

      case 4:
        return 'Falhou';

      default:
        return 'Desconhecido';
    }
  }

  processar(importacao: Importacao): void {
    this.processandoId.set(importacao.id);

    this.importacoesService.processar(importacao.id).subscribe({
      next: () => {
        this.processandoId.set(null);

        this.sucesso.set(`Importação #${importacao.id} processada com sucesso.`);

        this.carregarImportacoes();

        setTimeout(() => {
          this.sucesso.set(null);
        }, 4000);
      },
      error: (erro) => {
        console.error('Erro ao processar importação:', erro);

        const mensagem = erro?.error?.detail ?? 'Não foi possível processar a importação.';

        this.erro.set(mensagem);

        this.processandoId.set(null);
      },
    });
  }

  abrirFormulario(): void {
    this.exibindoFormulario.set(true);
  }

  fecharFormulario(): void {
    this.exibindoFormulario.set(false);
  }

  aoCriarImportacao(): void {
    this.exibindoFormulario.set(false);

    this.sucesso.set('Importação criada com sucesso.');

    this.carregarImportacoes();

    setTimeout(() => {
      this.sucesso.set(null);
    }, 4000);
  }

  totalImportacoes(): number {
    return this.importacoes().length;
  }

  totalRecebidas(): number {
    return this.importacoes().filter((importacao) => importacao.status === 0).length;
  }

  totalConcluidas(): number {
    return this.importacoes().filter((importacao) => importacao.status === 2).length;
  }

  totalComProblema(): number {
    return this.importacoes().filter(
      (importacao) => importacao.status === 3 || importacao.status === 4,
    ).length;
  }
}
