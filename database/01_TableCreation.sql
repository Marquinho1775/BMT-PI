-- Creación de tabla de usuarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    Id          uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    Name        varchar(255)        NOT NULL,
    LastName    varchar(255)        NOT NULL,
    UserName    varchar(255)        NOT NULL UNIQUE,
    Email       varchar(255)        NOT NULL UNIQUE,
    IsVerified  bit,
    Password    varchar(255)        NOT NULL
);
GO

-- Creación de tabla de emprendimientos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Enterprises' AND xtype='U')
CREATE TABLE Enterprises (
    Id                      uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    IdentificationType      int                 NOT NULL,
    IdentificationNumber    varchar(255)        NOT NULL UNIQUE,
    Name                    varchar(255)        NOT NULL UNIQUE,
    Description             varchar(1000)
);
GO

-- Creación de tabla de emprendedores
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Entrepreneurs' AND xtype='U')
CREATE TABLE Entrepreneurs (
    Id                  uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    UserId              uniqueidentifier    NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(Id),
    Identification      varchar(255)        NOT NULL UNIQUE
);
GO

-- Creación de tabla de relación entre emprendedores y emprendimientos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Entrepreneurs_Enterprises' AND xtype='U')
CREATE TABLE Entrepreneurs_Enterprises (
    EntrepreneurId      uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Entrepreneurs(Id),
    EnterpriseId        uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Enterprises(Id),
    Administrator       bit                 NOT NULL,
    CONSTRAINT PK_Entrepreneurs_Enterprises PRIMARY KEY (EntrepreneurId, EnterpriseId)
);
GO

-- Creación de tablas relacionadas a productos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U')
CREATE TABLE Products (
    Id                  uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    EnterpriseId        uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Enterprises(Id),
    Name                varchar(255)        NOT NULL,
    Description         varchar(1000),
    Price               decimal(18,2)       NOT NULL,
    Weight              decimal(18,2)       NOT NULL
);
GO

-- Creación de tabla de categorías
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tags' AND xtype='U')
CREATE TABLE Tags (
    Id                  uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    Name                varchar(255)        NOT NULL UNIQUE
);
GO

-- Creación de tabla de relación entre productos y categorías
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Product_Tags' AND xtype='U')
CREATE TABLE Product_Tags (
    ProductId           uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Products(Id),
    TagId               uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Tags(Id),
    CONSTRAINT PK_ProductCategories PRIMARY KEY (ProductId, TagId)
);
GO

-- Creación de tabla de imágenes
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Images' AND xtype='U')
CREATE TABLE Images (
    Id                  uniqueidentifier    NOT NULL PRIMARY KEY DEFAULT (newid()),
    OwnerId             uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Products(Id),
    URL                 varchar(255)        NOT NULL UNIQUE
);
GO

-- Creación de tabla de productos no perecederos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NonPerishableProducts' AND xtype='U')
CREATE TABLE NonPerishableProducts (
    ProductId           uniqueidentifier    NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Products(Id),
    Stock               int                 NOT NULL
);
GO

-- Creación de tabla de productos perecederos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerishableProducts' AND xtype='U')
CREATE TABLE PerishableProducts (
    ProductId           uniqueidentifier    NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Products(Id),
    Limit               int                 NOT NULL,
    WeekDaysAvailable   varchar(20)         NOT NULL
);
GO

-- Creación de tabla de disponibilidad de productos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DateDisponibility' AND xtype='U')
CREATE TABLE DateDisponibility (
    ProductId           uniqueidentifier    NOT NULL FOREIGN KEY REFERENCES Products(Id),
    Date                date                NOT NULL,
    Stock               int                 NOT NULL,
    CONSTRAINT PK_DateDisponibility PRIMARY KEY (ProductId, Date)
);
GO

-- Creación de tabla de direcciones
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Directions' AND xtype='U')
CREATE TABLE Directions (
    UserName    varchar(255) NOT NULL,
    NumDirection varchar(255) NOT NULL,
    Province    varchar(255) NOT NULL,
    Canton      varchar(255) NOT NULL,
    District    varchar(255) NOT NULL,
    OtherSigns  text,
    Coordinates varchar(255) NOT NULL,
    PRIMARY KEY (UserName, NumDirection),
    FOREIGN KEY (UserName) REFERENCES Users(UserName)
);
GO

-- Creación de tabla de códigos de confirmación
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Codes' AND xtype='U')
CREATE TABLE Codes (
    Id uniqueidentifier NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Users(Id) ON DELETE CASCADE,
    Code char(6)
);
GO
