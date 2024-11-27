BEGIN TRANSACTION;

DECLARE @ConstraintName NVARCHAR(128);

-- Retrieve the current primary key constraint name for Order_Product
SELECT @ConstraintName = kc.name
FROM sys.key_constraints AS kc
JOIN sys.tables AS t ON kc.parent_object_id = t.object_id
WHERE kc.[type] = 'PK' AND t.name = 'Order_Product';

-- Drop the current primary key constraint if it exists
IF @ConstraintName IS NOT NULL
BEGIN
    DECLARE @Sql NVARCHAR(MAX);
    SET @Sql = 'ALTER TABLE Order_Product DROP CONSTRAINT ' + QUOTENAME(@ConstraintName) + ';';
    EXEC sp_executesql @Sql;
END
ELSE
BEGIN
    PRINT 'Primary key constraint not found for table Order_Product.';
END;

-- Add a new column Id with a default uniqueidentifier and set it as the new primary key
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('Order_Product'))
BEGIN
    ALTER TABLE Order_Product
    ADD Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
END;

-- Add the new primary key constraint on Id
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Order_Product_Id')
BEGIN
    ALTER TABLE Order_Product
    ADD CONSTRAINT PK_Order_Product_Id PRIMARY KEY (Id);
END;

COMMIT TRANSACTION;

-- Rollback if an error occurs
IF @@ERROR <> 0 
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error detected. Transaction rolled back.';
END;
