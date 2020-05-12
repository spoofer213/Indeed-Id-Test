using Purse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Services
{
	public interface ICurrencyService
	{
		Task<List<Currency>> GetCurrenciesAndRates();
		Currency ChangeCurrencyRate(Currency currency);
		void AddCurrencyToList(Currency currency);
	}
}
