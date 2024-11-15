-- Creación del Stored Procedure para obtener empresas asociadas a un usuario
CREATE PROCEDURE GetEnterprisesByUser
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.IdentificationType, e.IdentificationNumber, e.Name, e.Description, e.Email, e.PhoneNumber
    FROM Entrepreneurs ent
    INNER JOIN  Entrepreneurs_Enterprises ee ON ent.Id = ee.EntrepreneurId
    INNER JOIN Enterprises e ON ee.EnterpriseId = e.Id
    WHERE ent.UserId = @UserId;
END
GO