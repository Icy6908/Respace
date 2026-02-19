-- CreatePointsTransactions.sql
-- Run this script on the database referenced by your RespaceDb connection string
-- Example: open SSMS, connect to (localdb)\MSSQLLocalDB, choose the RespaceDb database, and execute.

IF OBJECT_ID('dbo.PointsTransactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PointsTransactions (
        TransactionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId INT NOT NULL,
        TxnType NVARCHAR(50) NOT NULL,
        Points INT NOT NULL,
        Reference NVARCHAR(200) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE INDEX IX_PointsTransactions_UserId ON dbo.PointsTransactions(UserId);
END
ELSE
BEGIN
    PRINT 'Table dbo.PointsTransactions already exists.';
END
