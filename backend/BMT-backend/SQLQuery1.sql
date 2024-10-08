USE [localBasePI]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 23/9/2024 12:01:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[LastName] [varchar](255) NOT NULL,
	[UserName] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[IsVerified] [bit] NULL,
	[Password] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [Id]
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


SELECT * FROM Entrepreneurs_Enterprises;
SELECT * FROM Entrepreneurs;
SELECT * FROM Users;
SELECT * FROM Enterprises;

select en.Name as EnterpriseName, en.IdentificationNumber, u.Name as UserName, u.LastName, en.Description from Entrepreneurs_Enterprises ee 
join Enterprises en on ee.EnterpriseId = en.Id 
join Users u on (select UserId from Entrepreneurs where Identification = 555) = u.Id 
where ee.EntrepreneurId = (select Id from Entrepreneurs where Identification = 555);


SELECT 
    e.Id AS EntrepreneurId,
    e.Identification,
    u.Id AS UserId,
    u.Name,
    u.LastName,
    u.UserName,
    u.Email,
    u.IsVerified
FROM 
    Entrepreneurs e
JOIN 
    Users u ON e.UserId = u.Id
WHERE 
    u.Id = @UserId;