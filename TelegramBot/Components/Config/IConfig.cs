using System.Collections.Generic;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config
{
	internal interface IConfig
	{
		string ServerAddress { get; }
		string UserName { get; }
		string UserPassword { get; }
		List<string> AllowedAdGroups { get; }
		string TelegramBotToken { get; }
		List<TelegramUser> TelegramUsers { get; }

		void RemoveUser(TelegramUser user);
		void SetNewUser(TelegramUser user);
		bool TryGetParamsFromFile();
	}
}
