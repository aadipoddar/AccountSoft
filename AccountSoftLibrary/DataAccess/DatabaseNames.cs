namespace AccountSoftLibrary.DataAccess;

public static class TableNames
{
	public static string Accounting => "Accounting";
	public static string AccountingDetails => "AccountingDetails";
	public static string Company => "Company";
	public static string Group => "Group";
	public static string AccountType => "AccountType";
	public static string Ledger => "Ledger";
	public static string Voucher => "Voucher";
}

public static class StoredProcedureNames
{
	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";

	public static string LoadFinancialYearByDate => "Load_FinancialYear_By_Date";

	public static string LoadLastAccountingByFinancialYearCompanyVoucher => "Load_LastAccounting_By_FinancialYear_Company_Voucher";

	public static string InsertLedger => "Insert_Ledger";

	public static string InsertAccounting => "Insert_Accounting";
	public static string InsertAccountingDetails => "Insert_AccountingDetails";
}

public static class ViewNames
{

}