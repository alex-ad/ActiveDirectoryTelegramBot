using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class ResponseComputerByName : ResponseBase
	{
		private readonly IAdReader _ad;

		public ResponseComputerByName(List<string> message, IAdReader ad) : base()
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
				var computerPrincipal = _ad.GetComputerObjectByName(MessagesIn[1]);
				if ( computerPrincipal == null )
					return;

				var sb = new StringBuilder();

				var compData = new ComputerInfo
				{
					Enabled = computerPrincipal.Enabled.Value,
					Name = computerPrincipal.DistinguishedName,
					Description = computerPrincipal.Description,
					LastLogon = computerPrincipal.LastLogon ?? DateTime.MinValue
				};

				sb.AppendLine(ParseResponseObject(compData));
				Message = sb.ToString();
			});
		}
	}
}
