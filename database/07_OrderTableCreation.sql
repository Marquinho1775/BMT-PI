CREATE TABLE Orders (
    OrderId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (newid()),
    OrderDate DATETIME NOT NULL,
    OrderCost DECIMAL(18, 2) NOT NULL,
    DeliveryFee DECIMAL(18, 2) NOT NULL,
    Weight DECIMAL(18, 2) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    DirectionName VARCHAR(255) NOT NULL,
    Province VARCHAR(255) NOT NULL,
	Canton VARCHAR(255) NOT NULL,
	District VARCHAR(255) NOT NULL,
	OtherSigns VARCHAR(255) NULL,
	Coordinates VARCHAR(255) NOT NULL,
	Status INT NOT NULL,
    Products NVARCHAR(MAX), -- JSON de productos
    CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT FK_UserName FOREIGN KEY (UserName) REFERENCES Users(UserName),
    CONSTRAINT FK_Direction FOREIGN KEY (UserName, DirectionName) REFERENCES Directions(UserName, NumDirection),
);