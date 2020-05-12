using Purse.Models;
using Purse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Purse.Services
{
	public class UserManager: IUserManager
	{
		private static List<User> _users = new List<User>();

		public User AddUser(string userName)
		{
			var newUser = new User(userName);
			_users.Add(newUser);
			return newUser;
		}

		public User GetUser(string userName)
		{
			var user = _users.FirstOrDefault(x => x.UserName.Equals(userName));
			if (user != null)
				return user;
			return null;
		}
	}
}
