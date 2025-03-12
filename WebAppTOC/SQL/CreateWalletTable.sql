-- Crear base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WalletDB')
BEGIN
    CREATE DATABASE WalletDB;
END
GO

USE WalletDB;
GO

-- Crear tabla Wallets
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Wallets')
BEGIN
    CREATE TABLE Wallets
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        DocumentId INT NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Balance DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL
    );
END
GO

-- Crear tabla Transactions (Historial de movimientos)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Transactions')
BEGIN
    CREATE TABLE Transactions
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        WalletId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Type NVARCHAR(10) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Transactions_Wallets FOREIGN KEY (WalletId) 
            REFERENCES Wallets(Id) ON DELETE CASCADE
    );
END
GO

-- Índice para búsquedas por DocumentId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Wallets_DocumentId')
BEGIN
    CREATE INDEX IX_Wallets_DocumentId ON Wallets(DocumentId);
END
GO

-- Índice para búsquedas de transacciones por WalletId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_WalletId')
BEGIN
    CREATE INDEX IX_Transactions_WalletId ON Transactions(WalletId);
END
GO

-- Crear un procedimiento almacenado para actualizar el balance y la fecha de actualización
CREATE OR ALTER PROCEDURE sp_UpdateWalletBalance
    @WalletId INT,
    @NewBalance DECIMAL(18,2)
AS
BEGIN
    UPDATE Wallets
    SET Balance = @NewBalance,
        UpdatedAt = GETDATE()
    WHERE Id = @WalletId;
END
GO

-- Crear un procedimiento almacenado para registrar una transacción y actualizar el balance
CREATE OR ALTER PROCEDURE sp_RegisterTransaction
    @WalletId INT,
    @Amount DECIMAL(18,2),
    @Type NVARCHAR(10)
AS
BEGIN
    DECLARE @CurrentBalance DECIMAL(18,2);
    
    -- Obtener el balance actual
    SELECT @CurrentBalance = Balance 
    FROM Wallets 
    WHERE Id = @WalletId;
    
    -- Registrar la transacción
    INSERT INTO Transactions (WalletId, Amount, Type)
    VALUES (@WalletId, @Amount, @Type);
    
    -- Actualizar el balance según el tipo de transacción
    -- Asumiendo que "DEPOSIT" suma al balance y "WITHDRAW" resta del balance
    DECLARE @NewBalance DECIMAL(18,2);
    
    IF @Type = 'DEPOSIT'
        SET @NewBalance = @CurrentBalance + @Amount;
    ELSE IF @Type = 'WITHDRAW'
        SET @NewBalance = @CurrentBalance - @Amount;
    ELSE
        SET @NewBalance = @CurrentBalance; -- Para otros tipos, mantener el balance
    
    -- Actualizar el balance y la fecha de actualización
    UPDATE Wallets
    SET Balance = @NewBalance,
        UpdatedAt = GETDATE()
    WHERE Id = @WalletId;
    
END
GO 