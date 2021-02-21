namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Оповещение: объект удален
	/// </summary>
	internal class AdNotifyMessageObjectDeleted : AdNotifyMessage
	{
		private AdNotifyMessageObjectDeleted(AdNotifyType.ChangingObjectType changingObject,
			AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property,
			string value) : base(changingObject, changingOperation, schemeClass, name, property, value)
		{
		}

		public AdNotifyMessageObjectDeleted(string schemeClass, string name) : this(
			AdNotifyType.ChangingObjectType.Any, AdNotifyType.ChangingOperationType.Deleted, schemeClass, name,
			null, null)
		{
		}
	}
}
