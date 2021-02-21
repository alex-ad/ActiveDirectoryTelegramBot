namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	/// Список команд, которые принимет бот в чате от пользователя
	/// </summary>
	internal static class Commands
	{
		public static string[] Help = {"/Help", "/h"};
		public static string[] UserInfoByLogin = {"/UserByLogin", "/ul", "/u"};
		public static string[] UserInfoByName = {"/UserByName", "/un"};
		public static string[] GroupInfo = {"/Group", "/g"};
		public static string[] ComputerInfo = {"/Computer", "/c"};
		public static string[] NotificationsOn = {"/NotificationsOn", "/non"};
		public static string[] NotificationsOff = {"/NotificationsOff", "/nof"};
	}
}