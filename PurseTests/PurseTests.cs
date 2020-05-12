using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using Purse.Models;
using Purse.Services;
using Purse.Services.Interfaces;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PurseTests
{
	public class Tests
	{
		private static IHttpClientFactory httpFactoryMock = Substitute.For<IHttpClientFactory>();
		private static ICurrencyService currencyService = new CurrencyService(httpFactoryMock);
		private static IPurseManager purserManager = new PurseManager(currencyService);
		
		[Test]
		public void Replenish_Test([Values(5000, 1000, -200, -5000)] double input)
		{
			var currencyAccount = new CurrencyAccount("RUB", 10000);
			var expectedResult = currencyAccount.Value + input;
			purserManager.ReplanishCurrencyAccount(currencyAccount, input);
			Assert.AreEqual(expectedResult, currencyAccount.Value);
		}

		[Test]
		public async Task ConvertCurrencies_Test([Values(1.5)] double rateForUSD, 
			[Values(20)] double rateForRUB)
		{
			var user = new User("Test");
			currencyService.AddCurrencyToList(new Currency("EUR", 1));
			currencyService.AddCurrencyToList(new Currency("USD", rateForUSD));
			currencyService.AddCurrencyToList(new Currency("RUB", rateForRUB));

			var currencyAccountRUB = new CurrencyAccount("RUB", 10000);

			await purserManager.AddAccountCurrencyForUser(user, currencyAccountRUB);
			
			await purserManager.ConvertCurrencies(user, "RUB", "USD", 2000);

			var expectedResult = (2000 / rateForRUB) * rateForUSD;
			var realResult = user.Purse.CurrencyAccounts.FirstOrDefault(x => x.CurrencyName.Equals("USD"));			
			Assert.AreEqual(expectedResult, realResult.Value);
		}
	}
}