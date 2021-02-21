using System;
using System.Collections.Concurrent;
using System.Linq;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	internal class AdNotifyCollection
	{
		private static ConcurrentDictionary<string, AdNotifyMessage> NotifyMessages { get; }
		public static int Count => NotifyMessages.Count;

		static AdNotifyCollection()
		{
			NotifyMessages = new ConcurrentDictionary<string, AdNotifyMessage>();
		}

		public void Push(AdNotifyMessage message)
		{
			NotifyMessages.AddOrUpdate(message.Name, message, (key, val) => {
				val.Property += Environment.NewLine + message.Property;
				val.Value += Environment.NewLine + message.Value;
				return val;
			});
		}

		public AdNotifyMessage Pop()
		{
			var msg = NotifyMessages.LastOrDefault().Value;
			NotifyMessages.TryRemove(msg.Name, out var ret);
			return ret;
		}
	}
}
