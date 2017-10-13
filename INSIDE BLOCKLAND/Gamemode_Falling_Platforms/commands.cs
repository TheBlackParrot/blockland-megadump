function serverCmdJoin(%client) {
	%mg = $DefaultMinigame;

	if(!$FallingPlatforms::AcceptingPlayers) {
		return;
	}

	// prevents players who are loading from joining
	if(!isObject(%client.camera)) {
		return;
	}

	if(!isObject(%client.player)) {
		%client.spawnPlayer();
	} else {
		return;
	}

	%spawn = BrickGroup_888888.getObject(getRandom(0, BrickGroup_888888.getCount()-1));
	%client.player.setTransform(vectorAdd("0 0 5", %spawn.getTransform()));
	serverPlay3D(%client.joinSound, %spawn.getTransform());
	messageAll('', "\c4" @ %client.name SPC "\c6has joined the game!");
}

function serverCmdLeaderboard(%this, %pb) {
	if(%pb $= "") {
		if(!isObject(FallingPlatformsLeaderboard)) {
			return;
		}
		%list = FallingPlatformsLeaderboard;
	} else {
		if(!isObject(FallingPlatformsPBLeaderboard)) {
			return;
		}
		%list = FallingPlatformsPBLeaderboard;		
	}

	if(getSimTime() - %this.lastleadcmd <= 1000) {
		return;
	}
	%this.lastleadcmd = getSimTime();

	%count = %list.rowCount();
	if(%count > 15) {
		%count = 15;
	}

	for(%i=0;%i<%count;%i++) {
		%row = %list.getRowText(%i);
		if(getField(%row, 1) $= "" || getField(%row, 2) $= "") {
			%list.removeRow(%i);
			%i--;
			continue;
		}
		messageClient(%this, '', "\c3" @ %i+1 @ ". \c6" @ getField(%row, 0) SPC "-\c4" SPC getField(%row, 1));
	}

	messageClient(%this, '', "\c6--------");

	%pos = %list.getRowNumByID(%this.bl_id);
	for(%i=(%pos-2);%i<=(%pos+2);%i++) {
		if(%i < 0 || %i > %list.rowCount()) {
			continue;
		}

		%row = %list.getRowText(%i);
		if(getField(%row, 1) $= "" || getField(%row, 2) $= "") {
			%list.removeRow(%i);
			%i--;
			continue;
		}

		if(%pos == %i) {
			messageClient(%this, '', "\c3" @ %i+1 @ ". \c2" @ getField(%row, 0) SPC "-\c5" SPC getField(%row, 1));
		} else {
			messageClient(%this, '', "\c3" @ %i+1 @ ". \c6" @ getField(%row, 0) SPC "-\c5" SPC getField(%row, 1));
		}
	}
}

function serverCmdToggleSounds(%client) {
	if(%client.noSillySounds) {
		%client.noSillySounds = false;
		%tag = "\c2ON";
	} else {
		%client.noSillySounds = true;
		%tag = "\c0OFF";
	}

	messageClient(%client, '', "\c6Random sound effects are now" SPC %tag);

	%client.saveData();
}

function serverCmdHelp(%client) {
	messageClient(%client, '', "\c4/join \c7-- \c6Join the game.");
	messageClient(%client, '', "\c4/leaderboard \c7-- \c6View the leaderboards.");
	messageClient(%client, '', "\c4/leaderboard pb \c7-- \c6View the leaderboards for personal bests.");
	messageClient(%client, '', "\c4/toggleSounds \c7-- \c6Toggle the random sound effects.");
}