CREATE TABLE [dbo].[Ledger]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(250) NOT NULL UNIQUE, 
    [Remarks] VARCHAR(250) NOT NULL, 
    [GroupId] INT NOT NULL, 
    [AccountTypeId] INT NOT NULL, 
    [Status] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Ledger_ToGroup] FOREIGN KEY (GroupId) REFERENCES [Group](Id), 
    CONSTRAINT [FK_Ledger_ToAccountType] FOREIGN KEY (AccountTypeId) REFERENCES [AccountType](Id)
)