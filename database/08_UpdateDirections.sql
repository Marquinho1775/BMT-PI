ALTER TABLE Directions
DROP COLUMN Province

ALTER TABLE Directions
DROP COLUMN Canton

ALTER TABLE Directions
DROP COLUMN District

ALTER TABLE Directions
ADD Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();