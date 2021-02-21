using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Ответ на запрос информации о группе Active Directory по ее имени
	/// </summary>
	/// <remarks>Наследник класса ResponseBase</remarks>
	internal class ResponseGroupByName : ResponseBase
	{
		private readonly IAdReader _ad;

		/// <summary>
		///		Ответ на запрос информации о группе Active Directory по ее имени
		/// </summary>
		/// <param name="message">Запрос в виде списка подстрок</param>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		public ResponseGroupByName(List<string> message, IAdReader ad)
		{
			MessagesIn = message;
			_ad = ad;
		}

		/// <summary>
		///		Формирование ответа на запрос информации о группе Active Directory по ее имени
		/// </summary>
		public override async Task Init()
		{
			await base.Init();

			if (MessagesIn.Count() < 2 || string.IsNullOrEmpty(MessagesIn[1]))
				return;

			await Task.Run(() =>
			{
				var groupPrincipal = _ad.GetGroupObjectByName(MessagesIn[1]);
				if (groupPrincipal == null)
					return;

				var sb = new StringBuilder();

				var groupData = new GroupInfo
				{
					Name = groupPrincipal.DistinguishedName,
					Description = groupPrincipal.Description
				};

				var userData = new List<string>(_ad.GetUserNamesByGroupObject(groupPrincipal)).OrderBy(x => x);

				sb.AppendLine(ParseResponseObject(groupData)).AppendLine(ParseResponseList(userData));

				Message = sb.ToString();
			});
		}
	}
}