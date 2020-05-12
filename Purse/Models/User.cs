using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Purse.Models
{
	/// <summary>
	///		Пользователь
	/// </summary>
	public class User
	{
		public User(string userName)
		{
			UserName = userName;
		}
		[Required]
		public string UserName { get; set; }
		[JsonIgnore]
		public Purse Purse { get; set; } = new Purse();
	}
}
