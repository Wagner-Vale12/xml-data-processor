import { Component } from '@angular/core';
import { ImportacoesList } from './features/importacoes/importacoes-list/importacoes-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ImportacoesList],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
