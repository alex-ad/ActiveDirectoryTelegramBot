using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
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
