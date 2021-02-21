namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	internal static class AdNotifyType
	{
		public enum ChangingObjectType
		{
			Computer,
			User,
			Group,
			Any
		}

		// TODO v2 Заготовка для будущего использования
		public enum ChangingOperationType
		{
			Added,
			Deleted,
			Modified
		}
	}
}