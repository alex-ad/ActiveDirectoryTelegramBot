using System;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Включение и отключение оповещений об изменениях в Active Directory
	/// </summary>
	internal class Subscriber
	{
		private readonly IAdReader _ad;
		private readonly IConfig _config;
		private readonly int _telegramUserId;

		/// <summary>
		///		Включение и отключение оповещений об изменениях в Active Directory
		/// </summary>
		/// <param name="userId">Id пользователя, отправившего завпрос в чате</param>
		/// <param name="config">Настройки приложения из файла Config.config</param>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		public Subscriber(int userId, IConfig config, IAdReader ad)
		{
			_telegramUserId = userId;
			_config = (Config) config;
			_ad = ad;
		}

		/// <summary>
		///		Включение оповещений об изменениях в Active Directory
		/// </summary>
		/// <param name="userName">Имя пользователя (логин) Active Directory</param>
		/// <param name="userPassword">Пароль пользователя Active Directory</param>
		/// <returns></returns>
		public string SignIn(string userName, string userPassword)
		{
			if (IsIdentifiedUser())
			{
				if (!IsAllowedUser())
					return
						"Sorry, You are not allowed to get access this service because of the Bot Config. Contact your system administrator for help.";
				return "You are already SignedIn.";
			}

			if (!_ad.IsIdentifiedUser(userName, userPassword, _config.AllowedAdGroups))
				return
					"Sorry, You are not allowed to get access this service because of you Active Directory Account permissions. Contact your system administrator for help.";

			var user = new TelegramUser
			{
				Allowed = true,
				TelegramId = _telegramUserId,
			};

			try
			{
				_config.TelegramUsers.Add(user);
				_config.SetNewUser(user);
			}
			catch (Exception ex)
			{
				if (IsIdentifiedUser())
					_config.TelegramUsers.Remove(user);
				return $"Some error occured on creating new user: {ex.Message}";
			}

			return "Congratulations. You was successfully SignedIn.";
		}

		/// <summary>
		///		Включение оповещений об изменениях в Active Directory
		/// </summary>
		/// <returns></returns>
		public string SignOut()
		{
			if (!IsIdentifiedUser()) return "You are not SignedIn yet. Aren't you?";

			var user = _config.TelegramUsers.Find(x => x.TelegramId == _telegramUserId);

			try
			{
				_config.RemoveUser(user);
			}
			catch (Exception ex)
			{
				if (!IsIdentifiedUser())
					_config.TelegramUsers.Add(user);
				return $"Some error occured on removing user: {ex.Message}";
			}

			return "We are sorry, that You left us. Bye.";
		}

		/// <summary>
		///		Проверка (в файле Config.config), подписан ли пользователь на получение оповещений
		/// </summary>
		/// <returns></returns>
		public bool IsIdentifiedUser() => _config.TelegramUsers.Exists(x => x.TelegramId == _telegramUserId);

		/// <summary>
		///		Проверка (в файле Config.config), разрешено ли пользователю получать оповещения
		/// </summary>
		/// <returns></returns>
		public bool IsAllowedUser() => _config.TelegramUsers.Exists(x => x.TelegramId == _telegramUserId && x.Allowed);
	}
}