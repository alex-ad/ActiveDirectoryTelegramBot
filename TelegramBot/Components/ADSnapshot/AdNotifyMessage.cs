namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	internal abstract class AdNotifyMessage
	{
		private AdNotifyType.ChangingObjectType Object { get; }
		private AdNotifyType.ChangingOperationType Operation { get; }
		public string SchemeClass { get; }
		public string Name { get; }
		public string Property { get; set; }
		public string Value { get; set; }

		private AdNotifyMessage() { }

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
