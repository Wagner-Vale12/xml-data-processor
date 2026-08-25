CREATE TABLE Importacoes
(
    Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NomeArquivo NVARCHAR(255) NOT NULL,
    DataRecebimento DATETIME2 NOT NULL,
    Status INT NOT NULL,
    TotalRegistros INT NOT NULL DEFAULT 0,
    TotalSucessos INT NOT NULL DEFAULT 0,
    TotalErros INT NOT NULL DEFAULT 0,
    TotalDuplicados INT NOT NULL DEFAULT 0
);

CREATE TABLE Movimentos
(
    Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdExterno NVARCHAR(100) NOT NULL,
    Tipo INT NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    DataMovimento DATETIME2 NOT NULL,
    Documento NVARCHAR(100) NULL,

    CONSTRAINT UQ_Movimentos_IdExterno
        UNIQUE (IdExterno)
);