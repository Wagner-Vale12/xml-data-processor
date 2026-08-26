import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ImportacoesList } from './importacoes-list';

describe('ImportacoesList', () => {
  let component: ImportacoesList;
  let fixture: ComponentFixture<ImportacoesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportacoesList],
    }).compileComponents();

    fixture = TestBed.createComponent(ImportacoesList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
