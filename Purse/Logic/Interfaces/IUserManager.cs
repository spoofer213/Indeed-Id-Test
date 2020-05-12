using Purse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Services.Interfaces
{
	public interface IUserManager
	{
		User GetUser(string userName);
		User AddUser(string userName);
	}
}
