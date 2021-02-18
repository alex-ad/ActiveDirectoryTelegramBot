using System;
using System.Collections.Generic;
using System.Linq;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
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

		public Messenger(IAdReader ad, User telegramUser, IConfig config)
		{
			_ad = ad;
			_telegramUser = telegramUser;
			_config = config;
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
			if ( msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByLogin) )
				return new ResponseUserByLogin(msgParts, _ad);
			if ( msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByName) )
				return new ResponseUserByName(msgParts, _ad);
			if ( msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOn) )
				return new ResponseSignIn(msgParts, _ad, _config, _telegramUser.Id);
			if (msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOff))
				return new ResponseSignOut(_ad, _config, _telegramUser.Id);
			if ( msgParts[0].EqualsOneOfTheValues(Commands.ComputerInfo) )
				return new ResponseComputerByName(msgParts, _ad);
			if ( msgParts[0].EqualsOneOfTheValues(Commands.GroupInfo) )
				return new ResponseGroupByName(msgParts, _ad);

			return new ResponseHelp();
		}

		private List<string> MsgParse(string message)
		{
			return message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}
