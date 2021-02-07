using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Config
{
	public interface IConfig
	{
		void RemoveUser(TelegramUser user);
		void SetNewUser(TelegramUser user);
		bool TryGetParamsFromFile();
		void Reload();
	}
}
