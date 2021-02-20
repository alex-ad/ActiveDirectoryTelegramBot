using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	internal static class AdNotifyType
	{
		public enum ChangingObjectType
		{
			Computer,
			User,
			Group
		}

		public enum ChangingOperationType
		{
			Added,
			Deleted,
			Modified,
		}
	}
}
