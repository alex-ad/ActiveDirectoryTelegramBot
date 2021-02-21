using System;
using System.Collections.Generic;
using System.Linq;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;
using Telegram.Bot.Types;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	/// Send messages and receive responses
	/// </summary>
	class Messenger
	{
		private readonly IAdReader _ad;
		private readonly User _telegramUser;
		private readonly IConfig _config;
		private readonly IComponent[] _decorators;

		public Messenger(IAdReader ad, User telegramUser, IConfig config, IComponent[] decorators)
		{
			_ad = ad;
			_telegramUser = telegramUser;
			_config = config;
			_decorators = decorators;
		}

		public ResponseBase DoRequest(Message msg)
		{
			if ( string.IsNullOrEmpty(msg.Text) )
				return new ResponseHelp();
			var msgParts = MsgParse(msg.Text);
			if ( msgParts?.Count() < 1 )
				return new ResponseHelp();

			if ( msgParts[0].EqualsOneOfTheValues(Commands.Help))
				return new ResponseHelp();
			
			if (_decorators.OfType<IAdReader>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByLogin))
				return new ResponseUserByLogin(msgParts, _ad);
			
			if (_decorators.OfType<IAdReader>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByName))
				return new ResponseUserByName(msgParts, _ad);
			
			if (_decorators.OfType<IAdSnapshot>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOn))
				return new ResponseSignIn(msgParts, _ad, _config, _telegramUser.Id);
			
			if (_decorators.OfType<IAdSnapshot>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOff))
				return new ResponseSignOut(_ad, _config, _telegramUser.Id);
			
			if (_decorators.OfType<IAdReader>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.ComputerInfo))
				return new ResponseComputerByName(msgParts, _ad);
			
			if (_decorators.OfType<IAdReader>()?.FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.GroupInfo))
				return new ResponseGroupByName(msgParts, _ad);

			return new ResponseHelp();
		}

		private List<string> MsgParse(string message)
		{
			return message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}
