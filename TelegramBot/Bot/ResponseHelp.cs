using System;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.HelpMsg;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Ответ на запрос списка поддерживаемых команд
	/// </summary>
	/// <remarks>Наследник класса ResponseBase</remarks>
	internal class ResponseHelp : ResponseBase
	{
		/// <summary>
		///		Вывод списка поддерживаемых команд
		/// </summary>
		/// <returns></returns>
		public override async Task Init()
		{
			Message = string.Empty;
			if (HelpMessage.MsgList?.Count < 1) return;
			HelpMessage.MsgList?.ForEach(x => Message += x + Environment.NewLine);
		}
	}
}