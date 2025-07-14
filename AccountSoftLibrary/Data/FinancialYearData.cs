namespace AccountSoftLibrary.Data;

public static class FinancialYearData
{
	public static async Task<FinancialYearModel> LoadFinancialYearByDate(DateOnly Date) =>
		(await SqlDataAccess.LoadData<FinancialYearModel, dynamic>(StoredProcedureNames.LoadFinancialYearByDate, new { Date })).FirstOrDefault();
}
