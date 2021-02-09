using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public interface IAdSnapshotFacade
	{
		void RunAsync(int loopPeriodInMilliseconds);
	}
}
