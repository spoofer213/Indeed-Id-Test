using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Models
{
	/// <summary>
	///		Кошелек
	/// </summary>
	public class Purse
	{
		public List<CurrencyAccount> CurrencyAccounts { get; set; } = new List<CurrencyAccount>();
	}
}
