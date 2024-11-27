ALTER TABLE Orders
ADD NumOrder INT IDENTITY(1000, 1);

GO
ALTER TRIGGER [dbo].[trg_RestoreStockOnCancel]
ON [dbo].[Orders]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica si la columna Status fue actualizada
    IF UPDATE(Status)
    BEGIN
        -- Crea una variable de tabla para almacenar los OrderId con status cancelado o terminado
        DECLARE @CancelledOrders TABLE (OrderId UNIQUEIDENTIFIER);

        -- Inserta los OrderId donde el Status cambió a 5 (Cancelado) o 6 (Terminado)
        INSERT INTO @CancelledOrders (OrderId)
        SELECT i.OrderId
        FROM inserted i
        INNER JOIN deleted d ON i.OrderId = d.OrderId
        WHERE (i.Status IN (5, 6)) AND (d.Status NOT IN (5, 6));

        -- Restaura el stock para productos no perecederos
        UPDATE np
        SET np.Stock = np.Stock + op.Amount
        FROM NonPerishableProducts np
        INNER JOIN Order_Product op ON op.ProductId = np.ProductId
        INNER JOIN @CancelledOrders co ON op.OrderId = co.OrderId;

        -- Restaura el stock para productos perecederos
        UPDATE dd
        SET dd.Stock = dd.Stock + op.Amount
        FROM DateDisponibility dd
        INNER JOIN PerishableProducts pp ON dd.ProductId = pp.ProductId
        INNER JOIN Order_Product op ON op.ProductId = pp.ProductId
        INNER JOIN Orders o ON op.OrderId = o.OrderId
        INNER JOIN @CancelledOrders co ON o.OrderId = co.OrderId
        WHERE dd.Date = o.OrderDeliveryDate;
    END
END;
