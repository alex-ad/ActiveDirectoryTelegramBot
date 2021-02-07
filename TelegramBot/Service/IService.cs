using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public interface IService
	{
		void Add<T>(T service) where T : Decorator;

		Decorator GetService<T>() where T : Decorator;
	}
}
