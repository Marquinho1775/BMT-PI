CREATE PROCEDURE UpdateForeignKeyWithCascade
    @ChildTable NVARCHAR(128),
    @ChildColumn NVARCHAR(128),
    @ParentTable NVARCHAR(128),
    @ParentColumn NVARCHAR(128) = 'Id',
    @ConstraintName NVARCHAR(128) = NULL
AS
BEGIN
    DECLARE @ConstraintToDrop NVARCHAR(128);
    DECLARE @sql NVARCHAR(MAX);

    -- Encontrar el nombre de la restricción FK existente
    SELECT TOP 1 @ConstraintToDrop = fk.name
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.tables ct ON fk.parent_object_id = ct.object_id
    INNER JOIN sys.columns cc ON fkc.parent_object_id = cc.object_id AND fkc.parent_column_id = cc.column_id
    INNER JOIN sys.tables pt ON fk.referenced_object_id = pt.object_id
    INNER JOIN sys.columns pc ON fkc.referenced_object_id = pc.object_id AND fkc.referenced_column_id = pc.column_id
    WHERE ct.name = @ChildTable
      AND pt.name = @ParentTable
      AND cc.name = @ChildColumn
      AND pc.name = @ParentColumn;

    IF @ConstraintToDrop IS NOT NULL
    BEGIN
        -- Eliminar la restricción FK existente
        SET @sql = N'ALTER TABLE [' + @ChildTable + '] DROP CONSTRAINT [' + @ConstraintToDrop + '];';
        EXEC sp_executesql @sql;

        -- Asignar un nombre a la nueva restricción si no se proporcionó
        IF @ConstraintName IS NULL
        BEGIN
            SET @ConstraintName = 'FK_' + @ChildTable + '_' + @ChildColumn;
        END

        -- Crear la nueva restricción FK con ON DELETE CASCADE
        SET @sql = N'ALTER TABLE [' + @ChildTable + '] ADD CONSTRAINT [' + @ConstraintName + '] FOREIGN KEY ([' + @ChildColumn + ']) REFERENCES [' + @ParentTable + ']([' + @ParentColumn + ']) ON DELETE CASCADE;';
        EXEC sp_executesql @sql;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró una restricción FK que coincida con los parámetros proporcionados.';
    END
END;
