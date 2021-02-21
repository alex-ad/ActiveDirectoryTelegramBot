namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger
{
	internal interface ILogger
	{
		void Log(string message, OutputTarget outputTarget);
	}
}