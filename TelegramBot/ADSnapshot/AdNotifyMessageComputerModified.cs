using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	internal class AdNotifyMessageComputerModified : AdNotifyMessage
	{
		private AdNotifyMessageComputerModified(AdNotifyType.ChangingObjectType changingObject, AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property, string value) : base(changingObject, changingOperation, schemeClass, name, property, value) { }

		public AdNotifyMessageComputerModified(string schemeClass, string name, string property, string value) : this(AdNotifyType.ChangingObjectType.Computer, AdNotifyType.ChangingOperationType.Modified, schemeClass, name, property, value) { }
	}
}
