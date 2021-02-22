using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Modules
{
	/// <summary>
	///     Пинг для определение IP-адреса компьютера
	/// </summary>
	internal static class ComputerPinger
	{
		/// <summary>
		///     Пинг для определение IP-адреса компьютера по его имени <paramref name="computerName" />
		/// </summary>
		/// <param name="computerName">Имя компьютера</param>
		/// <returns></returns>
		public static async Task<string> Ping(string computerName)
		{
			var ping = new Ping();
			var result = await ping.SendPingAsync(computerName, 1000);
			return result.Address.ToString();
		}
	}
}