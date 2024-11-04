CREATE TABLE ShoppingCarts (
    Id     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Total  DECIMAL(18,2)    NOT NULL
);
GO

ALTER TABLE ShoppingCarts
ADD CONSTRAINT FK_ShoppingCarts_UserId
FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE;
GO

CREATE TABLE ShoppingCartProducts (
    ShoppingCartId UNIQUEIDENTIFIER NOT NULL,
    ProductId      UNIQUEIDENTIFIER NOT NULL,
    Quantity       INT              NOT NULL,
    Subtotal       DECIMAL(18,2)    NOT NULL
	CONSTRAINT UQ_ShoppingCartProducts UNIQUE (ShoppingCartId, ProductId)
);
GO

ALTER TABLE ShoppingCartProducts
ADD CONSTRAINT FK_ShoppingCartProducts_ShoppingCartId
FOREIGN KEY (ShoppingCartId) REFERENCES ShoppingCarts(Id) ON DELETE CASCADE;
GO

ALTER TABLE ShoppingCartProducts
ADD CONSTRAINT FK_ShoppingCartProducts_ProductId
FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
GO

