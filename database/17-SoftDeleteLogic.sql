ALTER TABLE Products
ADD SoftDeleted BIT NOT NULL DEFAULT 0;

ALTER TABLE Users
ADD SoftDeleted BIT NOT NULL DEFAULT 0;

ALTER TABLE Enterprises
ADD SoftDeleted BIT NOT NULL DEFAULT 0;

ALTER TABLE Directions
ADD SoftDeleted BIT NOT NULL DEFAULT 0;