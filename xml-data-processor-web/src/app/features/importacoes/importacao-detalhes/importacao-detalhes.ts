import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';

import { Importacao } from '../../../models/importacao';

@Component({
  selector: 'app-importacao-detalhes',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './importacao-detalhes.html',
  styleUrl: './importacao-detalhes.css',
})
export class ImportacaoDetalhes {
  readonly importacao = input.required<Importacao>();
  readonly fechar = output<void>();

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
}