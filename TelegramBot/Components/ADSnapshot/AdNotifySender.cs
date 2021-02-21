using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	internal class AdNotifySender
	{
		public delegate void BroadcastMessage(AdNotifyMessage message);

		public static event BroadcastMessage OnBroadcastMessage;

		private static AdNotifySender _instance;
		private static AdNotifyCollection _adNotifier;
		private static bool _active;

		private AdNotifySender() { }

		public static AdNotifySender Instance()
		{
			_instance = _instance ?? new AdNotifySender();
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
