import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ImportacaoForm } from './importacao-form';

describe('ImportacaoForm', () => {
  let component: ImportacaoForm;
  let fixture: ComponentFixture<ImportacaoForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportacaoForm],
    }).compileComponents();

    fixture = TestBed.createComponent(ImportacaoForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
