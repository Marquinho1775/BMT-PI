use BMT_database;
go 

-- Renombrar tabla de ProductTags
sp_rename 'Product_Tags', 'ProductTags';

-- Eliminar tabla actual de imágenes y recrearla
drop table Images;

create table ProductImages (
	Id					uniqueidentifier	not null primary key default newid(),
	ProductId			uniqueidentifier	not null foreign key references Products(Id),
	URL					varchar(255)		not null unique,
);

-- Añadir a usuario una columna para la dirección de la imágen de perfil
alter table Users add ProfilePictureURL varchar(255) not null DEFAULT 'default_url';

-- Modificar las restricciones de FK que referencian a products para que se haga un on delete cascade 
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
	ALTER TABLE ProductTags ADD CONSTRAINT FK_ProductTags_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'ProductImages' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE ProductImages DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
	ALTER TABLE ProductImages ADD CONSTRAINT FK_ProductImages_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'PerishableProducts' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE PerishableProducts DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
	ALTER TABLE PerishableProducts ADD CONSTRAINT FK_PerishableProducts_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'NonPerishableProducts' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE NonPerishableProducts DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
	ALTER TABLE NonPerishableProducts ADD CONSTRAINT FK_NonPerishableProducts_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;

SELECT @ConstraintName = fk.name
FROM sys.foreign_keys fk
INNER JOIN sys.objects o ON fk.parent_object_id = o.object_id
WHERE o.name = 'DateDisponibility' AND fk.referenced_object_id = OBJECT_ID('Products');
IF @ConstraintName IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE DateDisponibility DROP CONSTRAINT [' + @ConstraintName + '];';
    EXEC sp_executesql @sql;
	ALTER TABLE DateDisponibility ADD CONSTRAINT FK_DateDisponibility_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;

-- temp querys
--select * from Products;
-- * from Enterprises;
--select * from ProductImages;

-- delete from Products where Name = 'Producto1'