namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public interface IService
	{
		void Add<T>(T service) where T : Decorator;

		Decorator GetService<T>() where T : Decorator;
	}
}