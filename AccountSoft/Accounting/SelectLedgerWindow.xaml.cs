using System.Windows;

namespace AccountSoft.Accounting;

/// <summary>
/// Interaction logic for SelectLedgerWindow.xaml
/// </summary>
public partial class SelectLedgerWindow : Window
{
	private readonly AccountingCartModel _selectedCart = null;
	public LedgerModel SelectedLedger { get; private set; }
	public decimal? Debit { get; private set; }
	public decimal? Credit { get; private set; }
	public string Remarks { get; private set; }
	public bool IsConfirmed { get; private set; }
	public bool IsDeleted { get; private set; }

	public SelectLedgerWindow(AccountingCartModel selectedCart)
	{
		InitializeComponent();
		searchTextBox.Focus();

		_selectedCart = selectedCart;
	}

	public SelectLedgerWindow()
	{
		InitializeComponent();
		searchTextBox.Focus();

		deleteButton.Visibility = Visibility.Collapsed;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadLedgers();

	private async void searchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
		await LoadLedgers();

	private async Task LoadLedgers()
	{
		var searchText = searchTextBox.Text.ToLower();
		ledgerListView.ItemsSource = (await CommonData.LoadTableDataByStatus<LedgerModel>(TableNames.Ledger))
			.Where(x => x.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
			.OrderBy(x => x.Name)
			.ToList();
		ledgerListView.DisplayMemberPath = nameof(LedgerModel.Name);
		ledgerListView.SelectedValuePath = nameof(LedgerModel.Id);

		if (_selectedCart is null)
			return;

		ledgerListView.SelectedValue = _selectedCart.Id;
		nameTextBox.Text = _selectedCart.Name;
		debitTextBox.Text = _selectedCart.Debit?.ToString() ?? string.Empty;
		creditTextBox.Text = _selectedCart.Credit?.ToString() ?? string.Empty;
		remarksTextBox.Text = _selectedCart.Remarks.Trim();
		deleteButton.Visibility = Visibility.Visible;

		if (ledgerListView.SelectedItem is not LedgerModel ledger)
			return;

		var group = await CommonData.LoadTableDataById<GroupModel>(TableNames.Group, ledger.GroupId);
		var type = await CommonData.LoadTableDataById<AccountTypeModel>(TableNames.AccountType, ledger.AccountTypeId);
		nameTextBox.Text = ledger.Name;
		typeTextBox.Text = type.Name;
		groupTextBox.Text = group.Name.Trim();
	}

	private async void ledgerListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (ledgerListView.SelectedItem is not LedgerModel ledger)
			return;

		var group = await CommonData.LoadTableDataById<GroupModel>(TableNames.Group, ledger.GroupId);
		var type = await CommonData.LoadTableDataById<AccountTypeModel>(TableNames.AccountType, ledger.AccountTypeId);

		nameTextBox.Text = ledger.Name;
		typeTextBox.Text = type.Name;
		groupTextBox.Text = group.Name.Trim();
	}

	private void numericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private bool ValidateForm()
	{
		if (ledgerListView.SelectedItem is not LedgerModel)
		{
			MessageBox.Show("Please Select a Ledger", "Ledger Not Found");
			searchTextBox.Focus();
			return false;
		}

		if (string.IsNullOrEmpty(debitTextBox.Text) && string.IsNullOrEmpty(creditTextBox.Text))
		{
			MessageBox.Show("Please Enter Debit or Credit Amount", "Amount Not Found");
			debitTextBox.Focus();
			return false;
		}

		if (decimal.TryParse(debitTextBox.Text, out var debit) && debit <= 0)
		{
			MessageBox.Show("Debit Amount cannot be negative", "Invalid Debit Amount");
			debitTextBox.Focus();
			return false;
		}

		if (decimal.TryParse(creditTextBox.Text, out var credit) && credit <= 0)
		{
			MessageBox.Show("Credit Amount cannot be negative", "Invalid Credit Amount");
			creditTextBox.Focus();
			return false;
		}

		if (debitTextBox.Text == "0" && creditTextBox.Text == "0")
		{
			MessageBox.Show("Debit and Credit Amount cannot be zero", "Invalid Amount");
			debitTextBox.Focus();
			return false;
		}

		if (!string.IsNullOrEmpty(debitTextBox.Text) && !string.IsNullOrEmpty(creditTextBox.Text))
		{
			MessageBox.Show("Please enter either Debit or Credit Amount, not both", "Invalid Amount");
			debitTextBox.Focus();
			return false;
		}

		SelectedLedger = ledgerListView.SelectedItem as LedgerModel;
		Debit = string.IsNullOrEmpty(debitTextBox.Text) ? null : decimal.Parse(debitTextBox.Text);
		Credit = string.IsNullOrEmpty(creditTextBox.Text) ? null : decimal.Parse(creditTextBox.Text);
		Remarks = remarksTextBox.Text.Trim();

		return true;
	}

	private void addLedgerButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		IsConfirmed = true;
		DialogResult = true;
		Close();
	}

	private void cancelButton_Click(object sender, RoutedEventArgs e)
	{
		IsConfirmed = false;
		DialogResult = true;
		Close();
	}

	private void deleteButton_Click(object sender, RoutedEventArgs e)
	{
		if (_selectedCart is null)
		{
			MessageBox.Show("No Ledger Selected to Delete", "Delete Ledger");
			return;
		}

		if (MessageBox.Show("Are you sure you want to delete this ledger entry?", "Delete Ledger Entry", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
		{
			IsDeleted = true;
			IsConfirmed = true;
			DialogResult = true;
			Close();
		}
	}
}
