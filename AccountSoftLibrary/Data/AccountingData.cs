namespace AccountSoftLibrary.Data;

public static class AccountingData
{
	public static async Task<AccountingModel> LoadLastAccountingByFinancialYearCompanyVoucher(int FinancialYearId, int CompanyId, int VoucherId) =>
		(await SqlDataAccess.LoadData<AccountingModel, dynamic>(StoredProcedureNames.LoadLastAccountingByFinancialYearCompanyVoucher, new { FinancialYearId, CompanyId, VoucherId })).FirstOrDefault();
}
