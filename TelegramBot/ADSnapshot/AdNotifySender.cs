using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public class AdNotifySender
	{
		private static AdNotifySender _instance;
		private static Config.Config _config;
		private static AdSnapshot _adSnapshot;
		private static AdNotifyCollection _adNotifier;
		private static TelegramBot _bot;
		private static bool _active;

		protected AdNotifySender() { }

		public static AdNotifySender Instance(AdSnapshot adSnapshot, Config.Config config, TelegramBot bot)
		{
			_instance = _instance ?? new AdNotifySender();
			_config = config;
			_adSnapshot = adSnapshot;
			_adNotifier = _adNotifier ?? new AdNotifyCollection();
			_bot = bot;
			_active = false;
			AdSnapshot.OnAdChanged += SendNotifyFromQueue;

			return _instance;
		}

		private static void SendNotifyFromQueue()
		{
			if ( _active ) return;
			_active = true;
			IterateNotifyCollection();
		}

		private static async void IterateNotifyCollection()
		{
			await Task.Run(() =>
			{
				while ( AdNotifyCollection.Count > 0 )
				{
					var message = _adNotifier.Pop();
					_bot.SendBroadCastMessage(message);
				}

			});

			_active = false;
		}
	}
}
