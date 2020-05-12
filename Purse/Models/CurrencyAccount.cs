using System.ComponentModel.DataAnnotations;

namespace Purse.Models
{
	/// <summary>
	///		Валютный счёт
	/// </summary>
	public class CurrencyAccount
	{
		public CurrencyAccount() { }

		public CurrencyAccount(string currencyName, double value)
		{
			CurrencyName = currencyName;
			Value = value;
		}

		[Required]
		public string CurrencyName { get; set; }
		public double Value { get; set; } = 0;
	}
}