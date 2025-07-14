using System.Windows;

namespace AccountSoft;

/// <summary>
/// Interaction logic for AccountingWindow.xaml
/// </summary>
public partial class AccountingWindow : Window
{
	private readonly int _selectedCompany;

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
}
