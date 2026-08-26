import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ImportacoesService } from '../../../core/services/importacoes';
import { CriarImportacaoRequest } from '../../../models/criar-importacao-request';

@Component({
  selector: 'app-importacao-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './importacao-form.html',
  styleUrl: './importacao-form.css',
})
export class ImportacaoForm {
  private readonly formBuilder = inject(FormBuilder);
  private readonly importacoesService = inject(ImportacoesService);

  readonly importacaoCriada = output<number>();
  readonly cancelar = output<void>();

  readonly salvando = signal(false);
  readonly erro = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    nomeArquivo: ['', Validators.required],
    dataRecebimento: ['', Validators.required],
  });

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando.set(true);
    this.erro.set(null);

    const request: CriarImportacaoRequest = {
      nomeArquivo: this.form.controls.nomeArquivo.value,
      dataRecebimento: this.form.controls.dataRecebimento.value,
    };

    this.importacoesService.criar(request).subscribe({
      next: (response) => {
        this.salvando.set(false);

        this.importacaoCriada.emit(response.id);
      },

      error: (erro) => {
        console.error('Erro ao criar importação:', erro);

        this.erro.set('Não foi possível criar a importação.');

        this.salvando.set(false);
      },
    });
  }
}
