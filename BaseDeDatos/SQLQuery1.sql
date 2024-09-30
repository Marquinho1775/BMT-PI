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

Create table Codes (
    Id uniqueidentifier NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    Code char(6),
);