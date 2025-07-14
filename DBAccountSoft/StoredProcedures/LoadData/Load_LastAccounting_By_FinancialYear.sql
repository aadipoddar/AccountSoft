CREATE PROCEDURE [dbo].[Load_LastAccounting_By_FinancialYear_Company_Voucher]
	@FinancialYearId INT,
	@CompanyId INT,
	@VoucherId INT
AS
BEGIN
	SELECT TOP 1 *
	FROM Accounting
	WHERE FinancialYearId = @FinancialYearId
	AND CompanyId = @CompanyId
	AND VoucherId = @VoucherId
	ORDER BY AccountingDate DESC
END