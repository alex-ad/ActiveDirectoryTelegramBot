using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	internal interface IAdSnapshot
	{
		void RunAsync(int loopPeriodInMilliseconds);
	}
}
