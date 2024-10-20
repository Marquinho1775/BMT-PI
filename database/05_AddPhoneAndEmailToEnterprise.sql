use BMT_database
go

ALTER TABLE Enterprises ADD Email VARCHAR(255) NOT NULL unique;

alter table Enterprises add PhoneNumber varchar(255) not null unique;