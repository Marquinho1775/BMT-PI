use BMT_database;
go 

-- Renombrar tabla de Product/Tags
sp_rename 'Product_Tags', 'ProductTags';

-- Eliminar tabla actual de imágenes
drop table Images;

-- Crear nueva tabla de imágenes
create table ProductImages (
	Id					uniqueidentifier	not null primary key default newid(),
	ProductId				uniqueidentifier	not null foreign key references Products(Id),
	URL					varchar(255)		not null unique,
);

-- Añadir a usuario una columna para la dirección de la imágen de perfil
alter table Users add ProfilePictureURL varchar(255)

-- Modificar las restricciones de FK que referencian a products para que se haga un on delete cascade (ProductTags & ProductImages) 
DECLARE @ConstraintName NVARCHAR(200);
DECLARE @sql NVARCHAR(MAX); 

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'ProductTags' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE ProductTags DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
END

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'ProductImages' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE ProductImages DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
END

-- Crear la clave foránea en ProductTags con ON DELETE CASCADE
ALTER TABLE ProductTags
ADD CONSTRAINT FK_ProductTags_ProductId FOREIGN KEY (ProductId)
REFERENCES Products(Id) ON DELETE CASCADE;

-- Crear la clave foránea en Images con ON DELETE CASCADE
ALTER TABLE ProductImages
ADD CONSTRAINT FK_ProductImages_ProductId FOREIGN KEY (ProductId)
REFERENCES Products(Id) ON DELETE CASCADE;