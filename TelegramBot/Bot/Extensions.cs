using System;
using System.Collections.Generic;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Методы расширения
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		///		Сравнение заданного значения <paramref name="searchFor"/> с эелементами списка <paramref name="searchIn"/>
		/// </summary>
		/// <param name="searchFor">Что ищем</param>
		/// <param name="searchIn">Где ищем</param>
		/// <returns>True если значение найдено, иначе False</returns>
		public static bool EqualsOneOfTheValues(this string searchFor, IEnumerable<string> searchIn)
		{
			foreach (var s in searchIn)
				if (searchFor.Equals(s, StringComparison.OrdinalIgnoreCase))
					return true;

			return false;
		}
	}
}