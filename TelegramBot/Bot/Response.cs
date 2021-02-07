using System.Collections.Generic;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class Response
	{
		public string Message { get; set; }
		public string EditedMessage { get; set; }
		public UserInfo UserData { get; set; } 
		public IEnumerable<string> GroupData { get; set; }

		public Response()
		{
			Message = "Invalid message format, use /help command for more information";
			EditedMessage = string.Empty;
		}
	}
}
