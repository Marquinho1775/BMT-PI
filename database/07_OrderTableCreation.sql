CREATE TABLE Orders (
    OrderId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (newid()),
    OrderCost DECIMAL(18, 2) NOT NULL,
    DeliveryFee DECIMAL(18, 2) NOT NULL,
    Weight DECIMAL(18, 2) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL,
    CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
);
GO

CREATE TABLE Order_Product (
	OrderId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductId UNIQUEIDENTIFIER NOT NULL,
	Amount INT NOT NULL,
	ProductsCost INT NOT NULL,
	DeliveryDate DATE NOT NULL,
	CONSTRAINT FK_OrderId FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
	CONSTRAINT FK_ProductId FOREIGN KEY (ProductId) References Products(Id),
);
GO

ALTER TABLE Orders
ADD OrderDate DATETIME DEFAULT GETDATE() NOT NULL; 
GO

alter table Orders
ADD OrderDeliveryDate DATE NOT NULL; 
GO

alter table Orders
ADD OrderPaymentMethod varchar(255) NOT NULL; 
GO

alter table Order_Product 
drop column DeliveryDate