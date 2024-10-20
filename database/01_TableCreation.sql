create database BMT_database;
GO

use BMT_database;
GO

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

-- Creación de tabla de categorías
create table Tags (
	Id					uniqueidentifier	not null primary key default newid(),
	Name				varchar(255)		not null unique,
);

-- Creación de tabla de relación entre productos y categorías
create table Product_Tags (
	ProductId			uniqueidentifier	not null foreign key references Products(Id),
	TagId				uniqueidentifier	not null foreign key references Tags(Id),
	constraint PK_ProductCategories primary key (ProductId, TagId)
);

-- Creación de tabla de imágenes
create table Images (
	Id					uniqueidentifier	not null primary key default newid(),
	OwnerId				uniqueidentifier	not null foreign key references Products(Id),
	URL					varchar(255)		not null unique,
);

-- Creación de tabla de productos no perecederos
create table NonPerishableProducts (
	ProductId			uniqueidentifier	not null primary key foreign key references Products(Id),
	Stock				int					not null,
);

-- Creación de tabla de productos perecederos
create table PerishableProducts (
	ProductId			uniqueidentifier	not null primary key foreign key references Products(Id),
	Limit				int					not null,
	WeekDaysAvailable	varchar(20)			not null,
);

-- Creación de tabla de disponibilidad de productos
create table DateDisponibility (
	ProductId			uniqueidentifier	not null foreign key references Products(Id),
	Date				date				not null,
	Stock				int					not null,
	constraint PK_DateDisponibility primary key (ProductId, Date)
);
GO

-- Creación de la tabla de direcciones
CREATE TABLE Directions (
    UserName    VARCHAR(255) NOT NULL,
    NumDirection VARCHAR(255) NOT NULL,
    Province    VARCHAR(255) NOT NULL,
    Canton      VARCHAR(255) NOT NULL,
    District    VARCHAR(255) NOT NULL,
    OtherSigns  TEXT,
    Coordinates VARCHAR(255) NOT NULL,
    PRIMARY KEY (UserName, NumDirection),
    FOREIGN KEY (UserName) REFERENCES Users(UserName)
);
GO

-- Creación de tabla de códigos de confirmación
Create table Codes (
    Id uniqueidentifier NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    Code char(6),
);
