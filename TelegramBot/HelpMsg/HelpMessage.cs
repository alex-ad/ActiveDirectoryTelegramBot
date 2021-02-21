using System.Collections.Generic;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.HelpMsg
{
	/// <summary>
	///		Список команд, выводимых при запросе помощи через чат
	/// </summary>
	public static class HelpMessage
	{
		public static List<string> MsgList { get; set; } = new List<string>();
	}
}