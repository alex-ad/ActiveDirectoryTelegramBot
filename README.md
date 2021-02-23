# ActiveDirectoryTelegramBot
Informational TelegramBot for Active Directory (**alpha: sometimes testing and fixing**)
***
## Features:
* TelegramBot can retrieve some information abot the ActiveDirectory objects: users, computers, groups
* Any user can subscribe to notifications on Active Directory changes
***
## Commands (via chat)
* **/UserByLogin** (__/ul__, __/u__) <**USER_LOGIN**>: Get user data by AccountName
* **/UserByName** (__/un__) <**USER_NAME**> : Get user data by FullName (DisplayName)
* **/Group** (__/g__) <**GROUP_NAME**>: Get group data by GroupName
* **/Computer** (__/c__) <**COMPUTER_NAME**> : Get computer data by ComputerName
* **/NotificationsOn** (__/non__) **-u**<**USER_AD_ACCOUNT_NAME**> **-p**<**USER_AD_PASSWORD**> : Subscribe to notifications on AD changes
* **/NotificationsOff** (__/nof__) : Unsubscribe to notifications on AD changes
***
## Config syntax (file "Config.config")
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <AD>
    <UserName>AD_USER_NAME_FOR_READING</UserName>
    <UserPassword>AD_USER_PASSWORD</UserPassword>
    <ServerName>DOMAIN.NAME</ServerName>
  </AD>
  <TelegramBot>
    <Token>YOUR_TELEGRAM_BOT_TOKEN</Token>
  </TelegramBot>
  <UsersAccessList>
    <AllowedAdGroups>
      <Group>LIST_OF_GROUP_NAMES_WHICH_MEMBERS_ARE_ALLOWED_TO_USE_THIS_BOT</Group>
      <Group>LIST_OF_GROUP_NAMES_WHICH_MEMBERS_ARE_ALLOWED_TO_USE_THIS_BOT</Group>
    </AllowedAdGroups>
  </UsersAccessList>
</configuration>
```
***
## How to get TelegramBot Token?
You have to activate your own TelegramBot and retrieve there individual token. For more information visit [BotFather](https://t.me/botfather)
***
## TODO (may be... in version-2...)
- [ ] Subsctibe to different types of Active Directory events (add, remove, modify)
- [ ] Subscribe to Active Directory changes on different objects (user, group, computer)
- [ ] Subscribe and Unsubscribe via GUI-buttons
- [ ] Add active hyperlink elements (and context menus), e.g. clicking on ComputerName you can get information about computer, and so on
- [ ] Hot reloading of the bot via chat-command
- [ ] Modifiyng config via chat-commands
- [ ] Add Admin who can modify and reload config
- [ ] Search for user by a part of full name
- [ ] and so on
