using Purse.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Purse.Services
{
	public class CurrencyService: ICurrencyService
	{
		private readonly IHttpClientFactory _clientFactory;
		private static List<Currency> currencies = new List<Currency>();

		public CurrencyService(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;			
		}

		public async Task<List<Currency>> GetCurrenciesAndRates()
		{
			if(currencies.Count == 0)
				await InitCurrencies();
			return currencies;
		}

		public Currency ChangeCurrencyRate(Currency currencyForChange)
		{
			var currency = currencies.FirstOrDefault(x => x.CurrencyName == currencyForChange.CurrencyName);
			if(currency != null)
			{
				currencies.Remove(currency);
				currencies.Add(currencyForChange);
				return currencyForChange;
			}

			throw new Exception("Указанная валюта не найдена");
		}

		private async Task InitCurrencies()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
			var client = _clientFactory.CreateClient();
			var response = await client.SendAsync(request);			
			if (response.IsSuccessStatusCode)
			{
				XmlDocument xmlResult = new XmlDocument();
				using (var responseStream = await response.Content.ReadAsStreamAsync())
				{
					var responseInBytes = new byte[responseStream.Length];
					await responseStream.ReadAsync(responseInBytes, 0, (int)responseStream.Length);
					var result = System.Text.Encoding.ASCII.GetString(responseInBytes);					
					xmlResult.LoadXml(result);					
				}

				var nodeList = xmlResult.GetElementsByTagName("Cube");
				var currencyNodes = new List<XmlNode>(nodeList.Cast<XmlNode>());
				foreach (var node in currencyNodes.Where(x => x.Attributes.Count > 1))
				{
					AddCurrencyToList(new Currency(node.Attributes["currency"].Value, double.Parse(node.Attributes["rate"].Value, CultureInfo.InvariantCulture)));
				}
				currencies.Add(new Currency("EUR", 1));
			}
		}

		public void AddCurrencyToList(Currency currency)
		{
			currencies.Add(currency);
		}
	}
}
