using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Отправка оповещения об изменениях в Active Directory
	/// </summary>
	internal class AdNotifySender
	{
		public delegate void BroadcastMessage(AdNotifyMessage message);

		private static AdNotifySender _instance;
		private static AdNotifyCollection _adNotifier;
		private static bool _active;

		private AdNotifySender()
		{
		}

		public static event BroadcastMessage OnBroadcastMessage;

		public static AdNotifySender Instance()
		{
			_instance = _instance ?? new AdNotifySender();
			_adNotifier = _adNotifier ?? new AdNotifyCollection();
			_active = false;
			AdSnapshot.OnAdChanged += SendNotifyFromQueue;

			return _instance;
		}

		/// <summary>
		///		Отправка оповещений об изменениях в Active Directory, находящихся в очереди оповещений
		/// </summary>
		private static void SendNotifyFromQueue()
		{
			if (_active) return;
			_active = true;
			IterateNotifyCollection();
		}

		/// <summary>
		///		Проход по очереди оповещений и вызов события-оповещения об изменении
		/// </summary>
		private static async void IterateNotifyCollection()
		{
			await Task.Run(() =>
			{
				while (AdNotifyCollection.Count > 0)
				{
					var message = _adNotifier.Pop();
					OnBroadcastMessage?.Invoke(message);
				}
			});

			_active = false;
		}
	}
}