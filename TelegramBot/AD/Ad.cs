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
	public class Ad : Decorator, IAdFacade
	{
		private static IComponent[] _decorators;
		private static ILogger _logger;
		private static AdReader _ad;
		private static AdConnection _adConnection;
		private static PrincipalContext _adContext;
		private static Ad _instance;
		private static IConfig _config;

		public AdReader Request => _ad;

		protected Ad() { }

		public static Ad Instance()
		{
			_instance = _instance ?? new Ad();
			return _instance;
		}

		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = _decorators?.OfType<IConfig>().FirstOrDefault();
			_logger?.Log("Initializing Service: Active Directory...", OutputTarget.Console);

			Config.Config.OnConfigUpdated += Config_OnConfigUpdated;
			Connect();
		}

		private void Config_OnConfigUpdated(Config.Config config)
		{
			_config = config;
			Connect();
		}

		public void Connect()
		{
			_adConnection = AdConnection.Instance(_logger, _config);
			if ( _adConnection != null )
				if ( _adConnection.TryConnect(out _adContext) )
					_ad = new AdReader(_adContext);
		}
	}
}
