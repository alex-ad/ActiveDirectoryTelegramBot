using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Ответ на запрос об отказе от получения оповещений об изменениях в Active Directory
	/// </summary>
	internal class ResponseSignOut : ResponseBase
	{
		private readonly IAdReader _ad;
		private readonly IConfig _config;
		private readonly int _userId;

		/// <summary>
		///		Ответ на запрос об отказе от получения оповещений об изменениях в Active Directory
		/// </summary>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		/// <param name="config">Настройки приложения из файла Config.config</param>
		/// <param name="userId">Id пользователя, отправившего запрос в чате</param>
		public ResponseSignOut(IAdReader ad, IConfig config, int userId)
		{
			_ad = ad;
			_config = config;
			_userId = userId;
		}

		/// <summary>
		///		Выключение оповещений об изменениях в Active Directory
		/// </summary>
		/// <returns></returns>
		public override async Task Init()
		{
			await base.Init();

			await Task.Run(() =>
			{
				var user = new Subscriber(_userId, _config, _ad);
				Message = user.SignOut();
			});
		}
	}
}