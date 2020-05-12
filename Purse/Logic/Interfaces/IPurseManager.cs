using Purse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Services.Interfaces
{
	public interface IPurseManager
	{
		Task ConvertCurrencies(User user, string currencyFrom, string currencyTo, double value);
		void ReplanishCurrencyAccount(CurrencyAccount currencyAccount, double value);
		Task<CurrencyAccount> AddAccountCurrencyForUser(User user, CurrencyAccount currencyAccount);
		CurrencyAccount GetCurrencyAccount(User user, string currencyName);
	}
}
