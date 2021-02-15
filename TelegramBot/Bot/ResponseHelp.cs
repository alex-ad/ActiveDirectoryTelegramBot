using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	// TODO Разнести строки помощи по своим сервисам
	class ResponseHelp : ResponseBase
	{
		private readonly IConfig _config;

		public ResponseHelp(IConfig config) : base()
		{
			_config = config;
		}

		public override async Task Init()
		{
			Message = "Commands list:\r\n/help [/h] - Help";
			_config.msgHelpList.ForEach(x => Message += x);
		}
	}
}
