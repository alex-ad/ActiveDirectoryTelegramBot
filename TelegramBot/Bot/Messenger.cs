using System;
using System.Collections.Generic;
using System.Linq;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;
using Telegram.Bot.Types;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///     Ответ в зависимости от запроса
	/// </summary>
	internal class Messenger
	{
		private readonly IAdReader _ad;
		private readonly IConfig _config;
		private readonly IComponent[] _decorators;
		private readonly User _telegramUser;


		/// <summary>
		///		Создание ответа в зависимости от запроса
		/// </summary>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		/// <param name="telegramUser">Пользователь Telegram, отправивший запрос в чате</param>
		/// <param name="config">Ссылка на конфигурацию</param>
		/// <param name="decorators">Список подключенных в StartUp декораторов (сервисов)</param>
		public Messenger(IAdReader ad, User telegramUser, IConfig config, IComponent[] decorators)
		{
			_ad = ad;
			_telegramUser = telegramUser;
			_config = config;
			_decorators = decorators;
		}

		/// <summary>
		///		Принимает запрос и формирует ответ
		/// </summary>
		/// <param name="msg">Запрос Telegram.Bot.Types</param>
		/// <returns>Ответ типа ResponseBase</returns>
		public ResponseBase DoRequest(Message msg)
		{
			if (string.IsNullOrEmpty(msg.Text))
				return new ResponseHelp();
			var msgParts = MsgParse(msg.Text);

			if (!msgParts.Any())
				return new ResponseHelp();

			if (msgParts[0].EqualsOneOfTheValues(Commands.Help))
				return new ResponseHelp();

			if (_decorators.OfType<IAdReader>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByLogin))
				return new ResponseUserByLogin(msgParts, _ad);

			if (_decorators.OfType<IAdReader>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.UserInfoByName))
				return new ResponseUserByName(msgParts, _ad);

			if (_decorators.OfType<IAdSnapshot>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOn))
				return new ResponseSignIn(msgParts, _ad, _config, _telegramUser.Id);

			if (_decorators.OfType<IAdSnapshot>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.NotificationsOff))
				return new ResponseSignOut(_ad, _config, _telegramUser.Id);

			if (_decorators.OfType<IAdReader>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.ComputerInfo))
				return new ResponseComputerByName(msgParts, _ad);

			if (_decorators.OfType<IAdReader>().FirstOrDefault() != null &&
			    msgParts[0].EqualsOneOfTheValues(Commands.GroupInfo))
				return new ResponseGroupByName(msgParts, _ad);

			return new ResponseHelp();
		}

		/// <summary>
		///		Разбиение строки на список подстрок, где в качестве разделителя служит пробел
		/// </summary>
		/// <param name="message"></param>
		/// <returns>Список подстрок</returns>
		private List<string> MsgParse(string message)
		{
			return message.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}