using System;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
	/// <summary>
	///		Битовые значения для определения, на получение каких именно оповещений подпиан пользователь
	/// </summary>
	// TODO v2 Заготовка для будущего использования
	[Flags]
	public enum AdNotifications
	{
		None = 0,
		UserInfoChanged = 1,
		UserAdded = 2,
		UserBlockedUnblocked = 4,
		UserDeleted = 8,
		UserMoved = 16,
		ComputerInfoChanged = 32,
		ComputerAdded = 64,
		ComputerBlockedUnblocked = 128,
		ComputerDeleted = 256,
		ComputerMoved = 512
	}

	public class TelegramUser
	{
		public int TelegramId { get; set; }
		public string ADUserName { get; set; }
		public bool Allowed { get; set; }
		public AdNotifications Notifications { get; set; }
	}
}