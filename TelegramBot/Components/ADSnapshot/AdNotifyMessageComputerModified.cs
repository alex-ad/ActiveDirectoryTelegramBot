namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Оповещение: компьютер изменен
	/// </summary>
	internal class AdNotifyMessageComputerModified : AdNotifyMessage
	{
		private AdNotifyMessageComputerModified(AdNotifyType.ChangingObjectType changingObject,
			AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property,
			string value) : base(changingObject, changingOperation, schemeClass, name, property, value)
		{
		}

		public AdNotifyMessageComputerModified(string schemeClass, string name, string property, string value) : this(
			AdNotifyType.ChangingObjectType.Computer, AdNotifyType.ChangingOperationType.Modified, schemeClass, name,
			property, value)
		{
		}
	}
}