using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public class AdNotifyCollection
	{
		//private static Queue<AdNotifyMessage> NotifyMessages { get; }
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
