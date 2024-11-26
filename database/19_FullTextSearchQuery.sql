-- 1. Verificar y crear el catálogo de texto completo si no existe
IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'FTCatalog')
BEGIN
    CREATE FULLTEXT CATALOG FTCatalog AS DEFAULT;
    PRINT 'Catálogo de texto completo "FTCatalog" creado.'
END
ELSE
BEGIN
    PRINT 'Catálogo de texto completo "FTCatalog" ya existe.'
END
GO

-- 2. Función para obtener el nombre del índice de clave primaria de una tabla
CREATE OR ALTER FUNCTION dbo.GetPrimaryKeyIndexName(@TableSchema SYSNAME, @TableName SYSNAME)
RETURNS SYSNAME
AS
BEGIN
    DECLARE @IndexName SYSNAME;

    SELECT @IndexName = i.name
    FROM sys.indexes i
    INNER JOIN sys.key_constraints kc
        ON i.object_id = kc.parent_object_id
        AND i.index_id = kc.unique_index_id
    WHERE kc.parent_object_id = OBJECT_ID(QUOTENAME(@TableSchema) + '.' + QUOTENAME(@TableName))
        AND kc.type = 'PK';

    RETURN @IndexName;
END
GO

-- 3. Procedimiento para crear índices de texto completo dinámicamente
CREATE OR ALTER PROCEDURE dbo.CreateFullTextIndex
    @SchemaName SYSNAME,
    @TableName SYSNAME,
    @Columns NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IndexName SYSNAME;
    DECLARE @SQL NVARCHAR(MAX);

    -- Obtener el nombre del índice de clave primaria
    SET @IndexName = dbo.GetPrimaryKeyIndexName(@SchemaName, @TableName);

    IF @IndexName IS NULL
    BEGIN
        PRINT 'No se encontró un índice de clave primaria para la tabla ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName) + '.';
        RETURN;
    END

    -- Construir la sentencia SQL para crear el índice de texto completo
    SET @SQL = '
        IF NOT EXISTS (
            SELECT * FROM sys.fulltext_indexes fi
            INNER JOIN sys.objects o ON fi.object_id = o.object_id
            WHERE o.name = ''' + @TableName + ''' AND SCHEMA_NAME(o.schema_id) = ''' + @SchemaName + '''
        )
        BEGIN
            CREATE FULLTEXT INDEX ON ' + QUOTENAME(@SchemaName) + '.' + QUOTENAME(@TableName) + '(' + @Columns + ')
            KEY INDEX ' + QUOTENAME(@IndexName) + ' ON FTCatalog;
            PRINT ''Índice de texto completo creado para ' + @SchemaName + '.' + @TableName + ''' 
        END
        ELSE
        BEGIN
            PRINT ''El índice de texto completo ya existe para ' + @SchemaName + '.' + @TableName + ''' 
        END
    ';

    -- Ejecutar la sentencia SQL dinámica
    EXEC sp_executesql @SQL;
END
GO

-- 4. Ejecutar el procedimiento para las tablas "Products" y "Enterprises"
EXEC dbo.CreateFullTextIndex @SchemaName = 'dbo', @TableName = 'Products', @Columns = 'Name, Description';
GO

EXEC dbo.CreateFullTextIndex @SchemaName = 'dbo', @TableName = 'Enterprises', @Columns = 'Name, Description';
GO

-- 5. Limpiar objetos creados (opcional)
-- DROP PROCEDURE dbo.CreateFullTextIndex;
-- DROP FUNCTION dbo.GetPrimaryKeyIndexName;
-- DROP FULLTEXT CATALOG FTCatalog;
