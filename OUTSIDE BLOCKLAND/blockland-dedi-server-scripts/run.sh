#!/bin/sh

NO_SCREEN=false
ATTACH=false
GAMEMODE="Custom"

OPTIND=1
while getopts "ag:hz" opt; do
	case "$opt" in
	a)	ATTACH=true
		;;
	g)	GAMEMODE=$OPTARG
		;;
	h)	echo "Blockland Dedicated Server Script"
		echo "version 1.1 -- August 29th, 2017 20:32 CDT"
		echo "TheBlackParrot (BL_ID 18701)"
		echo ""
		echo "Usage: ./run.sh [options]"
		echo ""
		echo "Options:	-a	Automatically attach to the session		[default false]"
		echo "		-g	Specify a gamemode				[default \"Custom\"]"
		echo "		-z	Don't attach to a seperate session		[default false]"
		exit 1
		;;
	z)	NO_SCREEN=true
		ATTACH=false
		;;
	esac
done
shift $((OPTIND-1))
[ "$1" = "--" ] && shift


SERVER_PATH=$(dirname "$(readlink -f "$0")")
echo "Using $SERVER_PATH as the server directory."
PORT=$(cat "$SERVER_PATH/config/server/prefs.cs" | grep '$Pref::Server::Port' | sed 's/[^0-9]*//g')
echo "Should be running on port $PORT."
SERVER_NAME="BL$PORT"


if [ ! -f "$SERVER_PATH/Blockland.exe" ]; then
	echo "Blockland executable is missing!"
	exit 1
fi

if [ ! -f "$SERVER_PATH/Add-Ons/Gamemode_$GAMEMODE/gamemode.txt" ]; then
	if [ ! -f "$SERVER_PATH/Add-Ons/GameMode_$GAMEMODE/gamemode.txt" ]; then
		if [ ! -f "$SERVER_PATH/Add-Ons/Gamemode_$GAMEMODE.zip" ]; then
			if [ ! -f "$SERVER_PATH/Add-Ons/GameMode_$GAMEMODE.zip" ]; then
				echo "Gamemode_$GAMEMODE does not exist in your Add-Ons folder!"
				exit 1
			fi
		fi
	fi
fi


if [ $(screen -list | grep -c "$SERVER_NAME") -gt 0 ]; then
	echo "Session already running on screen $SERVER_NAME, please shut that down first."
	exit 1
fi


if [ $NO_SCREEN = false ]; then
	screen -dmS \
		"$SERVER_NAME" \
		xvfb-run -a \
			-n 100 \
			-e /dev/stdout \
			wine wineconsole \
				--backend=curses \
				"$SERVER_PATH/Blockland.exe" \
				ptlaaxobimwroe \
				-dedicated \
				-gamemode "$GAMEMODE"
else
	xvfb-run -a \
		-n 100 \
		-e /dev/stdout \
		wine wineconsole \
			--backend=curses \
			"$SERVER_PATH/Blockland.exe" \
			ptlaaxobimwroe \
			-dedicated \
			-gamemode "$GAMEMODE"
fi

if [ $NO_SCREEN = false ]; then
	sleep 3

	if [ $(screen -list | grep -c "$SERVER_NAME") -gt 0 ]; then
		echo "Session started on screen $SERVER_NAME"
		if [ "$ATTACH" = true ]; then
			screen -x "$SERVER_NAME"
		else
			screen -list
		fi
	else
		echo "Failed to start server."
		exit 1
	fi
fi