using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot
{
	/// <summary>
	///     Подключение компонентов (сервисов) к боту
	/// </summary>
	
	internal static class StartUp
	{
		private static App _services;
		private static Logger _logger;

		/// <summary>
		///		Подключение компонентов (сервисов) к боту
		/// </summary>
		public static void Initialize()
		{
			_services = App.Instance();

			_services.Add(Logger.Instance());
			_services.Add(Config.Instance());
			_services.Add(Ad.Instance());
			_services.Add(AdSnapshot.Instance());

			_logger = (Logger) _services.GetService<Logger>();

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