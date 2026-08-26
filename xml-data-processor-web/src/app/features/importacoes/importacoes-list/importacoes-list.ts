import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { ImportacoesService } from '../../../core/services/importacoes';
import { Importacao } from '../../../models/importacao';
import { ImportacaoForm } from '../importacao-form/importacao-form';

@Component({
  selector: 'app-importacoes-list',
  standalone: true,
  imports: [CommonModule, ImportacaoForm],
  templateUrl: './importacoes-list.html',
  styleUrl: './importacoes-list.css',
})
export class ImportacoesList {
  private readonly importacoesService = inject(ImportacoesService);

  readonly importacoes = signal<Importacao[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal<string | null>(null);

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

  readonly processandoId = signal<number | null>(null);

  processar(importacao: Importacao): void {
    this.processandoId.set(importacao.id);

    this.importacoesService.processar(importacao.id).subscribe({
      next: () => {
        this.processandoId.set(null);
        this.carregarImportacoes();
      },
      error: (erro) => {
        console.error('Erro ao processar importação:', erro);

        this.erro.set('Não foi possível processar a importação.');

        this.processandoId.set(null);
      },
    });
  }

  readonly exibindoFormulario = signal(false);

  abrirFormulario(): void {
    this.exibindoFormulario.set(true);
  }

  fecharFormulario(): void {
    this.exibindoFormulario.set(false);
  }

  aoCriarImportacao(): void {
    this.exibindoFormulario.set(false);
    this.carregarImportacoes();
  }
}
