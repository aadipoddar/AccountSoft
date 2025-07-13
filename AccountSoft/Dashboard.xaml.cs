using System.Windows;

namespace AccountSoft;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	public Dashboard() =>
		InitializeComponent();

	private void voucherButton_Click(object sender, RoutedEventArgs e)
	{
		AccountingWindow accountingWindow = new();
		accountingWindow.Show();
	}

	private void masterButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
