using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot
{
	/// <summary>
	/// Main TelegramBot Module, which receives and sends messages
	/// </summary>
	// TODO v2 Контекстное меню у значимых элементов (юзеры, компы и т.п.
	// TODO v2 Горячая (через команду) перезагрузка конфига
	// TODO v2 Добавить админа (умеет блочить юзеров, перезагружать конфиг, менять конфиг)
	// TODO v2 Бот должен запускаться даже при пустом конфиге (в т.ч. при сбоях в цепочке Init), чтоб админ мог настроить командами, но оставить Завершение , если нет сети
	// TODO v2 Кнопки подписки, но только если получатся уведомления
	// TODO v2 Подписка на самого бота только по ссылке
	// TODO v2 Активные сервисы брать из конфига

	static class StartUp
	{
		private static App _services;
		private static Logger _logger;

		public static void Initialize()
		{
			_services = App.Instance();

			_services.Add(Logger.Instance());
			_services.Add(Config.Instance());
			_services.Add(Ad.Instance());
			_services.Add(AdSnapshot.Instance());

			_logger = (Logger)_services.GetService<Logger>();
			
			_services.Init(
				_logger,
				_services.GetService<Config>(),
				_services.GetService<Ad>(),
				_services.GetService<AdSnapshot>()
				);
			
			_logger.Log("All services are initialized. See log above for more information.", OutputTarget.Console);
		}
	}
}
