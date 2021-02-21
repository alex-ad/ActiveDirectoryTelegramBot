using System;
using System.Collections.Concurrent;
using System.Linq;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Добавление и удаление оповещения из очереди оповещений
	/// </summary>
	internal class AdNotifyCollection
	{
		static AdNotifyCollection()
		{
			NotifyMessages = new ConcurrentDictionary<string, AdNotifyMessage>();
		}

		private static ConcurrentDictionary<string, AdNotifyMessage> NotifyMessages { get; }
		public static int Count => NotifyMessages.Count;

		/// <summary>
		///		Добавление оповещения в очередь оповещений
		/// </summary>
		/// <param name="message"></param>
		public void Push(AdNotifyMessage message)
		{
			NotifyMessages.AddOrUpdate(message.Name, message, (key, val) =>
			{
				val.Property += Environment.NewLine + message.Property;
				val.Value += Environment.NewLine + message.Value;
				return val;
			});
		}

		/// <summary>
		///		Eдаление оповещения из очереди оповещений
		/// </summary>
		/// <returns></returns>
		public AdNotifyMessage Pop()
		{
			var msg = NotifyMessages.LastOrDefault().Value;
			NotifyMessages.TryRemove(msg.Name, out var ret);
			return ret;
		}
	}
}