namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Оповещение: пользователь изменен
	/// </summary>
	internal class AdNotifyMessageUserModified : AdNotifyMessage
	{
		private AdNotifyMessageUserModified(AdNotifyType.ChangingObjectType changingObject,
			AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property,
			string value) : base(changingObject, changingOperation, schemeClass, name, property, value)
		{
		}

		public AdNotifyMessageUserModified(string schemeClass, string name, string property, string value) : this(
			AdNotifyType.ChangingObjectType.User, AdNotifyType.ChangingOperationType.Modified, schemeClass, name,
			property, value)
		{
		}
	}
}