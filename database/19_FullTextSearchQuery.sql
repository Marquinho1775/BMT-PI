IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'FTCatalog')
CREATE FULLTEXT CATALOG FTCatalog AS DEFAULT;
GO

create fulltext index on Products(Name, Description)
key index PK__Products__3214EC07905BE42C on FTCatalog;
go

create fulltext index on Enterprises(Name, Description)
key index PK__Enterpri__3214EC0791052781 on FTCatalog;
go


-- SELECT p.Id, Result.RANK as Relevance
-- FROM Products p INNER JOIN CONTAINSTABLE(Products, (Name, Description), 'isabout("Galletas", navidad weight(1), padre weight(.5))') as Result
-- ON p.Id = Result.[Key]
-- ORDER BY Result.RANK DESC;