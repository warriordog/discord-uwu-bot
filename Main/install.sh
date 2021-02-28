#!/bin/bash

# Define variables
BOT_USER="uwubot"
BOT_DIR="/opt/UwuBot"
BOT_BIN_DIR="$BOT_DIR/bin"
BOT_SERVICE="uwubot"
BOT_SERVICE_SOURCE="$BOT_SERVICE.service"
BOT_SERVICE_TARGET="/etc/systemd/system/$BOT_SERVICE.service"

# Stop service if running
if [ -f "$BOT_SERVICE_TARGET" ]; then
    systemctl -q stop "$BOT_SERVICE"
    systemctl -q disable "$BOT_SERVICE"
fi

# Create user if not exist
if ! id -u $BOT_USER > /dev/null 2>&1; then
    useradd -r -m -d "$BOT_DIR" -s "/usr/sbin/nologin" $BOT_USER
fi

# Delete bot binaries
if [ -d "$BOT_BIN_DIR/" ]; then
    rm -rf "$BOT_BIN_DIR"
fi

# Create bot directories if missing
if [ ! -d "$BOT_DIR/" ]; then
    mkdir "$BOT_DIR/" 
fi
if [ ! -d "$BOT_BIN_DIR/" ]; then
    mkdir "$BOT_BIN_DIR/"
fi

# Install bot files
cp -rf * "$BOT_BIN_DIR/"
rm -f "$BOT_BIN_DIR/appsettings.Development.json"
mv "$BOT_BIN_DIR/appsettings.json" "$BOT_DIR/appsettings.json"
mv "$BOT_BIN_DIR/start.sh" "$BOT_DIR/start.sh"
chmod ug+x "$BOT_DIR/start.sh"

# Deploy default config if not exist
if [ ! -f "$BOT_DIR/appsettings.Production.json" ]; then
    mv "$BOT_BIN_DIR/appsettings.Production.json" "$BOT_DIR/appsettings.Production.json"
    echo Default configuration deployed - you must add your Discord token to "$BOT_DIR/appsettings.Production.json".
fi

# Set permissions
chown -R root:$BOT_USER "$BOT_DIR"
chmod -R o-rwx "$BOT_DIR"

# Install service
cp -rf "$BOT_SERVICE_SOURCE" "$BOT_SERVICE_TARGET"
chmod ug+x "$BOT_SERVICE_TARGET"
systemctl -q daemon-reload
systemctl -q enable $BOT_SERVICE

# Success
echo UwU Bot has been installed. Start it with "systemctl start $BOT_SERVICE".