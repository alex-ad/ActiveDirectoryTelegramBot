using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Service
{
	public abstract class Decorator : IComponent
	{
		public IComponent Component { protected get; set; }

		public virtual void Init(params IComponent[] decorators) => Component?.Init(decorators);
	}
}
