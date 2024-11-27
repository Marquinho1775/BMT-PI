CREATE PROCEDURE GetProductDetails
    @ProductId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener detalles del producto, incluyendo tipo y datos específicos
    SELECT 
        p.Id, 
        p.Name, 
        p.Description, 
        p.Weight, 
        p.Price, 
        e.Id AS EnterpriseId, 
        e.Name AS EnterpriseName,
        CASE
            WHEN np.ProductId IS NOT NULL THEN 'NonPerishable'
            WHEN pp.ProductId IS NOT NULL THEN 'Perishable'
            ELSE ''
        END AS ProductType,
        np.Stock,
        pp.[Limit],
        pp.WeekDaysAvailable
    FROM Products p
    JOIN Enterprises e ON p.EnterpriseId = e.Id
    LEFT JOIN NonPerishableProducts np ON p.Id = np.ProductId
    LEFT JOIN PerishableProducts pp ON p.Id = pp.ProductId
    WHERE p.Id = @ProductId;

    -- Obtener etiquetas asociadas al producto
    SELECT 
        t.Name AS TagName
    FROM ProductTags pt
    JOIN Tags t ON pt.TagId = t.Id
    WHERE pt.ProductId = @ProductId;

    -- Obtener URLs de imágenes asociadas al producto
    SELECT 
        i.URL AS ImageUrl
    FROM ProductImages i
    WHERE i.ProductId = @ProductId;
END
