using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public static class AdNotifyType
	{
		public enum ChangingObjectType
		{
			Computer,
			User
		}

		public enum ChangingOperationType
		{
			Added,
			Deleted,
			Modified,
		}
	}
}
