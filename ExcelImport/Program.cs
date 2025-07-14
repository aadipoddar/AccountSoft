using AccountSoftLibrary.Data;

using OfficeOpenXml;

FileInfo fileInfo = new(@"C:\Others\suppliers.xlsx");

ExcelPackage.License.SetNonCommercialPersonal("AadiSoft");

using var package = new ExcelPackage(fileInfo);

await package.LoadAsync(fileInfo);

var worksheet = package.Workbook.Worksheets[0];

await InsertSupplier(worksheet);

Console.WriteLine("Finished importing Items.");
Console.ReadLine();

static async Task InsertSupplier(ExcelWorksheet worksheet)
{
	int row = 1;

	while (worksheet.Cells[row, 1].Value is not null)
	{
		var groupId = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty;
		var name = worksheet.Cells[row, 2].Value.ToString();
		var accountType = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty;

		if (string.IsNullOrWhiteSpace(name))
		{
			Console.WriteLine("Not Inserted Row = " + row);
			continue;
		}

		Console.WriteLine("Inserting New Product: " + name);

		try
		{
			await LedgerData.InsertLedger(new()
			{
				Id = 0,
				Name = name,
				GroupId = int.Parse(groupId),
				AccountTypeId = int.Parse(accountType),
				Remarks = "",
				Status = true
			});
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error inserting row {row} + {name}: {ex.Message}");
			Console.ResetColor();
		}

		row++;
	}
}