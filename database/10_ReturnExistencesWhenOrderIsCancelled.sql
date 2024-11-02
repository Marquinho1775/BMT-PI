CREATE TRIGGER [dbo].[trg_RestoreStockOnCancel]
ON [dbo].[Orders]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the Status column was updated
    IF UPDATE(Status)
    BEGIN
        -- Get all OrderIds where Status changed to 5 (Cancelled)
        ;WITH CancelledOrders AS (
            SELECT i.OrderId
            FROM inserted i
            INNER JOIN deleted d ON i.OrderId = d.OrderId
            WHERE i.Status = 5 AND d.Status <> 5
        )
        -- Restore stock for non-perishable products
        UPDATE np
        SET np.Stock = np.Stock + op.Amount
        FROM NonPerishableProducts np
        INNER JOIN Order_Product op ON op.ProductId = np.ProductId
        INNER JOIN CancelledOrders co ON op.OrderId = co.OrderId;

        -- Restore stock for perishable products
        UPDATE dd
        SET dd.Stock = dd.Stock + op.Amount
        FROM DateDisponibility dd
        INNER JOIN PerishableProducts pp ON dd.ProductId = pp.ProductId
        INNER JOIN Order_Product op ON op.ProductId = pp.ProductId
        INNER JOIN Orders o ON op.OrderId = o.OrderId
        INNER JOIN CancelledOrders co ON o.OrderId = co.OrderId
        WHERE dd.Date = o.OrderDeliveryDate;
    END
END;
GO
