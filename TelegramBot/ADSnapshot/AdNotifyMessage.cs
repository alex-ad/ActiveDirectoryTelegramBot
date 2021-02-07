using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public abstract class AdNotifyMessage
	{
		public AdNotifyType.ChangingObjectType Object { get; }
		public AdNotifyType.ChangingOperationType Operation { get; }
		public string SchemeClass { get; set; }
		public string Name { get; set; }
		public string Property { get; set; }
		public string Value { get; set; }

		protected AdNotifyMessage() { }

		protected AdNotifyMessage(AdNotifyType.ChangingObjectType changingObject, AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property, string value)
		{
			Object = changingObject;
			Operation = changingOperation;
			SchemeClass = schemeClass;
			Name = name;
			Property = property;
			Value = value;
		}
	}
}
