function serverCmdJoin(%client) {
	if(%client.inGame) {
		return;
	}

	//if($QuicklyNow::InProgress || !$QuicklyNow::AcceptingPlayers) {
	//	return;
	//}

	if($QuicklyNow::InProgress) {
		return;
	}

	%client.inGame = true;
	%client.spawnPlayer();

	messageAll('', "\c4" @ %client.name SPC "\c6has joined the game!");

	%mg = $DefaultMinigame;

	%count = 0;
	for(%i=0;%i<%mg.numMembers;%i++) {
		%client = %mg.member[%i];
		if(isObject(%client.player)) {
			if(%client.inGame) {
				%count++;
			}
		}
	}

	if(%mg.numMembers == %count) {
		cancel($QuicklyNow::GameSched);
		%mg.preGameLoop();
	}
}