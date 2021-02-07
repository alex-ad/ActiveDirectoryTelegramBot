using System.Collections.Generic;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
    class GroupUsersInfo : GroupInfo
    {
        private List<UserInfo> UserList { get; set; }
    }
}
