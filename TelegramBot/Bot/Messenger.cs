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
		private List<string> _msgParts;
		private readonly Response _response;
		private readonly AdReader _ad;
		private readonly User _telegramUser;
		private readonly IConfig _config;

		public Messenger(AdReader ad, User telegramUser, IConfig config)
		{
			_response = new Response();
			_ad = ad;
			_telegramUser = telegramUser;
			_config = config;
		}

		public Response DoRequest(Message msg)
		{
			if ( string.IsNullOrEmpty(msg.Text) )
				return _response;
			_msgParts = MsgParse(msg.Text);
			if ( _msgParts.Count < 1 )
				return _response;

			if ( _msgParts[0].EqualsOneOfTheValues(Commands.Help) )
				ResponseHelp();
			else if ( _msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByLogin) )
				ResponseUserInfoByLogin(_msgParts);
			else if ( _msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByName) )
				ResponseUserInfoByName(_msgParts);
			else if ( _msgParts[0].EqualsOneOfTheValues(Commands.SignIn) )
				ResponseSignIn(_msgParts);
			else if (_msgParts[0].EqualsOneOfTheValues(Commands.SignOut))
				ResponseSignOut();

			return _response;
		}

		private List<string> MsgParse(string message)
		{
			return message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		private void ResponseHelp()
		{
			_response.Message = "Commands list:\r\n/help [/h] - Help\r\n/connect [/c] - connect\r\n/userbylogin [/ul][/u] - Get user data by AccountName\r\n/userbyname [/un] - Get user data by FullName (DisplayName)\r\n/group [/g] - Get group data by GroupName\r\n/computer [/c] - Get computer data by ComputerName";
		}

		private void ResponseUserInfoByLogin(List<string> message)
		{
			if ( message.Count() < 2 || string.IsNullOrEmpty(message[1]) )
				return;

			var userPrincipal = _ad.GetUserObjectByLogin(message[1]);
			if ( userPrincipal == null )
				return;

			_response.Message = $"<<< Data for {message[1]} >>>";

			_response.UserData = new UserInfoExt
			{
				Enabled = userPrincipal.Enabled.Value,
				Name = userPrincipal.DisplayName,
				Mail = userPrincipal.EmailAddress,
				TelephoneNumber = userPrincipal.VoiceTelephoneNumber,
				LastLogon = userPrincipal.LastLogon ?? DateTime.MinValue,
				Company = _ad.GetUserProperty(userPrincipal, "Company"),
				Department = _ad.GetUserProperty(userPrincipal, "Department"),
				SamAccountName = userPrincipal.SamAccountName,
				Sid = userPrincipal.Sid.ToString(),
				Title = _ad.GetUserProperty(userPrincipal, "Title"),
				Description = userPrincipal.Description
			};

			_response.GroupData = new List<string>(_ad.GetGroupsByUser(userPrincipal)).OrderBy(x => x);
		}

		private void ResponseUserInfoByName(List<string> message)
		{
			if ( message.Count() < 4 || string.IsNullOrEmpty(message[1]) )
				return;

			var userPrincipal = _ad.GetUserObjectByName($"{message[1]} {message[2]} {message[3]}");
			if ( userPrincipal == null )
				return;

			_response.Message = $"<<< Data for {message[1]} {message[2]} {message[3]} >>>";

			_response.UserData = new UserInfoExt
			{
				Enabled = userPrincipal.Enabled.Value,
				Name = userPrincipal.DisplayName,
				Mail = userPrincipal.EmailAddress,
				TelephoneNumber = userPrincipal.VoiceTelephoneNumber,
				LastLogon = userPrincipal.LastLogon ?? DateTime.MinValue,
				Company = _ad.GetUserProperty(userPrincipal, "Company"),
				Department = _ad.GetUserProperty(userPrincipal, "Department"),
				SamAccountName = userPrincipal.SamAccountName,
				Sid = userPrincipal.Sid.ToString(),
				Title = _ad.GetUserProperty(userPrincipal, "Title"),
				Description = _ad.GetUserProperty(userPrincipal, "Description")
			};

			_response.GroupData = new List<string>(_ad.GetGroupsByUser(userPrincipal)).OrderBy(x => x);
		}

		private void ResponseSignIn(List<string> message)
		{
			if ( message.Count() < 3 || string.IsNullOrEmpty(message[1]) || string.IsNullOrEmpty(message[2]) )
				return;
			if ( message[1].Length < 3 || message[2].Length < 3 )
				return;

			var userName = string.Empty;
			var userPassword = string.Empty;

			if ( message[1].Substring(0, 2).Equals("-u", StringComparison.OrdinalIgnoreCase) )
				userName = message[1].Substring(2);
			else if ( message[1].Substring(0, 2).Equals("-p", StringComparison.OrdinalIgnoreCase) )
				userPassword = message[1].Substring(2);

			if ( message[2].Substring(0, 2).Equals("-u", StringComparison.OrdinalIgnoreCase) )
				userName = message[2].Substring(2);
			else if ( message[2].Substring(0, 2).Equals("-p", StringComparison.OrdinalIgnoreCase) )
				userPassword = message[2].Substring(2);

			if ( string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword) )
				return;

			var user = new Subscriber(_telegramUser.Id, _config, _ad);
			_response.Message = user.SignIn(userName, userPassword);
			_response.EditedMessage = $"{message[1]} -u{userName} -p***";
		}

		private void ResponseSignOut()
		{
			var user = new Subscriber(_telegramUser.Id, _config, _ad);
			_response.Message = user.SignOut();
		}
	}
}
