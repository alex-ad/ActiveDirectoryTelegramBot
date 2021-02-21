namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	internal class AdNotifyMessageGroupModified : AdNotifyMessage
	{
		private AdNotifyMessageGroupModified(AdNotifyType.ChangingObjectType changingObject, AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property, string value) : base(changingObject, changingOperation, schemeClass, name, property, value) { }

		public AdNotifyMessageGroupModified(string schemeClass, string name, string property, string value) : this(AdNotifyType.ChangingObjectType.Group, AdNotifyType.ChangingOperationType.Modified, schemeClass, name, property, value) { }
	}
}
