using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public class App : IService, IComponent
	{
		private static List<Decorator> _services;
		private static App _instance;
		private static IComponent[] _decorators;
		private static ILogger _logger;
		private static IConfig _config;
		private static IAdFacade _ad;

		public static App Instance()
		{
			_instance = _instance ?? new App();
			_services = new List<Decorator>();
			return _instance;
		}

		protected App() { }

		public void Add<T>(T service) where T : Decorator
		{
			if (_services.Count > 0)
			{
				var srv = _services.OfType<T>()?.FirstOrDefault();
				if ( srv != null )
					_services.Remove(srv);
			}

			service.Component = _services.LastOrDefault();
			_services.Add(service);
		}

		public Decorator GetService<T>() where T : Decorator
		{
			return _services.OfType<T>()?.FirstOrDefault();
		}

		public void Init(params IComponent[] decorators)
		{
			_services.LastOrDefault()?.Init();

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = _decorators?.OfType<IConfig>().FirstOrDefault();
			_ad = _decorators?.OfType<IAdFacade>().FirstOrDefault();

			new TelegramBot(_logger, _ad, _config);
		}
	}
}
