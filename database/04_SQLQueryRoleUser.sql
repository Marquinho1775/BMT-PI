ALTER TABLE Users
ADD Role VARCHAR(3) NOT NULL DEFAULT 'cli';
GO

ALTER TABLE Users
ADD CONSTRAINT chk_role CHECK (Role IN ('cli', 'emp', 'dev'));
GO