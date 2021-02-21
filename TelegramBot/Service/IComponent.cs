namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public interface IComponent
	{
		void Init(params IComponent[] decorators);
	}
}