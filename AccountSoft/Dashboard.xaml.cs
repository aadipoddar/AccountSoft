using System.Windows;

namespace AccountSoft;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	public Dashboard() =>
		InitializeComponent();

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadInitialData();

	private async Task LoadInitialData()
	{
		Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

		companyComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<CompanyModel>(TableNames.Company);
		companyComboBox.SelectedValuePath = nameof(CompanyModel.Id);
		companyComboBox.DisplayMemberPath = nameof(CompanyModel.Name);
		companyComboBox.SelectedIndex = 0;
	}

	private void voucherButton_Click(object sender, RoutedEventArgs e)
	{
		AccountingWindow accountingWindow = new((int)companyComboBox.SelectedValue);
		accountingWindow.Show();
	}

	private void masterButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
