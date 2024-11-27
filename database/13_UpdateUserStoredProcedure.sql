CREATE PROCEDURE UpdateUser
    @Id uniqueidentifier,
    @Username varchar(255) = NULL,
    @Name varchar(255) = NULL,
    @LastName varchar(255) = NULL,
    @Password varchar(255) = NULL
AS
BEGIN
    -- Inicio de la transacción
    BEGIN TRANSACTION;

    -- Actualización de los campos que no sean NULL
    UPDATE Users
    SET 
        UserName = ISNULL(@Username, UserName),
        Name = ISNULL(@Name, Name),
        LastName = ISNULL(@LastName, LastName),
        Password = ISNULL(@Password, Password)
    WHERE Id = @Id;

    -- Si no hay errores, se confirma la transacción
    IF @@ERROR = 0
    BEGIN
        COMMIT TRANSACTION;
    END
    ELSE
    BEGIN
        -- Si hay errores, se revierte la transacción
        ROLLBACK TRANSACTION;
    END
END;

