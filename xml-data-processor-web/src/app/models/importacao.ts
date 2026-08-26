export interface Importacao {
  id: number;
  nomeArquivo: string;
  dataRecebimento: string;
  status: number;
  totalRegistros: number;
  totalSucessos: number;
  totalErros: number;
  totalDuplicados: number;
}