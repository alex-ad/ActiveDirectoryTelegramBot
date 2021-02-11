using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	class Subscriber
	{
		private readonly int _telegramUserId;
		private readonly Config.Config _config;
		private readonly AdReader _ad;

		public Subscriber(int userId, IConfig config, AdReader ad)
		{
			_telegramUserId = userId;
			_config = (Config.Config)config;
			_ad = ad;
		}

		/*public string SignIn(string userName)
		{
			if ( IsIdentifiedUser())
			{
				if ( !IsAllowedUser() )
					return "Sorry, You are not allowed to get access this service because of the Bot Config. Contact your system administrator for help.";
				return "You are already SignedIn.";
			}
			if (!_ad.IsIdentifiedUser(userName, _config.AllowedAdGroups)) return "Sorry, You are not allowed to get access this service because of you Active Directory Account permissions. Contact your system administrator for help.";

			var user = new TelegramUser
			{
				ADUserName = userName,
				Allowed = true,
				TelegramId = _telegramUserId,
				Notifications = AdNotifications.None
			};

			try
			{
				_config.TelegramUsers.Add(user);
				_config.SetNewUser(user);
			}
			catch (Exception ex)
			{
				if (IsIdentifiedUser()) _config.TelegramUsers.Remove(user);
				return $"Some error occured on creating new user: {ex.Message}";
			}

			return "Congratulations. You was successfully SignedIn.";
		}*/

		public string SignIn(string userName, string userPassword)
		{
			if ( IsIdentifiedUser() )
			{
				if ( !IsAllowedUser() )
					return "Sorry, You are not allowed to get access this service because of the Bot Config. Contact your system administrator for help.";
				return "You are already SignedIn.";
			}
			if ( !_ad.IsIdentifiedUser(userName, userPassword, _config.AllowedAdGroups) )
				return "Sorry, You are not allowed to get access this service because of you Active Directory Account permissions. Contact your system administrator for help.";

			var user = new TelegramUser
			{
				ADUserName = userName,
				Allowed = true,
				TelegramId = _telegramUserId,
				Notifications = AdNotifications.None
			};

			try
			{
				_config.TelegramUsers.Add(user);
				_config.SetNewUser(user);
			} catch ( Exception ex )
			{
				if ( IsIdentifiedUser() )
					_config.TelegramUsers.Remove(user);
				return $"Some error occured on creating new user: {ex.Message}";
			}

			return "Congratulations. You was successfully SignedIn.";
		}

		public string SignOut()
		{
			if ( !IsIdentifiedUser() )
			{
				return "You are not SignedIn yet. Aren't you?";
			}

			var user = _config.TelegramUsers.Find(x => x.TelegramId == _telegramUserId);

			try
			{
				// TODO Config должен сам добавлять/удалять и модель, и запись в файл
				_config.TelegramUsers.Remove(user);
				_config.RemoveUser(user);
			} catch ( Exception ex )
			{
				if ( !IsIdentifiedUser() )
					_config.TelegramUsers.Add(user);
				return $"Some error occured on removing user: {ex.Message}";
			}

			return "We are sorry, that You left us. Bye.";
		}

		public bool IsIdentifiedUser() => _config.TelegramUsers.Exists(x => x.TelegramId == _telegramUserId);

		public bool IsAllowedUser() => _config.TelegramUsers.Exists(x => x.TelegramId == _telegramUserId && x.Allowed);

		public void SubscribeOnNotifications()
		{

		}
	}
}
