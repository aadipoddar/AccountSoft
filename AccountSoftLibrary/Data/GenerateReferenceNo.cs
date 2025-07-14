namespace AccountSoftLibrary.Data;

public static class GenerateReferenceNo
{
	private static string GenerateInitialsByName(string name)
	{
		if (name is null)
			return string.Empty;

		string prefix = string.Empty;
		string[] words = name.Split([' ', '-', '_', '.'], StringSplitOptions.RemoveEmptyEntries);

		foreach (var word in words)
			if (word.Length > 1)
			{
				var firstLetter = word[0];

				if (char.IsLetter(firstLetter) && char.IsUpper(firstLetter))
					prefix += char.ToUpper(firstLetter);
			}

		return prefix;
	}

	private static int GetNumberOfDigits(this int number) =>
		number switch
		{
			< 10 => 1,
			< 100 => 2,
			< 1000 => 3,
			< 10000 => 4,
			< 100000 => 5,
			_ => 6
		};

	public static async Task<string> GenerateAccountingReferenceNo(int companyId, int voucherId, DateOnly accountingDate)
	{
		var company = await CommonData.LoadTableDataById<CompanyModel>(TableNames.Company, companyId);
		var voucher = await CommonData.LoadTableDataById<VoucherModel>(TableNames.Voucher, voucherId);
		var financialYear = await FinancialYearData.LoadFinancialYearByDate(accountingDate);
		var lastAccounting = await AccountingData.LoadLastAccountingByFinancialYearCompanyVoucher(financialYear.Id, companyId, voucherId);

		var companyPrefix = GenerateInitialsByName(company.Name);
		var voucherPrefix = GenerateInitialsByName(voucher.Name);
		var year = financialYear.YearNo;

		if (lastAccounting is not null)
		{
			var lastReferenceNo = lastAccounting.ReferenceNo;
			if (lastReferenceNo.StartsWith($"{companyPrefix}{year}{voucherPrefix}"))
			{
				var lastNumber = int.Parse(lastReferenceNo[(companyPrefix.Length + year.GetNumberOfDigits() + voucherPrefix.Length)..]);
				int nextNumber = lastNumber + 1;
				return $"{companyPrefix}{year}{voucherPrefix}{nextNumber:D6}";
			}
		}

		return $"{companyPrefix}{year}{voucherPrefix}000001";
	}
}
