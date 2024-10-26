ALTER TABLE Enterprises
ADD Email VARCHAR(255),
    PhoneNumber VARCHAR(255);

UPDATE Enterprises
SET Email = '',
    PhoneNumber = '';

ALTER TABLE Enterprises
ALTER COLUMN Email VARCHAR(255) NOT NULL;

alter table Enterprises
alter column PhoneNumber VARCHAR(255) NOT NULL;

UPDATE Enterprises
SET Email = 'email_' + CONVERT(VARCHAR(255), Id),
    PhoneNumber = 'phone_' + CONVERT(VARCHAR(255), Id);

ALTER TABLE Enterprises
ADD CONSTRAINT UQ_Enterprises_Email UNIQUE (Email),
    CONSTRAINT UQ_Enterprises_PhoneNumber UNIQUE (PhoneNumber);
