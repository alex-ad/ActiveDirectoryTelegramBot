using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	internal class TelegramBot : IDisposable, IComponent
	{
		private static TelegramBotClient _bot;
		private static IAdReader _ad;
		private static ILogger _logger;
		private static IConfig _config;

		public TelegramBot(ILogger logger, IAdReader adReader, IConfig config)
		{
			_ad = (Ad)adReader;
			_logger = (Logger.Logger)logger;
			_config = (Config.Config)config;

			_logger.Log("Starting Bot", OutputTarget.Console | OutputTarget.File);
		}

		private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			if (e.Message?.Type != MessageType.Text)
				return;
			var msg = new Messenger(_ad.Request, e.Message.From, _config);

			try
			{
				var response = msg.DoRequest(e.Message);
				await _bot.SendTextMessageAsync(e.Message.From.Id, response.Message);

				if (!string.IsNullOrEmpty(response.EditedMessage))
					//await _bot.EditMessageTextAsync(e.Message.Chat.Id, e.Message.MessageId, response.EditedMessage);
					await _bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);

				if (response.UserData != null)
					await _bot.SendTextMessageAsync(e.Message.From.Id, ParseResponseObject(response.UserData));
				if (response.GroupData?.Count() > 1)
				{
					var groups = ParseResponseList(response.GroupData);
					await _bot.SendTextMessageAsync(e.Message.From.Id, groups);
				}
			}
			catch (Exception ex)
			{
				await _bot.SendTextMessageAsync(e.Message.From.Id, ex.Message);
			}
		}

		private static string ParseResponseObject(object responseObj)
		{
			var t = responseObj.GetType();
			var props = t.GetProperties();
			var sb = new StringBuilder();

			foreach (var prop in props)
			{
				if (prop.GetIndexParameters().Length == 0)
					sb.AppendLine($"{prop.Name}: {prop.GetValue(responseObj)}");
				else
					sb.AppendLine($"{prop.Name}: <Indexed>");
			}

			return sb.ToString();
		}

		private static string ParseResponseList(IEnumerable<string> responseObj)
		{
			var sb = new StringBuilder();

			foreach (var s in responseObj)
				sb.AppendLine(s);

			return sb.ToString();
		}

		private string FormatMessage(AdNotifyMessage message)
		{
			var msg = new StringBuilder();
			msg.AppendLine($"[{message.SchemeClass.ToUpper()}] {message.Name}");
			var props = message.Property.Split(new []{ Environment.NewLine }, StringSplitOptions.None);
			var values = message.Value.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			if (props.Length < 1) return string.Empty;

			for (var i = 0; i < props.Length; i++)
			{
				var val = values[i];
				if ( props[i] == "samaccounttype" ) val = $"({values[i]}) " + (AdType.SamAccountType)int.Parse(values[i]);
				else if ( props[i] == "useraccountcontrol" ) val = $"({values[i]}) " + (AdType.UserAccountControl)int.Parse(values[i]);
				else if ( props[i] == "primarygroupid" ) val = $"({values[i]}) " + (AdType.PrimaryGroupId)int.Parse(values[i]);
				msg.AppendLine($"[{props[i]}] => {val}");
			}

			return msg.ToString();
		}

		private async void SendBroadCastMessage(AdNotifyMessage message)
		{
			await Task.Run(() =>
			{
				var users = _config.TelegramUsers.Where(x => x.Allowed).Select(x => x.TelegramId).ToList();
				var msg = FormatMessage(message);
				foreach ( var u in users )
				{
					_bot.SendTextMessageAsync(u, msg);
				}
			});
		}

		public void Dispose()
		{
			_logger.Log("OMG! The killed Bot. You Bastards!", OutputTarget.Console | OutputTarget.File);
			_bot.StopReceiving();
		}

		public void Init(params IComponent[] decorators)
		{
			Config.Config.OnConfigUpdated += Config_OnConfigUpdated;
			AdNotifySender.OnBroadcastMessage += AdNotifySender_OnBroadcastMessage;
		}

		private void AdNotifySender_OnBroadcastMessage(AdNotifyMessage message)
		{
			SendBroadCastMessage(message);
		}

		private void Config_OnConfigUpdated(IConfig config)
		{
			_config = config;
			if (string.IsNullOrEmpty(_config.TelegramBotToken)) return;

			try
			{
				_bot = new TelegramBotClient(_config.TelegramBotToken);
				_bot.OnMessage += Bot_OnMessage;

				var me = _bot.GetMeAsync().Result;
				_logger?.Log($"{me.FirstName} is ready for receiving commands", OutputTarget.Console);

				_bot.StartReceiving();
			}
			catch (ArgumentException ex)
			{
				_logger?.Log($"TelegramBot initializing error: {ex.Message}", OutputTarget.Console);
			}
			catch (ApiRequestException ex)
			{
				_logger?.Log($"TelegramBot initializing error: {ex.Message}", OutputTarget.Console);
			}
			
		}
	}
}
