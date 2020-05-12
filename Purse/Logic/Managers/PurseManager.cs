using Purse.Models;
using Purse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Services
{
	public class PurseManager: IPurseManager
    {
        private readonly ICurrencyService _currencyService;        
        private static List<Currency> currencies;

        public PurseManager(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public CurrencyAccount GetCurrencyAccount(User user, string currencyName)
        {
            return user.Purse.CurrencyAccounts.FirstOrDefault(x => x.CurrencyName.Equals(currencyName));            
        }

        public async Task ConvertCurrencies(User user, string currencyFrom, string currencyTo, double value)
        {
            var currencyAccountFrom = user.Purse.CurrencyAccounts.FirstOrDefault(x => x.CurrencyName.Equals(currencyFrom));
            if (currencyAccountFrom == null)
                throw new Exception("Указанный пользователь не имеет счёта с указанной валютой");

            if (currencyAccountFrom.Value < value)
                throw new Exception("Недостаточно средства для списания");

            var currencyAccountTo = user.Purse.CurrencyAccounts.FirstOrDefault(x => x.CurrencyName.Equals(currencyTo));
            if (currencyAccountTo == null)
                currencyAccountTo = await AddAccountCurrencyForUser(user, new CurrencyAccount(currencyTo, 0));

            var currencyRateFrom = currencies.Where(x => x.CurrencyName.Equals(currencyFrom)).Select(x => x.Rate).FirstOrDefault();
            var currencyRateTo = currencies.Where(x => x.CurrencyName.Equals(currencyTo)).Select(x => x.Rate).FirstOrDefault();
            var valueInEuro = value / currencyRateFrom;
            var resultValue = valueInEuro * currencyRateTo;

            ReplanishCurrencyAccount(currencyAccountFrom, -value);
            ReplanishCurrencyAccount(currencyAccountTo, resultValue);
        }

        public void ReplanishCurrencyAccount(CurrencyAccount currencyAccount, double value)
        {
            currencyAccount.Value += value;
        }

        public async Task<CurrencyAccount> AddAccountCurrencyForUser(User user, CurrencyAccount currencyAccount)
        {
            await InitCurrencies();
            var checkCorrectCurrency = currencies.FirstOrDefault(x => x.CurrencyName.Equals(currencyAccount.CurrencyName));
            if (checkCorrectCurrency == null)
                throw new Exception("Неизвестный тип валюты!");

            user.Purse.CurrencyAccounts.Add(currencyAccount);
            return currencyAccount;
        }

        private async Task InitCurrencies()
        {
            if (currencies == null || !currencies.Any())
                currencies = await _currencyService.GetCurrenciesAndRates();
        }
    }
}
