using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Config
{
	internal interface IConfig
	{
		string ServerAddress { get; }
		string UserName { get; }
		string UserPassword { get; }
		List<string> AllowedAdGroups { get; }
		string TelegramBotToken { get; }
		List<TelegramUser> TelegramUsers { get; }
		List<string> msgHelpList { get; }

		void RemoveUser(TelegramUser user);
		void SetNewUser(TelegramUser user);
		bool TryGetParamsFromFile();
	}
}
