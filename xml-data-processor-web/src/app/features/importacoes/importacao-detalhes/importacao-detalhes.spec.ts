import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ImportacaoDetalhes } from './importacao-detalhes';

describe('ImportacaoDetalhes', () => {
  let component: ImportacaoDetalhes;
  let fixture: ComponentFixture<ImportacaoDetalhes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportacaoDetalhes],
    }).compileComponents();

    fixture = TestBed.createComponent(ImportacaoDetalhes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
