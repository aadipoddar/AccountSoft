namespace AccountSoftLibrary.Data;

public static class AccountingData
{
	public static async Task<int> InsertAccounting(AccountingModel accounting) =>
		(await SqlDataAccess.LoadData<int, dynamic>(StoredProcedureNames.InsertAccounting, accounting)).FirstOrDefault();

	public static async Task InsertAccountingDetails(AccountingDetailsModel accountingDetails) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertAccountingDetails, accountingDetails);

	public static async Task<AccountingModel> LoadLastAccountingByFinancialYearCompanyVoucher(int FinancialYearId, int CompanyId, int VoucherId) =>
		(await SqlDataAccess.LoadData<AccountingModel, dynamic>(StoredProcedureNames.LoadLastAccountingByFinancialYearCompanyVoucher, new { FinancialYearId, CompanyId, VoucherId })).FirstOrDefault();
}
