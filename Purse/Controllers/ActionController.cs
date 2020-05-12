using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Purse.Models;
using Purse.Services;
using Purse.Services.Interfaces;

namespace Purse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : Controller
    {        
        private readonly ICurrencyService _currencyService;
        private readonly IPurseManager _purseManager;        
        private readonly IUserManager _userManager;        

        public ActionController(ICurrencyService currencyService, IPurseManager purseManager, IUserManager userManager)
        {
            _currencyService = currencyService;
            _purseManager = purseManager;
            _userManager = userManager;
        }

        /// <summary>
        ///     Создание пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        [HttpPost]
        [Route("CreateUser")]
        public IActionResult CreateUser(string userName)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _userManager.GetUser(userName);
                if (checkUser != null)
                    return BadRequest("Пользователь с таким именем уже существует");

                var user = _userManager.AddUser(userName);
                return Ok(user);
            }
            return BadRequest();
        }

        /// <summary>
        ///     Получение списка валют
        /// </summary>
        [HttpGet]
        [Route("GetCurrencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var result = await _currencyService.GetCurrenciesAndRates();
            return Ok(result);
        }

        /// <summary>
        ///     Создание валютного счёта
        /// </summary>
        /// <param name="currencyAccount">Валютный счёт</param>
        /// <param name="userName">Имя пользователя, для которого создается счёт</param>
        [HttpPost]
        [Route("CreateCurrencyAccount")]
        public async Task<IActionResult> CreateCurrencyAccount(CurrencyAccount currencyAccount, string userName)
        {           
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUser(userName);
                if (user == null)
                    return BadRequest("Указанного пользователя не существует");

                try
                {
                    await _purseManager.AddAccountCurrencyForUser(user, currencyAccount);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }            
                
                return Ok(user.Purse);
            }
            return BadRequest();            
        }

        /// <summary>
        ///     Пополнение счёта
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="currency">Валюта</param>
        /// <param name="value">Значение пополнения</param>
        [HttpGet]
        [Route("Replenish")]
        public async Task<IActionResult> Replenish(string userName, string currency, double value)
        {
            var user = _userManager.GetUser(userName);
            if (user == null)
                return BadRequest("Пользователь с таким именем не найден");

            var currencyAccount = _purseManager.GetCurrencyAccount(user, currency);
            if (currencyAccount == null)
                try
                {
                    await _purseManager.AddAccountCurrencyForUser(user, new CurrencyAccount(currency, value));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            else
                _purseManager.ReplanishCurrencyAccount(currencyAccount, value);

            return Ok(user.Purse);
        }

        /// <summary>
        ///     Списание со счёта
        /// </summary>
        /// <returns></returns>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="currency">Валюта</param>
        /// <param name="value">Значение пополнения</param>
        [HttpGet]
        [Route("Debit")]
        public IActionResult Debit(string userName, string currency, double value)
        {
            var user = _userManager.GetUser(userName);
            if (user == null)
                return BadRequest("Пользователь с таким именем не найден");

            var currencyAccount = _purseManager.GetCurrencyAccount(user, currency);
            if(currencyAccount == null)
                return BadRequest("Указанный пользователь не имеет счёта с указанной валютой");

            _purseManager.ReplanishCurrencyAccount(currencyAccount, -value);
            return Ok(user.Purse);
        }

        /// <summary>
        ///     Перевод средств с одного кошелька на другой
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="currencyFrom">Счёт с которого списываем</param>
        /// <param name="currencyTo">Счёт на который зачисляем</param>
        /// <param name="value">Значение перевода</param>
        [HttpGet]
        [Route("Convert")]
        public async Task<IActionResult> Convert(string userName, string currencyFrom, string currencyTo, double value)
        {
            var user = _userManager.GetUser(userName);
            if (user == null)
                return BadRequest("Пользователь с таким именем не найден");

            try
            {
                await _purseManager.ConvertCurrencies(user, currencyFrom, currencyTo, value);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }    

            return Ok(user.Purse);
        }

        /// <summary>
        ///     Получение состояния кошелька указанного пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        [HttpGet]
        [Route("GetPurseState")]
        public IActionResult GetPurseState(string userName)
        {
            var user = _userManager.GetUser(userName);
            if (user == null)
                return BadRequest("Пользователь с таким именем не найден");

            return Ok(user.Purse);
        }

        /// <summary>
        ///     Изменить курс указанной валюты
        /// </summary>
        /// <param name="currency">Название валюты</param>
        [HttpPost]
        [Route("ChangeCurrencyRate")]
        public IActionResult ChangeCurrencyRate(Currency currency)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var changedCurrency = _currencyService.ChangeCurrencyRate(currency);
                    return Ok(changedCurrency);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }                
            }
            return BadRequest();
        }
    }
}