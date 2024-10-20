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
alter table Users add ProfilePictureURL varchar(255) not null DEFAULT 'uploads/defualt_profile_image.png';

-- Modificar las restricciones de FK que referencian a products para que se haga un on delete cascade 
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'ProductTags',
    @ChildColumn = 'ProductId',
    @ParentTable = 'Products',
    @ConstraintName = 'FK_ProductTags_ProductId';

-- Actualizar FK en ProductImages
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'ProductImages',
    @ChildColumn = 'ProductId',
    @ParentTable = 'Products',
    @ConstraintName = 'FK_ProductImages_ProductId';

-- Actualizar FK en PerishableProducts
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'PerishableProducts',
    @ChildColumn = 'ProductId',
    @ParentTable = 'Products',
    @ConstraintName = 'FK_PerishableProducts_ProductId';

-- Actualizar FK en NonPerishableProducts
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'NonPerishableProducts',
    @ChildColumn = 'ProductId',
    @ParentTable = 'Products',
    @ConstraintName = 'FK_NonPerishableProducts_ProductId';

-- Actualizar FK en DateDisponibility
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'DateDisponibility',
    @ChildColumn = 'ProductId',
    @ParentTable = 'Products',
    @ConstraintName = 'FK_DateDisponibility_ProductId';

-- Actualizar FK en Entrepreneurs
EXEC UpdateForeignKeyWithCascade
    @ChildTable = 'Entrepreneurs',
    @ChildColumn = 'UserId',
    @ParentTable = 'Users',
    @ConstraintName = 'FK_Entrepreneurs_UserId';

-- temp querys
--select * from Products;
-- * from Enterprises;
--select * from ProductImages;

-- delete from Products where Name = 'Producto1'