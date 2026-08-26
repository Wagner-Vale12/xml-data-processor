import { TestBed } from '@angular/core/testing';
import { Importacoes } from './importacoes';

describe('Importacoes', () => {
  let service: Importacoes;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Importacoes);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
