﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Ответ на запрос информации о пользователе Active Directory по его имени
	/// </summary>
	/// <remarks>Наследник класса ResponseBase</remarks>
	internal class ResponseUserByName : ResponseBase
	{
		private readonly IAdReader _ad;

		/// <summary>
		///		Ответ на запрос информации о пользователе Active Directory по его имени
		/// </summary>
		/// <param name="message">Запрос в виде подстрок</param>
		/// <param name="ad">Контекст подключения к Active Directory</param>
		public ResponseUserByName(List<string> message, IAdReader ad)
		{
			MessagesIn = message;
			_ad = ad;
		}

		/// <summary>
		///		Ответ на запрос информации о пользователе Active Directory по его имени
		/// </summary>
		/// <returns></returns>
		public override async Task Init()
		{
			await base.Init();

			if (MessagesIn.Count() < 2 || string.IsNullOrEmpty(MessagesIn[1]))
				return;

			await Task.Run(() =>
			{
				var userPrincipal = _ad.GetUserObjectByName($"{MessagesIn[1]} {MessagesIn[2]} {MessagesIn[3]}");
				if (userPrincipal == null)
					return;

				var sb = new StringBuilder();

				var userData = new UserInfo
				{
					Enabled = userPrincipal.Enabled.Value,
					Name = userPrincipal.DisplayName,
					Mail = userPrincipal.EmailAddress,
					TelephoneNumber = userPrincipal.VoiceTelephoneNumber,
					LastLogon = userPrincipal.LastLogon ?? DateTime.MinValue,
					Company = _ad.GetUserProperty(userPrincipal, "Company"),
					Department = _ad.GetUserProperty(userPrincipal, "Department"),
					SamAccountName = userPrincipal.SamAccountName,
					Title = _ad.GetUserProperty(userPrincipal, "Title"),
					Description = userPrincipal.Description
				};

				var groupData = new List<string>(_ad.GetGroupsByUserObject(userPrincipal)).OrderBy(x => x);

				sb.AppendLine(ParseResponseObject(userData)).AppendLine(ParseResponseList(groupData));

				Message = sb.ToString();
			});
		}
	}
}