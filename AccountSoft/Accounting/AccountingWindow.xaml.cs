using System.Windows;

namespace AccountSoft.Accounting;

/// <summary>
/// Interaction logic for AccountingWindow.xaml
/// </summary>
public partial class AccountingWindow : Window
{
	private readonly int _selectedCompany;
	private readonly List<AccountingCartModel> _accountingCarts = [];

	#region LoadData

	public AccountingWindow(int selectedCompany)
	{
		InitializeComponent();

		_selectedCompany = selectedCompany;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadInitialData();

	private async Task LoadInitialData()
	{
		accountingDatePicker.SelectedDate = DateTime.Now;

		voucherTypeComboBox.ItemsSource = (await CommonData.LoadTableDataByStatus<VoucherModel>(TableNames.Voucher)).OrderBy(x => x.Name).ToList();
		voucherTypeComboBox.SelectedValuePath = nameof(VoucherModel.Id);
		voucherTypeComboBox.DisplayMemberPath = nameof(VoucherModel.Name);
		voucherTypeComboBox.SelectedIndex = 0;

		referenceNoTextBox.Text = await GenerateReferenceNo.GenerateAccountingReferenceNo(_selectedCompany, (int)voucherTypeComboBox.SelectedValue, DateOnly.FromDateTime(accountingDatePicker.SelectedDate.Value));
	}

	private async void accountingDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (accountingDatePicker.SelectedDate is null || voucherTypeComboBox.SelectedValue is null)
			return;

		referenceNoTextBox.Text = await GenerateReferenceNo.GenerateAccountingReferenceNo(_selectedCompany, (int)voucherTypeComboBox.SelectedValue, DateOnly.FromDateTime(accountingDatePicker.SelectedDate.Value));
	}

	private async void voucherTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (accountingDatePicker.SelectedDate is null || voucherTypeComboBox.SelectedValue is null)
			return;

		referenceNoTextBox.Text = await GenerateReferenceNo.GenerateAccountingReferenceNo(_selectedCompany, (int)voucherTypeComboBox.SelectedValue, DateOnly.FromDateTime(accountingDatePicker.SelectedDate.Value));
	}

	private void amountTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);
	#endregion

	#region Ledgers
	private void addLedgerButton_Click(object sender, RoutedEventArgs e)
	{
		SelectLedgerWindow selectLedgerWindow = new();

		if (selectLedgerWindow.ShowDialog() == true && selectLedgerWindow.IsConfirmed)
			_accountingCarts.Add(new()
			{
				Serial = _accountingCarts.Count + 1,
				Id = selectLedgerWindow.SelectedLedger.Id,
				Name = selectLedgerWindow.SelectedLedger.Name,
				Debit = selectLedgerWindow.Debit,
				Credit = selectLedgerWindow.Credit,
				Remarks = selectLedgerWindow.Remarks
			});

		UpdateTotals();

		addLedgerButton.Focus();

		if (_accountingCarts.Count > 0)
			ledgersDataGrid.ScrollIntoView(_accountingCarts.Last());

		if (selectLedgerWindow.IsConfirmed)
			addLedgerButton_Click(sender, e);
	}

	private void UpdateTotals()
	{
		ledgersDataGrid.ItemsSource = null;
		ledgersDataGrid.ItemsSource = _accountingCarts;

		decimal totalDebits = _accountingCarts.Where(x => x.Debit.HasValue).Sum(x => x.Debit.Value);
		decimal totalCredits = _accountingCarts.Where(x => x.Credit.HasValue).Sum(x => x.Credit.Value);
		decimal balance = totalDebits - totalCredits;

		debitsTotalTextBox.Text = totalDebits.FormatIndianCurrency();
		creditsTotalTextBox.Text = totalCredits.FormatIndianCurrency();
		balancesTextBox.Text = balance.FormatIndianCurrency();
	}

	private void searchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		string searchText = searchTextBox.Text.Trim().ToLower();
		if (string.IsNullOrEmpty(searchText))
		{
			ledgersDataGrid.ItemsSource = _accountingCarts;
			return;
		}

		var filteredCarts = _accountingCarts
			.Where(x => x.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
			.ToList();
		ledgersDataGrid.ItemsSource = filteredCarts;
	}

	private void ledgersDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.OriginalSource is FrameworkElement element && element.DataContext is AccountingCartModel selectedCart)
		{
			SelectLedgerWindow selectLedgerWindow = new(selectedCart);
			if (selectLedgerWindow.ShowDialog() == true && selectLedgerWindow.IsConfirmed)
			{
				_accountingCarts.Remove(selectedCart);

				if (!selectLedgerWindow.IsDeleted)
					_accountingCarts.Add(new()
					{
						Serial = _accountingCarts.Count + 1,
						Id = selectLedgerWindow.SelectedLedger.Id,
						Name = selectLedgerWindow.SelectedLedger.Name,
						Debit = selectLedgerWindow.Debit,
						Credit = selectLedgerWindow.Credit,
						Remarks = selectLedgerWindow.Remarks
					});
			}

			UpdateTotals();

		}

		UpdateTotals();
		addLedgerButton.Focus();
	}
	#endregion

	#region Saving
	private bool ValidateForm()
	{
		UpdateTotals();

		if (_accountingCarts.Count == 0)
		{
			MessageBox.Show("Please add at least one ledger entry.", "No Ledger Entries", MessageBoxButton.OK, MessageBoxImage.Warning);
			addLedgerButton.Focus();
			return false;
		}

		decimal totalDebits = _accountingCarts.Where(x => x.Debit.HasValue).Sum(x => x.Debit.Value);
		decimal totalCredits = _accountingCarts.Where(x => x.Credit.HasValue).Sum(x => x.Credit.Value);
		decimal balance = totalDebits - totalCredits;

		if (balance != 0)
		{
			MessageBox.Show("The Debit and Credit Side Should be Balanced", "Not Balanced", MessageBoxButton.OK, MessageBoxImage.Error);
			addLedgerButton.Focus();
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		saveButton.IsEnabled = false;

		int accountingId = await InsertAccounting();
		if (accountingId <= 0)
		{
			MessageBox.Show("Failed to save accounting entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			saveButton.IsEnabled = true;
			return;
		}

		await InsertAccountingDetail(accountingId);

		MessageBox.Show("Accounting entry saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		Close();
	}

	private async Task<int> InsertAccounting() =>
		await AccountingData.InsertAccounting(new()
		{
			Id = 0,
			AccountingDate = DateOnly.FromDateTime(accountingDatePicker.SelectedDate.Value),
			CompanyId = _selectedCompany,
			VoucherId = (int)voucherTypeComboBox.SelectedValue,
			ReferenceNo = referenceNoTextBox.Text.Trim().RemoveSpace(),
			FinancialYearId = (await FinancialYearData.LoadFinancialYearByDate(DateOnly.FromDateTime(accountingDatePicker.SelectedDate.Value))).Id,
			Remarks = remarksTextBox.Text.Trim(),
			Status = true
		});

	private async Task InsertAccountingDetail(int accountingId)
	{
		foreach (var cart in _accountingCarts)
			await AccountingData.InsertAccountingDetails(new()
			{
				Id = 0,
				AccountingId = accountingId,
				LedgerId = cart.Id,
				Amount = cart.Debit ?? cart.Credit ?? 0,
				Type = cart.Debit.HasValue ? 'D' : 'C',
				Remarks = cart.Remarks?.Trim() ?? string.Empty,
				Status = true
			});
	}
	#endregion
}