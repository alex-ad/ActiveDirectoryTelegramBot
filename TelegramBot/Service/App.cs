using System.Collections.Generic;
using System.Linq;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.HelpMsg;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public class App : IService, IComponent
	{
		private static List<Decorator> _services;
		private static App _instance;
		private static IComponent[] _decorators;
		private static ILogger _logger;
		private static IConfig _config;
		private static IAdReader _ad;

		private App()
		{
		}

		public void Init(params IComponent[] decorators)
		{
			HelpMessage.MsgList = new List<string>();
			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = _decorators?.OfType<IConfig>().FirstOrDefault();
			_ad = _decorators?.OfType<IAdReader>().FirstOrDefault();

			_services[0].Component = new TelegramBot(_logger, _ad, _config);

			_services.LastOrDefault()?.Init(decorators);
		}

		public void Add<T>(T service) where T : Decorator
		{
			if (_services.Count > 0)
			{
				var srv = _services.OfType<T>().FirstOrDefault();
				if (srv != null)
					_services.Remove(srv);
			}

			service.Component = _services.LastOrDefault();
			_services.Add(service);
		}

		public Decorator GetService<T>() where T : Decorator
		{
			return _services.OfType<T>().FirstOrDefault();
		}

		public static App Instance()
		{
			_instance = _instance ?? new App();
			_services = new List<Decorator>();
			return _instance;
		}
	}
}