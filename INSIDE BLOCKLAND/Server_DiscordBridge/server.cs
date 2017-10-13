function initDiscordBridge() {
	if(!isObject(DiscordTCPObject)) {
		new TCPObject(DiscordTCPObject);
	} else {
		DiscordTCPObject.disconnect();
	}

	%obj = DiscordTCPObject;
	%obj.connect("127.0.0.1:59999");

	if(!isObject(DiscordTCPLines)) {
		// blockland likes to merge multiple send commands being called at once into one line
		// "does this look stupid?" yes, but it's easy
		new GuiTextListCtrl(DiscordTCPLines);
	} else {
		DiscordTCPLines.clear();
	}
}
initDiscordBridge();

function DiscordTCPLines::send(%this, %data) {
	%this.addRow(getSimTime(), %data);
	if(!isEventPending(%this.checkToSendSched)) {
		%this.checkToSend();
	}
}

function DiscordTCPLines::checkToSend(%this) {
	if(%this.rowCount() <= 0) {
		return;
	}

	%this.checkToSendSched = %this.schedule(100, checkToSend);

	%data = %this.getRowText(0);
	%this.removeRow(0);

	DiscordTCPObject.send(%data);
}

function DiscordTCPObject::onConnected(%this) {
	cancel($DiscordBridgeConnectRetryLoop);

	echo("Connected to the Discord bridge.");
	DiscordTCPLines.send("new" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");
}

function DiscordTCPObject::onConnectFailed(%this) {
	echo("Trying to connect to the Discord bridge again (failed to connect)...");
	$DiscordBridgeConnectRetryLoop = %this.schedule(10000, connect, "127.0.0.1:59999");
}

function DiscordTCPObject::onDisconnect(%this) {
	echo("Trying to connect to the Discord bridge again (disconnected)...");
	$DiscordBridgeConnectRetryLoop = %this.schedule(10000, connect, "127.0.0.1:59999");
}

function DiscordTCPObject::onLine(%this, %line) {
	if(trim(getField(%line, 0)) $= "Connected.") {
		messageAll('', "<color:7289da>A bridge between this server and a Discord channel has been established. <color:ffffff>Use '@di' to communicate over it.");
		return;
	}

	%cmd = getField(%line, 0);
	switch$(%cmd) {
		case "msg":
			%msg = stripMLControlChars(getField(%line, 2));
			// thanks Greek
			for(%i = getWordCount(%msg) - 1; %i >= 0; %i --)
			{
				%word = getWord(%msg, %i);
				%pos = strPos(%word, "://") + 3;
				%pro = getSubStr(%word, 0, %pos);
				%url = getSubStr(%word, %pos, strLen(%word));

				if((%pro $= "http://" || %pro $= "https://" || %pro $= "ftp://") &&
					strPos(%url, ":") == -1)
				{
					%word = "<sPush><a:" @ %url @ ">" @ %url @ "</a><sPop>";
					%msg = setWord(%msg, %i, %word);
				}
			}

			if(getField(%line, 3) $= "global") {
				messageAll('', "\c7[DISCORD/GLOBAL] <color:ff4444>" @ stripMLControlChars(getField(%line, 1)) @ "<color:ffffff>:" SPC %msg);
			} else {
				messageAll('', "\c7[DISCORD] <color:7289da>" @ stripMLControlChars(getField(%line, 1)) @ "<color:ffffff>:" SPC %msg);
			}

		case "kill" or "kick":
			%victim_search = getField(%line, 2);
			%victim = findClientByName(%victim_search);
			if(!isObject(%victim)) {
				%victim = findClientByBL_ID(%victim_search);
				if(!isObject(%victim)) {
					DiscordTCPLines.send("cmd_failed" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB "The victim does not exist on the server.\r\n");
					return;
				}
			}

			switch$(%cmd) {
				case "kill":
					if(isObject(%victim.player)) {
						%victim.player.kill();
						messageClient(%victim, '', "You were remotely forcekilled by an admin.");
					} else {
						DiscordTCPLines.send("cmd_failed" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB "The victim is not alive.\r\n");
						return;
					}

				case "kick":
					%victim.delete("You were remotely kicked by" SPC getField(%line, 1) SPC "over Discord.");
					return;
			}

			DiscordTCPLines.send("cmd_ok" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");

		case "players":
			DiscordTCPLines.send("players_start" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");

			for(%i=0;%i<ClientGroup.getCount();%i++) {
				%client = ClientGroup.getObject(%i);
				DiscordTCPLines.send("players_line" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB %client.getDRank() TAB %client.name TAB %client.bl_id TAB getDTimestamp($Sim::Time - %client.connectedAt) @ "\r\n");
			}

			DiscordTCPLines.send("players_end" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");

		case "reload":
			messageAll('', "A command was issued to reload the Discord bridge.");
			exec("Add-Ons/Server_DiscordBridge/server.cs");
			DiscordTCPLines.send("cmd_ok" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");

		case "voice":
			switch$(getField(%line, 2)) {
				case "join":
					messageAll('', "\c7[DISCORD] <color:7289da>" @ stripMLControlChars(getField(%line, 1)) SPC "<color:ffffff>has <color:00ff00>joined <color:ffffff>voice chat.");
				case "leave":
					messageAll('', "\c7[DISCORD] <color:7289da>" @ stripMLControlChars(getField(%line, 1)) SPC "<color:ffffff>has <color:ff0000>left <color:ffffff>voice chat.");
			}

		case "ban":
			messageAll('', "<color:7289da>" @ stripMLControlChars(getField(%line, 1)) SPC "\c6has issued a ban.");
			banBLID(stripMLControlChars(getField(%line, 2)), stripMLControlChars(getField(%line, 3)), stripMLControlChars(getField(%line, 4)));
			DiscordTCPLines.send("cmd_ok" TAB $Pref::Server::Name TAB $Pref::Server::Port @ "\r\n");
	}
}

function getDTimestamp(%val) {
	%raw = mFloor(%val);
	%secs = %raw % 60;
	%mins = mFloor(%raw / 60) % 60;
	%hrs = mFloor(%raw / 60 / 60) % 24;
	%days = mFloor(%raw / 60 / 60 / 24);

	return %days @ "d" SPC %hrs @ "h" SPC %mins @ "m" SPC %secs @ "s";
}

function GameConnection::getDRank(%this) {
	if(getNumKeyID() == %this.bl_id || %this.bl_id == 999999) { return "H"; }
	if(%this.isSuperAdmin) { return "S"; }
	if(%this.isAdmin) { return "A"; }
	return "-";
}

package DiscordBridgePackage {
	function serverCmdMessageSent(%client, %msg) {
		if(getSubStr(%msg, 0, 3) $= "@di") {
			%words = getWords(%msg, 1);
			if(trim(%words) !$= "" && !%client.isSpamming) {
				DiscordTCPLines.send("chat" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB %client.name SPC "(" @ %client.bl_id @ ")" TAB %words  @ "\r\n");
			}
		}

		return parent::serverCmdMessageSent(%client, %msg);
	}

	function GameConnection::autoAdminCheck(%this) {
		if(%this.bl_id $= "") {
			return parent::autoAdminCheck(%this);
		}

		%this.connectedAt = $Sim::Time;

		DiscordTCPLines.send("connect" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB %this.name SPC "(" @ %this.bl_id @ ")\r\n");
		messageClient(%this, '', "Use '@di' to talk to the connected Discord guild.");

		DiscordTCPLines.send("topic" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB ClientGroup.getCount() TAB $Pref::Server::MaxPlayers TAB getDTimestamp($Sim::Time) @ "\r\n");

		return parent::autoAdminCheck(%this);
	}

	function GameConnection::onClientLeaveGame(%this) {
		if(%this.bl_id $= "") {
			return parent::autoAdminCheck(%this);
		}
		
		DiscordTCPLines.send("disconnect" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB %this.name SPC "(" @ %this.bl_id @ ")\r\n");

		DiscordTCPLines.send("topic" TAB $Pref::Server::Name TAB $Pref::Server::Port TAB ClientGroup.getCount()-1 TAB $Pref::Server::MaxPlayers TAB getDTimestamp($Sim::Time) @ "\r\n");
		return parent::onClientLeaveGame(%this);
	}

	function onServerDestroyed() {
		DiscordTCPObject.disconnect();
		parent::onServerDestroyed();
	}
};
activatePackage(DiscordBridgePackage);