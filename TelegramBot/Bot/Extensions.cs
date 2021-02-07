using System;
using System.Collections.Generic;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	static class Extensions
	{
		public static bool EqualsOneOfTheValues(this string searchFor, IEnumerable<string> searchIn)
		{
			foreach (var s in searchIn)
			{
				if (searchFor.Equals(s, StringComparison.OrdinalIgnoreCase)) return true;
			}

			return false;
		}
	}
}
