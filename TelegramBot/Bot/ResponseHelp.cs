using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	// TODO Разнести строки помощи по своим сервисам
	class ResponseHelp : ResponseBase
	{
		public ResponseHelp() : base() { }

		public override async Task Init()
		{
			Message = "Commands list:\r\n/help [/h] - Help\r\n/connect [/c] - connect\r\n/userbylogin [/ul][/u] - Get user data by AccountName\r\n/userbyname [/un] - Get user data by FullName (DisplayName)\r\n/group [/g] - Get group data by GroupName\r\n/computer [/c] - Get computer data by ComputerName";
		}
	}
}
