SELECT * FROM Users;

SELECT * FROM Directions WHERE Username = 'pelon';

DROP TABLE Directions;

-- Creación de la tabla de direcciones
CREATE TABLE Directions (
    UserName    VARCHAR(255) NOT NULL,
    NumDirection VARCHAR(255) NOT NULL,
    Province    VARCHAR(255) NOT NULL,
    Canton      VARCHAR(255) NOT NULL,
    District    VARCHAR(255) NOT NULL,
    OtherSigns  TEXT,
    Coordinates VARCHAR(255) NOT NULL,
    PRIMARY KEY (UserName, NumDirection),
    FOREIGN KEY (UserName) REFERENCES Users(UserName)
);
GO

SELECT * FROM Directions WHERE Username = 'pelon';
