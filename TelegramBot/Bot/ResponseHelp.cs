using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class ResponseHelp : ResponseBase
	{
		public ResponseHelp() : base() { }

		public override async Task Init()
		{
			Message = string.Empty;
			if (HelpMsg.HelpMsg.MsgList?.Count < 1) return;
			HelpMsg.HelpMsg.MsgList.ForEach(x => Message += x + Environment.NewLine);
		}
	}
}
