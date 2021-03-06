# discord-uwu-bot
A Discord bot that translates regular text into UwU-speak.

### Examples
|English|UwU|
|-------|---|
|Hello, world!|Hewlo, wowld! UwU!|
|Lorem ipsum dolar sit amet|Lowem ipsum dolaw sit amet UwU!|
|I'll have you know I graduated top of my class in the Navy Seals|I'wl have you knyow I gwawduatewd top of my class in dwe Nyavy Seals UwU!|

### Commands
* `uwu*that` - Uwuify a message in chat. Reply to a message to uwuifiy it.
* `uwu*this` - Uwuify text. The rest of the message will be uwuified.

### Discord Requirements (OAuth2)
To join the bot to a server, you must grant permissions integer `2048`. This consists of:
* `bot` scope
* `Send Messages` permission

### System Requirements
* .NET 5+
* Windows, Linux, or MacOS. A version of Linux with SystemD (such as Ubuntu) is required to use the built-in install script and service definition.

### Setup
Before you can run discord-uwu-bot, you need a Discord API Token. You can get this token by creating and registering a bot at the [Discord Developer Portal](https://discord.com/developers/docs/intro). You can use any name or profile picture for your bot. Once you have registered the bot, generate and save a Token.

#### Ubuntu / SystemD Linux
For Ubuntu (or other SystemD-based Linux systems), an install script and service definition are provided. This install script will create a service account (default `uwubot`), a working directory (default `/opt/UwuBot`), and a SystemD service (default `uwubot`). This script can update an existing installation and will preserve the `appsettings.Production.json` file containing your Discord Token and other configuration values.
1. Compile the bot or download pre-built binaries.
2. Run `sudo ./install.sh`.
3. \[First install only] Edit `/opt/UwuBot/appsettings.Production.json` and add your Discord token.
4. Run `sudo systemctl start uwubot` to start the bot.

#### Other OS
For non-Ubuntu systems, manual setup is required. The steps below are the bare minimum to run the bot, and do not incude steps needed to create a persistent service.
1. Compile the bot or download pre-built binaries.
2. Edit `appsettings.Production.json` and add your Discord token.
3. Run `dotnet DiscordUwuBot.Main.dll` to start the bot.
