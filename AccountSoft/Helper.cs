using System.Globalization;
using System.Windows.Controls;

namespace AccountSoft;

public static class Helper
{
	public static string RemoveSpace(this string str) =>
		str.Replace(" ", "");

	public static string FormatIndianCurrency(this decimal rate) =>
		string.Format(new CultureInfo("hi-IN"), "{0:C}", rate);

	public static string FormatIndianCurrency(this decimal? rate)
	{
		rate ??= 0;

		return string.Format(new CultureInfo("hi-IN"), "{0:C}", rate);
	}

	public static string FormatIndianCurrency(this int rate) =>
		string.Format(new CultureInfo("hi-IN"), "{0:C}", rate);

	public static void ValidateIntegerInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		e.Handled = !int.TryParse(e.Text, out _);

	public static void ValidateDecimalInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		bool approvedDecimalPoint = false;

		if (e.Text == ".")
		{
			if (!((TextBox)sender).Text.Contains('.'))
				approvedDecimalPoint = true;
		}

		if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
			e.Handled = true;
	}
}
