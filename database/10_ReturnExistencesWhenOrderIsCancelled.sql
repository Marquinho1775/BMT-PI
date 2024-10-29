CREATE TRIGGER trg_RestoreStockOnCancel
ON Orders
AFTER UPDATE
AS
BEGIN
    -- Verificar si el estatus de la orden fue cambiado a 5 (Cancelado)
    IF UPDATE(Status)
    BEGIN
        DECLARE @OrderId UNIQUEIDENTIFIER;

        -- Obtener el ID de la orden que fue cancelada
        SELECT @OrderId = OrderId
        FROM inserted
        WHERE Status = 5;

        -- Verificar que el ID de la orden no sea NULL
        IF @OrderId IS NOT NULL
        BEGIN
            -- Para productos no perecederos
            UPDATE NonPerishableProducts
            SET Stock = Stock + op.Amount
            FROM Order_Product AS op
            INNER JOIN NonPerishableProducts AS np ON op.ProductId = np.ProductId
            WHERE op.OrderId = @OrderId;

            -- Para productos perecederos
            UPDATE DateDisponibility
            SET Stock = Stock + op.Amount
            FROM Order_Product AS op
            INNER JOIN PerishableProducts AS pp ON op.ProductId = pp.ProductId
            INNER JOIN DateDisponibility AS dd ON op.ProductId = dd.ProductId AND op.DeliveryDate = dd.Date
            WHERE op.OrderId = @OrderId;
        END
    END
END;
GO
