using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	/// <summary>
	///		Базовый класс для формировния ответов разного типа
	/// </summary>
	internal abstract class ResponseBase
	{
		/// <summary>
		///		Сообщение из чата в виде подстрок
		/// </summary>
		protected List<string> MessagesIn;

		protected ResponseBase()
		{
			NeedToClean = false;
		}

		/// <value>Признак того, необходимо ли удаление сообщения пользователя перед отправкой ответа</value>
		public bool NeedToClean { get; protected set; }
		/// <value>Ответ в виде строки</value>
		public string Message { get; protected set; }

		public virtual async Task Init()
		{
			Message = "Invalid message format, use /help command for more information";
		}

		/// <summary>
		///		Обход объекта <paramref name="responseObj"/>, содержащего в себе имена и значения полей измененного элемента Active Directory, и формирование ответа в виде строки
		/// </summary>
		/// <param name="responseObj">Заполненная модель (объект-наследник абстрактного класса ObjectInfo) с теми полями объета Active Directory, которые были изменены</param>
		/// <returns>Ответ string</returns>
		protected string ParseResponseObject(ObjectInfo responseObj)
		{
			var t = responseObj.GetType();
			var props = t.GetProperties();
			var sb = new StringBuilder();

			foreach (var prop in props)
				if (prop.GetIndexParameters().Length == 0)
					sb.AppendLine($"{prop.Name}: {prop.GetValue(responseObj)}");
				else
					sb.AppendLine($"{prop.Name}: <Indexed>");

			return sb.ToString();
		}

		/// <summary>
		///		Обход списка строк  <paramref name="responseObj"/> и формирование ответа в виде одной строки
		/// </summary>
		/// <param name="responseObj">Список строк, который содержит несколько значений измененного поля объета Active Directory</param>
		/// <returns>Ответ String</returns>
		protected string ParseResponseList(IEnumerable<string> responseObj)
		{
			var sb = new StringBuilder();

			foreach (var s in responseObj)
				sb.AppendLine(s);

			return sb.ToString();
		}
	}
}