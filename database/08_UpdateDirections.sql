BEGIN TRANSACTION;

-- Eliminación de columnas en la tabla Directions, si existen
IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Province' AND object_id = OBJECT_ID('Directions'))
BEGIN
    ALTER TABLE Directions DROP COLUMN Province;
END;
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Canton' AND object_id = OBJECT_ID('Directions'))
BEGIN
    ALTER TABLE Directions DROP COLUMN Canton;
END;
GO

IF EXISTS (SELECT * FROM sys.columns WHERE name = 'District' AND object_id = OBJECT_ID('Directions'))
BEGIN
    ALTER TABLE Directions DROP COLUMN District;
END;
GO

-- Añadir columna Id a la tabla Directions, si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('Directions'))
BEGIN
    ALTER TABLE Directions ADD Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
END;
GO

-- Eliminar constraint de clave primaria existente y añadir uno nuevo
DECLARE @ConstraintName NVARCHAR(128);

SELECT @ConstraintName = kc.name
FROM sys.key_constraints AS kc
JOIN sys.tables AS t ON kc.parent_object_id = t.object_id
WHERE t.name = 'Directions' AND kc.type = 'PK';

IF @ConstraintName IS NOT NULL
BEGIN
    DECLARE @Sql NVARCHAR(MAX);
    SET @Sql = 'ALTER TABLE Directions DROP CONSTRAINT ' + QUOTENAME(@ConstraintName) + ';';
    EXEC sp_executesql @Sql;
END

-- Añadir nueva clave primaria
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Directions_Id')
BEGIN
    ALTER TABLE Directions
    ADD CONSTRAINT PK_Directions_Id PRIMARY KEY (Id);
END;
GO

-- Añadir columna DirectionId en la tabla Orders, si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'DirectionId' AND object_id = OBJECT_ID('Orders'))
BEGIN
    ALTER TABLE Orders ADD DirectionId UNIQUEIDENTIFIER;
END;
GO

-- Añadir restricción de clave foránea en Orders para DirectionId
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Direction')
BEGIN
    ALTER TABLE Orders
    ADD CONSTRAINT FK_Direction FOREIGN KEY (DirectionId) REFERENCES Directions(Id);
END;
GO

COMMIT TRANSACTION;

-- Si ocurre algún error, realizar rollback
IF @@ERROR <> 0 
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error detected. Transaction rolled back.';
END;
