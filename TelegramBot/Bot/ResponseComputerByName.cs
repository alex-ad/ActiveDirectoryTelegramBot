using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Modules;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Ответ на запрос информации о компьютере Active Directory по его имени
	/// </summary>
	/// <remarks>Наследник класса ResponseBase</remarks>
	internal class ResponseComputerByName : ResponseBase
	{
		private readonly IAdReader _ad;

		/// <summary>
		///		Ответ на запрос информации о компьютере Active Directory по его имени
		/// </summary>
		/// <remarks>Наследник класса ResponseBase</remarks>
		/// <param name="message">Запрос в виде списка подстрок</param>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		public ResponseComputerByName(List<string> message, IAdReader ad)
		{
			MessagesIn = message;
			_ad = ad;
		}

		/// <summary>
		///		Формирование ответа на запрос информации о компьютере Active Directory по его имени
		/// </summary>
		/// <returns></returns>
		public override async Task Init()
		{
			await base.Init();

			if (MessagesIn.Count() < 2 || string.IsNullOrEmpty(MessagesIn[1]))
				return;

			await Task.Run(async () =>
			{
				var computerPrincipal = _ad.GetComputerObjectByName(MessagesIn[1]);
				if (computerPrincipal == null)
					return;

				var sb = new StringBuilder();

				var compData = new ComputerInfo
				{
					Enabled = computerPrincipal.Enabled.Value,
					Name = computerPrincipal.DistinguishedName,
					Description = computerPrincipal.Description,
					OS = _ad.GetComputerProperty(computerPrincipal, "operatingSystem"),
					IP = await ComputerPinger.Ping(computerPrincipal.Name),
					LastLogon = computerPrincipal.LastLogon ?? DateTime.MinValue
				};

				sb.AppendLine(ParseResponseObject(compData));
				Message = sb.ToString();
			});
		}
	}
}