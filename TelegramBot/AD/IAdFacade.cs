using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	public interface IAdFacade
	{
		AdReader Request { get; }
		void Connect();
	}
}
