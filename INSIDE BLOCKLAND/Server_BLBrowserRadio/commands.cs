function serverCmdRefreshRadioList(%client) {
	if(!%client.isSuperAdmin) {
		return;
	}

	talk("Refreshing the radio list...");
	getRadioList();
}

function serverCmdNowPlaying(%client) {
	if(!%client.canUtilizeRadio || %client.canUtilizeRadio $= "") {
		return;
	}

	%next = $Server::BLBrowser::CurrentlyPlaying;

	%title = getField(%next, 0);
	%artist = getField(%next, 1);
	%duration = getField(%next, 2);
	%path = getField(%next, 3);
	%elapsed = getTimeString(mFloor($Sim::Time - $Server::BLBrowserRadio::StartedAt));

	messageClient(%client, '', "\c2Now Playing:\c6" SPC %artist SPC "-" SPC %title SPC "\c7[" @ %elapsed SPC "/" SPC getTimeString(mCeil(%duration/1000)) @ "]");
}

function serverCmdToggleMusic(%client) {
	if(%client.hearRadio) {
		%client.hearRadio = false;
		%tag = "\c0OFF";
		commandToClient(%client, 'AWS_LoadUrl', "about:blank");
	} else {
		%client.hearRadio = true;
		%tag = "\c2ON";
	}

	messageClient(%client, '', "\c6The radio is now" SPC %tag);

	%file = new FileObject();

	%file.openForWrite("config/server/BLBrowserRadio/saves/" @ %client.bl_id);

	%file.writeLine(%client.hearRadio);

	%file.close();
	%file.delete();
}