ALTER TABLE Enterprises
ADD Email VARCHAR(255),
    PhoneNumber VARCHAR(255);
GO

UPDATE Enterprises
SET Email = '',
    PhoneNumber = '';
GO

ALTER TABLE Enterprises
ALTER COLUMN Email VARCHAR(255) NOT NULL;
GO

alter table Enterprises
alter column PhoneNumber VARCHAR(255) NOT NULL;
GO

UPDATE Enterprises
SET Email = 'email_' + CONVERT(VARCHAR(255), Id),
    PhoneNumber = 'phone_' + CONVERT(VARCHAR(255), Id);
GO

ALTER TABLE Enterprises
ADD CONSTRAINT UQ_Enterprises_Email UNIQUE (Email),
    CONSTRAINT UQ_Enterprises_PhoneNumber UNIQUE (PhoneNumber);
GO
