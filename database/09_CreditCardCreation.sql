create table CreditCard(
    Id			uniqueidentifier	NOT NULL PRIMARY KEY DEFAULT (newid()),
	UserID		uniqueidentifier	not null foreign key references Users(Id) ON DELETE CASCADE,
    Name		varchar(255)		NOT NULL,
    Number      varchar(255)        NOT NULL,
	MagicNumber varchar(255)        NOT NULL,
	DateVenc    varchar(255)		NOT NULL,
 );


