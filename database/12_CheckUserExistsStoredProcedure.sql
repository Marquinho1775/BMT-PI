CREATE PROCEDURE CheckUserExists
    @Email VARCHAR(255),
    @UserName VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        SELECT 'Email' AS Result;
    END
    ELSE IF EXISTS (SELECT 1 FROM Users WHERE UserName = @UserName)
    BEGIN
        SELECT 'Username' AS Result;
    END
    ELSE
    BEGIN
        SELECT 'Available' AS Result;
    END
END;
GO
