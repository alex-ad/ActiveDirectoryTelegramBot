using System;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
	/// <summary>
	///		Битовые значения для определения, на получение каких именно оповещений подпиан пользователь
	/// </summary>

	public class TelegramUser
	{
		public int TelegramId { get; set; }
		public bool Allowed { get; set; }
	}
}