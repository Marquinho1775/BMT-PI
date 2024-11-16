CREATE PROCEDURE CheckEnterpriseExist
    @IdentificationNumber VARCHAR(255),
    @Name VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Enterprises WHERE IdentificationNumber = @IdentificationNumber)
    BEGIN
        SELECT 'IdentificationNumber' AS Result;
    END
    ELSE IF EXISTS (SELECT 1 FROM Enterprises WHERE Name = @Name)
    BEGIN
        SELECT 'Name' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Available' AS Result;
    END
END;
GO

