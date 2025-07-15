using System.Windows;

namespace AccountSoft;

/// <summary>
/// Interaction logic for AccountingWindow.xaml
/// </summary>
public partial class AccountingWindow : Window
{
	private readonly int _selectedCompany;
	private readonly List<AccountingCartModel> _accountingCarts = [];

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

		ledgerComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<LedgerModel>(TableNames.Ledger);
		ledgerComboBox.SelectedValuePath = nameof(LedgerModel.Id);
		ledgerComboBox.DisplayMemberPath = nameof(LedgerModel.Name);

		typeComboBox.ItemsSource = new List<string> { "Debit", "Credit" };
		typeComboBox.SelectedIndex = 0;

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

	private void addLedgerButton_Click(object sender, RoutedEventArgs e)
	{
		string ledgerName = ledgerComboBox.SelectedItem is LedgerModel selectedLedger ? selectedLedger.Name : string.Empty;
		string amountText = amountTextBox.Text;
		string remarks = ledgerRemarksTextBox.Text;

		if (string.IsNullOrEmpty(ledgerName) || string.IsNullOrEmpty(amountText))
		{
			MessageBox.Show("Please select a ledger and enter an amount.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		if (decimal.TryParse(amountText, out decimal amount))
		{
			_accountingCarts.Add(new AccountingCartModel
			{
				Id = ledgerComboBox.SelectedValue is null ? 0 : (int)ledgerComboBox.SelectedValue,
				Serial = _accountingCarts.Count + 1,
				Name = ledgerName,
				Remarks = remarks,
				Debit = typeComboBox.SelectedIndex == 0 ? amount : null,
				Credit = typeComboBox.SelectedIndex == 1 ? amount : null
			});

			ledgersDataGrid.ItemsSource = null;
			ledgersDataGrid.ItemsSource = _accountingCarts;
		}
		else
		{
			MessageBox.Show("Invalid amount entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
