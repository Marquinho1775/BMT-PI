use BMT_database;

-- Creación de tabla de usuarios
create table Users(
    Id			uniqueidentifier	NOT NULL PRIMARY KEY DEFAULT (newid()),
    Name		varchar(255)		NOT NULL,
    LastName	varchar(255)		NOT NULL,
    UserName	varchar(255)		NOT NULL UNIQUE,
    Email		varchar(255)		NOT NULL UNIQUE,
    IsVerified	bit,
    Password	varchar(255)		NOT NULL,
);
GO

-- Creación de tabla de emprendimientos
create table Enterprises (
	Id						uniqueidentifier	not null primary key default newid(),
	IdentificationType		int					not null,
	IdentificationNumber	varchar(255)		not null unique,
	Name					varchar(255)		not null unique,
	Description				varchar(1000),
);
GO

-- Creación de tabla de emprendedores
create table Entrepreneurs (
	Id					uniqueidentifier	not null primary key default newid(),
	UserId				uniqueidentifier	not null unique foreign key references Users(Id),
	Identification		varchar(255)		not null unique,
);
GO

-- Creación de tabla de relación entre emprendedores y emprendimientos
create table Entrepreneurs_Enterprises (
	EntrepreneurId		uniqueidentifier	not null foreign key references Entrepreneurs(Id),
	EnterpriseId		uniqueidentifier	not null foreign key references Enterprises(Id),
	Administrator		bit					not null
	constraint PK_Entrepreneurs_Enterprises primary key (EntrepreneurId, EnterpriseId)
);
GO

-- Creacion de tablas relacionadas a productos
create table Products (
	Id					uniqueidentifier	not null primary key default newid(),
	EnterpriseId		uniqueidentifier	not null foreign key references Enterprises(Id),
	Name				varchar(255)		not null,
	Description			varchar(1000),
	Price				decimal(18,2)		not null,
	Weight				decimal(18,2)		not null,
);

create table Images (
	Id					uniqueidentifier	not null primary key default newid(),
	OwnerId				uniqueidentifier	not null foreign key references Products(Id),
	URL					varchar(255)		not null unique,
);

create table Tags (
	Id					uniqueidentifier	not null primary key default newid(),
	Name				varchar(255)		not null unique,
);

create table Product_Tags (
	ProductId			uniqueidentifier	not null foreign key references Products(Id),
	TagId				uniqueidentifier	not null foreign key references Tags(Id),
	constraint PK_ProductCategories primary key (ProductId, TagId)
);

create table NonPerishableProducts (
	ProductId			uniqueidentifier	not null primary key foreign key references Products(Id),
	Stock				int					not null,
);

create table PerishableProducts (
	ProductId			uniqueidentifier	not null primary key foreign key references Products(Id),
	Limit				int					not null,
	WeekDaysAvailable	varchar(20)			not null,
);

create table DateDisponibility (
	ProductId			uniqueidentifier	not null foreign key references Products(Id),
	BatchNumber			int					not null identity(1, 1),
	Date				date				not null,
	Stock				int					not null,
	constraint PK_DateDisponibility primary key (ProductId, BatchNumber)
);
GO

SELECT e.Id
FROM Enterprises e
JOIN Entrepreneurs_Enterprises ee ON e.Id = ee.EnterpriseId
JOIN Entrepreneurs en ON ee.EntrepreneurId = en.Id
JOIN Users u ON en.UserId = u.Id
WHERE u.UserName = @UserName;

-- select all tables 
select * from Users;
select * from Entrepreneurs;
select * from Enterprises;
select * from Entrepreneurs_Enterprises;
select * from Products;
select * from Images;
select * from Tags;
select * from Product_Tags;
select * from NonPerishableProducts;
select * from PerishableProducts;
select * from DateDisponibility;

-- drop all tables
drop table DateDisponibility;
drop table PerishableProducts;
drop table NonPerishableProducts;
drop table Product_Tags;
drop table Tags;
drop table Images;
drop table Products;
drop table Entrepreneurs_Enterprises;
drop table Entrepreneurs;
drop table Enterprises;
drop table Users;