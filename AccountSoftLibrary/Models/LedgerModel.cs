namespace AccountSoftLibrary.Models;

public class LedgerModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Remarks { get; set; }
	public int GroupId { get; set; }
	public int AccountTypeId { get; set; }
	public bool Status { get; set; }
}