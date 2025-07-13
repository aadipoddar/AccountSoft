CREATE TABLE [dbo].[Accounting]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ReferenceNo] VARCHAR(20) NOT NULL, 
    [VoucherId] INT NOT NULL, 
    [AccountingDateTime] DATETIME NOT NULL, 
    [Remarks] VARCHAR(500) NOT NULL, 
    [CompanyId] INT NOT NULL, 
    CONSTRAINT [FK_Accounting_ToVoucher] FOREIGN KEY (VoucherId) REFERENCES [Voucher](Id), 
    CONSTRAINT [FK_Accounting_ToCompany] FOREIGN KEY (CompanyId) REFERENCES [Company](Id)
)
