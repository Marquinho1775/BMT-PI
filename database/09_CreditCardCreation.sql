BEGIN TRANSACTION;

-- Creación de tabla CreditCard
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CreditCard' AND xtype='U')
BEGIN
    CREATE TABLE CreditCard (
        Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
        UserID          UNIQUEIDENTIFIER    NOT NULL FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
        Name            VARCHAR(255)        NOT NULL,
        Number          VARCHAR(255)        NOT NULL,
        MagicNumber     VARCHAR(255)        NOT NULL,
        DateVenc        VARCHAR(255)        NOT NULL
    );
END;
GO

COMMIT TRANSACTION;

-- Si ocurre algún error, realizar rollback
IF @@ERROR <> 0 
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error detected. Transaction rolled back.';
END;

