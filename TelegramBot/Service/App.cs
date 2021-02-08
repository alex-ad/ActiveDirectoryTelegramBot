using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public class App : IService, IComponent
	{
		private static List<Decorator> _services;
		private static App _instance;

		public static App Instance(params IComponent[] decorators)
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
			//_services.Add(service);
			_services.Insert(0, service);
		}

		public Decorator GetService<T>() where T : Decorator
		{
			return _services.OfType<T>()?.FirstOrDefault();
		}

		public void Init()
		{
			_services.LastOrDefault()?.Init();
		}
	}
}
