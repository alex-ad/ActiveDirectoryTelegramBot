namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public abstract class Decorator : IComponent
	{
		public IComponent Component { protected get; set; }

		public virtual void Init(params IComponent[] decorators)
		{
			Component?.Init(decorators);
		}
	}
}