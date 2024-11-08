BEGIN TRANSACTION;

-- Creación de tabla de carritos de compra
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ShoppingCarts' AND xtype='U')
BEGIN
    CREATE TABLE ShoppingCarts (
        Id      UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()),
        UserId  UNIQUEIDENTIFIER NOT NULL,
        Total   DECIMAL(18,2)    NOT NULL
    );

    ALTER TABLE ShoppingCarts
    ADD CONSTRAINT FK_ShoppingCarts_UserId
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE;
END;
GO

-- Creación de tabla de productos en carritos de compra
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ShoppingCartProducts' AND xtype='U')
BEGIN
    CREATE TABLE ShoppingCartProducts (
        ShoppingCartId  UNIQUEIDENTIFIER NOT NULL,
        ProductId       UNIQUEIDENTIFIER NOT NULL,
        Quantity        INT              NOT NULL,
        Subtotal        DECIMAL(18,2)    NOT NULL,
        CONSTRAINT UQ_ShoppingCartProducts UNIQUE (ShoppingCartId, ProductId)
    );

    ALTER TABLE ShoppingCartProducts
    ADD CONSTRAINT FK_ShoppingCartProducts_ShoppingCartId
    FOREIGN KEY (ShoppingCartId) REFERENCES ShoppingCarts(Id) ON DELETE CASCADE;

    ALTER TABLE ShoppingCartProducts
    ADD CONSTRAINT FK_ShoppingCartProducts_ProductId
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
END;
GO

COMMIT TRANSACTION;

-- Si ocurre algún error, se hace rollback
IF @@ERROR <> 0 
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error detected. Transaction rolled back.';
END;


