using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Logger
{
	public interface ILogger
	{
		void Log(string message, OutputTarget outputTarget);
	}
}
