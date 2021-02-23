using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Создание бота для обработки сообщений от пользователей
	/// </summary>
	internal class TelegramBot : IDisposable, IComponent
	{
		private static TelegramBotClient _bot;
		private static IAdReader _ad;
		private static ILogger _logger;
		private static IConfig _config;
		private static IComponent[] _decorators;

		/// <summary>
		///		Создание бота для обработки сообщений от пользователей
		/// </summary>
		/// <param name="logger">Формирование и вывод логов</param>
		/// <param name="adReader">Контекст подключения к Active Directory</param>
		/// <param name="config">Настройки приложения из файла Config.config</param>
		public TelegramBot(ILogger logger, IAdReader adReader, IConfig config)
		{
			_ad = adReader;
			_logger = logger;
			_config = config;

			_logger.Log("Starting Bot...", OutputTarget.Console);
		}

		public void Init(params IComponent[] decorators)
		{
			_decorators = decorators;
			Config.OnConfigUpdated += Config_OnConfigUpdated;
			AdNotifySender.OnBroadcastMessage += AdNotifySender_OnBroadcastMessage;
		}

		public void Dispose()
		{
			_logger.Log("OMG! The killed Bot. You Bastards!", OutputTarget.Console);
			_bot.StopReceiving();
		}

		/// <summary>
		///		Событие при получении запроса
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Message?.Type != MessageType.Text)
				return;
			var msg = new Messenger(_ad, e.Message.From, _config, _decorators);

			try
			{
				var response = msg.DoRequest(e.Message);
				await response.Init();

				await SendTextMessageAsync(e.Message.From.Id, response.Message);
				if (response.NeedToClean)
					await _bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
			}
			catch (Exception ex)
			{
				await _bot.SendTextMessageAsync(e.Message.From.Id, ex.Message);
			}
		}

		/// <summary>
		///		Объединение разных частей ответа в одну строку
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private string FormatMessage(AdNotifyMessage message)
		{
			var msg = new StringBuilder();
			msg.AppendLine($"[{message.SchemeClass.ToUpper()}] {message.Name}");
			var props = message.Property?.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			var values = message.Value?.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			if (props == null || props.Length < 1 || values == null || values.Length < 1)
				return msg.ToString();

			// Ищет, есть ли в ответе элементы, представляющие собой битовую маску, если есть, формирует строковое "человеко-понятное" значение
			for (var i = 0; i < props.Length; i++)
			{
				var val = values[i];
				if (props[i] == "samaccounttype")
					val = $"({values[i]}) " + (AdType.SamAccountType) int.Parse(values[i]);
				else if (props[i] == "useraccountcontrol")
					val = $"({values[i]}) " + (AdType.UserAccountControl) int.Parse(values[i]);
				else if (props[i] == "primarygroupid")
					val = $"({values[i]}) " + (AdType.PrimaryGroupId) int.Parse(values[i]);
				if (!string.IsNullOrEmpty(props[i]))
					msg.AppendLine($"[{props[i]}] => {val}");
			}

			return msg.ToString();
		}

		/// <summary>
		///		Отправка широковещательного ответа всем пользователям, подписанных на получение оповещений об изменениях в Active Directory
		/// </summary>
		/// <param name="message"></param>
		private async void SendBroadCastMessage(AdNotifyMessage message)
		{
			await Task.Run(() =>
			{
				var users = _config.TelegramUsers.Where(x => x.Allowed).Select(x => x.TelegramId).ToList();
				var msg = FormatMessage(message);
				foreach (var u in users) _bot.SendTextMessageAsync(u, msg);
			});
		}

		/// <summary>
		///		Отправка ответа пользователю в ответ на запрос в чате
		/// </summary>
		/// <param name="chatId"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private static async Task SendTextMessageAsync(int chatId, string message)
		{
			// Длина одного сообщения не может превышать 4096 символов, поэтому разбиваем одно сообщение (если оно длинее 4096) на несколько более мелких
			if (message.Length <= 4096)
			{
				await _bot.SendTextMessageAsync(chatId, message);
				return;
			}

			var loop = message.Length / 4096 + (message.Length % 4096 == 0 ? 0 : 1);
			var elapsed = 0;

			for (var i = 1; i <= loop; i++)
			{
				var count = elapsed + 4096 < message.Length ? 4096 : message.Length - elapsed;
				await _bot.SendTextMessageAsync(chatId, message.Substring(elapsed, count));
				elapsed += 4096;
			}
		}

		/// <summary>
		///		Отправить широковещательный ответ при возникновении события-оповещения об изменении в Active Directory
		/// </summary>
		/// <param name="message"></param>
		private void AdNotifySender_OnBroadcastMessage(AdNotifyMessage message)
		{
			SendBroadCastMessage(message);
		}

		/// <summary>
		///		Создание бота. Происходит при возникновении собитыя-оповещения о получении настроек приложения из Config.config
		/// </summary>
		/// <param name="config"></param>
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