BEGIN TRANSACTION;

-- Creación de tabla Orders
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
BEGIN
    CREATE TABLE Orders (
        OrderId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()),
        OrderCost DECIMAL(18, 2) NOT NULL,
        DeliveryFee DECIMAL(18, 2) NOT NULL,
        Weight DECIMAL(18, 2) NOT NULL,
        UserId UNIQUEIDENTIFIER NOT NULL,
        Status INT NOT NULL,
        OrderDate DATETIME DEFAULT GETDATE() NOT NULL,
        OrderDeliveryDate DATE NOT NULL,
        OrderPaymentMethod VARCHAR(255) NOT NULL,
        CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END;
GO

-- Creación de tabla Order_Product
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Order_Product' AND xtype='U')
BEGIN
    CREATE TABLE Order_Product (
        OrderId UNIQUEIDENTIFIER NOT NULL,
        ProductId UNIQUEIDENTIFIER NOT NULL,
        Amount INT NOT NULL,
        ProductsCost DECIMAL(18, 2) NOT NULL,
        CONSTRAINT FK_OrderId FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
        CONSTRAINT FK_ProductId FOREIGN KEY (ProductId) REFERENCES Products(Id),
        PRIMARY KEY (OrderId, ProductId)
    );
END;
GO

COMMIT TRANSACTION;

-- Si ocurre algún error, realizar rollback
IF @@ERROR <> 0 
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error detected. Transaction rolled back.';
END;
