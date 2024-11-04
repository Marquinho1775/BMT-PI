ALTER TABLE Directions
DROP COLUMN Province
GO

ALTER TABLE Directions
DROP COLUMN Canton
GO

ALTER TABLE Directions
DROP COLUMN District
GO

ALTER TABLE Directions
ADD Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
GO

-- Eliminar FK a la primary key actual de Directions en la tabla Orders
ALTER TABLE Orders
DROP CONSTRAINT FK_Direction;
GO

-- Obtener constraint actual de la tabla "Directions"  
SELECT
    kc.name AS ConstraintName
FROM 
    sys.key_constraints AS kc
JOIN 
    sys.tables AS t ON kc.parent_object_id = t.object_id
WHERE 
    t.name = 'Directions' AND kc.type = 'PK';

-- Borrar el constraint actual, sustituir valor de "Drop constraint" con el obtenido por el select anterior.
ALTER TABLE Directions
DROP CONSTRAINT PK_Directions_Id;
GO

-- Hacer que "Id" sea el nuevo primary key.
ALTER TABLE Directions
ADD CONSTRAINT PK_Directions_Id PRIMARY KEY (Id);
GO

-- Columna que guarde el Direction ID
ALTER TABLE Orders
ADD DirectionId UNIQUEIDENTIFIER;
GO

-- Volver a crear el constraint a la nueva PK de Directions en la tabla orders
ALTER TABLE Orders
ADD CONSTRAINT FK_Direction FOREIGN KEY (DirectionId) REFERENCES Directions(Id);
GO