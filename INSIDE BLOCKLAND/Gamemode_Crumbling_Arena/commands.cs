function serverCmdJoin(%client) {
	if(isObject(%client.player) || !$CrumblingArena::CanJoin) {
		%client.play2D(errorSound);
		return;
	}

	%client.spawnPlayer();
}