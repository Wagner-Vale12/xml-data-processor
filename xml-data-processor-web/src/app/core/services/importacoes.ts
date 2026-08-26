import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { Importacao } from '../../models/importacao';
import { CriarImportacaoRequest } from '../../models/criar-importacao-request';

@Injectable({
  providedIn: 'root',
})
export class ImportacoesService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5287/api/Importacoes';

  listar(): Observable<Importacao[]> {
    return this.http.get<Importacao[]>(this.apiUrl);
  }

  processar(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/processar`, {});
  }

  criar(request: CriarImportacaoRequest): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.apiUrl, request);
  }
}
