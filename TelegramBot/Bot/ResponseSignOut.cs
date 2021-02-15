using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class ResponseSignOut : ResponseBase
	{
		private readonly IAdReader _ad;
		private readonly IConfig _config;
		private readonly int _userId;

		public ResponseSignOut(IAdReader ad, IConfig config, int userId) : base()
		{
			_ad = ad;
			_config = config;
			_userId = userId;
		}

		public override async Task Init()
		{
			await base.Init();

			await Task.Run(() =>
			{
				var user = new Subscriber(_userId, _config, _ad);
				Message = user.SignOut();
			});
		}
	}
}
