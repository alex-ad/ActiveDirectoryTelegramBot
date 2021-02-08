using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	public class AdFacade : Decorator
	{
		private static IComponent[] _decorators;
		private static ILogger _logger;
		private static AdReader _ad;
		private static AdConnection _adConnection;
		private static PrincipalContext _adContext;
		private static AdFacade _instance;
		private static IConfig _config;

		protected AdFacade() { }

		public static AdFacade Instance(params IComponent[] decorators)
		{
			_instance = _instance ?? new AdFacade();
			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = _decorators?.OfType<IConfig>().FirstOrDefault();

			if (_ad == null)
			{
				_adConnection = AdConnection.Instance(_logger, _config);
				if ( _adConnection != null )
					if ( _adConnection.TryConnect(out _adContext) )
						_ad = new AdReader(_adContext);
			}

			return _instance;
		}

		public override void Init()
		{
			_logger?.Log("Initialize Service: Active Directory", OutputTarget.Console);
			base.Init();
		}
	}
}
