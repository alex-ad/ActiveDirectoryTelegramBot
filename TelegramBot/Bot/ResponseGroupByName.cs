using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class ResponseGroupByName : ResponseBase
	{
		private readonly IAdReader _ad;

		public ResponseGroupByName(List<string> message, IAdReader ad) : base()
		{
			MessagesIn = message;
			_ad = ad;
		}

		public override async Task Init()
		{
			await base.Init();

			if ( MessagesIn.Count() < 2 || string.IsNullOrEmpty(MessagesIn[1]) )
				return;

			await Task.Run(() =>
			{
				var groupPrincipal = _ad.GetGroupObjectByName(MessagesIn[1]);
				if ( groupPrincipal == null )
					return;

				var sb = new StringBuilder($"<<< Data for {MessagesIn[1]} >>>" + Environment.NewLine);

				var groupData = new GroupInfo
				{
					Name = groupPrincipal.DistinguishedName,
					Description = groupPrincipal.Description
				};

				var userData = new List<string>(_ad.GetUserNamesByGroupObject(groupPrincipal)).OrderBy(x => x);

				sb.AppendLine(ParseResponseObject(groupData)).AppendLine(ParseResponseList(userData));

				Message = sb.ToString();
			});
		}
	}
}
