using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class ResponseSignIn : ResponseBase
	{
		private readonly IAdReader _ad;
		private readonly IConfig _config;
		private readonly int _userId;

		public ResponseSignIn(List<string> message, IAdReader ad, IConfig config, int userId) : base()
		{
			MessagesIn = message;
			_ad = ad;
			_config = config;
			_userId = userId;
			NeedToClean = true;
		}

		public override async Task Init()
		{
			await base.Init();

			if ( MessagesIn.Count() < 3 || string.IsNullOrEmpty(MessagesIn[1]) || string.IsNullOrEmpty(MessagesIn[2]) )
				return;
			if ( MessagesIn[1].Length < 3 || MessagesIn[2].Length < 3 )
				return;

			var userName = string.Empty;
			var userPassword = string.Empty;

			if ( MessagesIn[1].Substring(0, 2).Equals("-u", StringComparison.OrdinalIgnoreCase) )
				userName = MessagesIn[1].Substring(2);
			else if ( MessagesIn[1].Substring(0, 2).Equals("-p", StringComparison.OrdinalIgnoreCase) )
				userPassword = MessagesIn[1].Substring(2);

			if ( MessagesIn[2].Substring(0, 2).Equals("-u", StringComparison.OrdinalIgnoreCase) )
				userName = MessagesIn[2].Substring(2);
			else if ( MessagesIn[2].Substring(0, 2).Equals("-p", StringComparison.OrdinalIgnoreCase) )
				userPassword = MessagesIn[2].Substring(2);

			if ( string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword) )
				return;

			await Task.Run(() =>
			{
				var user = new Subscriber(_userId, _config, _ad);
				Message = user.SignIn(userName, userPassword);
			});
		}
	}
}
