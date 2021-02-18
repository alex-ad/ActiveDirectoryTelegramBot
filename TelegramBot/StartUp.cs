using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
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
	// TODO Вся связь между классами - через интерфейсы
	// TODO v2 Активные сервисы брать из конфига
	// TODO Инфа о группах
	// TODO Инфа о компах
	// TODO Убрать лишнюю прослойку из AD-ADTree-ADReader ???

	static class StartUp
	{
		private static App _services;
		private static Logger.Logger _logger;

		public static void Initialize()
		{
			_services = App.Instance();

			_services.Add(Logger.Logger.Instance());
			_services.Add(Config.Config.Instance());
			_services.Add(Ad.Instance());
			_services.Add(AdSnapshot.Instance());

			_logger = (Logger.Logger)_services.GetService<Logger.Logger>();
			
			_services.Init(
				_logger,
				_services.GetService<Config.Config>(),
				_services.GetService<Ad>()
				);
			
			_logger.Log("All services are initialized. See log above for more information.", OutputTarget.Console);
		}
	}
}
