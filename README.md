# discord-uwu-bot
Discord bot that converts regular English into UwU-speak.

### Commands
* `uwu*that` - Uwuify a message in chat. Reply to a message to uwuifiy it.
* `uwu*this` - Uwuify text. The rest of the message will be uwuified.

### Requirements
* .NET 5+
* Windows, Linux, or MacOS. A version of Linux w/ SystemD (such as Ubuntu) is required to use the built-in install script and service definition.

### Setup

#### Ubuntu / SystemD Linux
Fod Ubuntu (or other SystemD-based Linux systems), an install script and service definition are provided. This install script will create a service account (default `uwubot`), a working directory (default `/opt/UwuBot`), and a SystemD service (default `uwubot`). This script can update an existing installation and will preserve the `appsettings.Production.json` file containing your Discord token and other configuration values.
1. Register a bot with the [Discord Developer Portal](https://discord.com/developers/docs/intro), and get an auth / access token.
2. Build the bot, or download pre-built binaries.
3. Run `sudo install.sh`.
4. \[First install only] Edit `/opt/UwuBot/appsettings.Production.json` and add your Discord token from part 1.
5. Run `sudo systemctl start uwubot` to start the bot.

#### Other OS
For non-Ubuntu systems, manual setup is required. The steps below are the bare minimum to run the bot, and do not incude steps needed to create a persistent service.
1. Register a bot with the [Discord Developer Portal](https://discord.com/developers/docs/intro), and get an auth / access token.
2. Build the bot, or download pre-built binaries.
3. Copy the files to a safe location.
4. Edit `appsettings.Production.json` and add your Discord token from part 1.
5. Run `dotnet DiscordUwuBot.Main.dll` to start the bot.
