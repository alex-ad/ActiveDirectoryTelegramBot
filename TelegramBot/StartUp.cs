﻿using System;
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
	// TODO Контекстное меню у значимых элементов (юзеры, компы и т.п.
	// TODO Красивее вывод списков
	// TODO Проверка логина-пароля при подписке : изменить вывод ошибки
	// TODO Горячая (через команду) перезагрузка конфига
	// TODO Добавить админа (умеет блочить юзеров, перезагружать конфиг, менять конфиг)
	// TODO Бот должен запускаться даже при пустом конфиге (в т.ч. при сбоях в цепочке Init), чтоб админ мог настроить командами, но оставить Завершение , если нет сети
	// TODO Оповещения АД
	// TODO Удаление строки с паролем после авторизации
	// TODO Кнопки подписки, но только если получатся уведомления
	// TODO Подписка на самого бота только по ссылке
	// TODO Ответы (response) сделать разными классами (наследование)
	// TODO Убрать авторизацию по паролю? оставить только - по группе?
	// TODO Вся связь между классами - через интерфейсы

	static class StartUp
	{
		/*private static TelegramBot _bot;
		private static AdReader _ad;
		private static AdConnection _adConnection;
		private static Config.Config _config;
		private static PrincipalContext _adContext;
		private static Logger.Logger _logger;
		private static AdSnapshot _adSnapshot;
		private static AdNotifySender _adNotifySender;*/
		private static App _services;

		// TODO Сделать гибкую инициалтзацию сервисов (Декоратор?)
		public static void Initialize()
		{
			_services = App.Instance();

			/*_logger = Logger.Logger.Instance();
			_config = Config.Config.Instance(_logger);*/

			_services.Add(Logger.Logger.Instance());
			_services.Add(Config.Config.Instance(_services.GetService<Logger.Logger>()));

			_services.Init();

			/*InitLogger()
				.InitConfig()
				.InitAd()
				.InitBot()
				.InitAdSnapshot()
				.InitAdNotifySender();*/
		}

		/*private static bool InitLogger()
		{
			_logger = Logger.Logger.Instance();
			if ( _logger == null )
				return false;
			return true;
		}*/

		/*private static bool InitConfig(this bool next)
		{
			if ( !next )
				return false;

			_config = Config.Config.Instance(_logger);
			return _config.TryGetParamsFromFile();
		}

		private static bool InitAd(this bool next)
		{
			if ( !next )
				return false;

			_logger.Log("Establishing AD connection");

			_adConnection = AdConnection.Instance(_logger, _config);
			if ( _adConnection == null )
				return false;

			if ( !_adConnection.TryConnect(out _adContext) )
				return false;

			_ad = new AdReader(_adContext);
			if ( _ad == null )
				return false;

			return true;
		}

		private static bool InitBot(this bool next)
		{
			if ( !next )
				return false;

			_bot = new TelegramBot(_logger, _ad, _config);

			return true;
		}

		private static bool InitAdSnapshot(this bool next)
		{
			if ( !next )
				return false;

			_adSnapshot = AdSnapshot.Instance(_config);
			_adSnapshot.RunAsync(3000);

			return true;
		}

		private static bool InitAdNotifySender(this bool next)
		{
			if ( !next )
				return false;

			_adNotifySender = AdNotifySender.Instance(_adSnapshot, _config, _bot);

			return true;
		}*/
	}
}