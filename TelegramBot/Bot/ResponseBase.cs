using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Bot
{
	abstract class ResponseBase
	{
		protected List<string> MessagesIn;
		public bool NeedToClean { get; protected set; }
		public string Message { get; protected set; }

		protected ResponseBase()
		{
			NeedToClean = false;
		}

		public virtual async Task Init()
		{
			Message = "Invalid message format, use /help command for more information";
		}

		protected string ParseResponseObject(object responseObj)
		{
			var t = responseObj.GetType();
			var props = t.GetProperties();
			var sb = new StringBuilder();

			foreach ( var prop in props )
			{
				if ( prop.GetIndexParameters().Length == 0 )
					sb.AppendLine($"{prop.Name}: {prop.GetValue(responseObj)}");
				else
					sb.AppendLine($"{prop.Name}: <Indexed>");
			}

			return sb.ToString();
		}

		protected string ParseResponseList(IEnumerable<string> responseObj)
		{
			var sb = new StringBuilder();

			foreach ( var s in responseObj )
				sb.AppendLine(s);

			return sb.ToString();
		}
	}
}
