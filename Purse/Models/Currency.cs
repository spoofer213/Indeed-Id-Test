using System.ComponentModel.DataAnnotations;

namespace Purse.Models
{
	public class Currency
	{
		public Currency(string currencyName, double rate)
		{
			CurrencyName = currencyName;
			Rate = rate;
		}
		[Required]
		public string CurrencyName { get; set; }
		public double Rate { get; set; }
	}
}
