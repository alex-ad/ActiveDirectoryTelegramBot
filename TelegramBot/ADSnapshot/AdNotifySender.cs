using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	internal class AdNotifySender
	{
		public delegate void BroadcastMessage(AdNotifyMessage message);

		public static event BroadcastMessage OnBroadcastMessage;

		private static AdNotifySender _instance;
		private static IConfig _config;
		private static IAdSnapshot _adSnapshot;
		private static AdNotifyCollection _adNotifier;
		private static bool _active;

		protected AdNotifySender() { }

		public static AdNotifySender Instance(IAdSnapshot adSnapshot, IConfig config)
		{
			_instance = _instance ?? new AdNotifySender();
			_config = config;
			_adSnapshot = adSnapshot;
			_adNotifier = _adNotifier ?? new AdNotifyCollection();
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
					OnBroadcastMessage?.Invoke(message);
				}

			});

			_active = false;
		}
	}
}
