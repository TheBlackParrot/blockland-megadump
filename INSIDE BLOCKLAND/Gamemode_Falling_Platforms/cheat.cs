function Player::cheatLoop(%this) {
	cancel(%this.cheatLoop);
	%this.cheatLoop = %this.schedule(300, cheatLoop);

	%mask = $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%this.getPosition(), 1, %mask);
	while(%hit = containerSearchNext()) {
		if(isObject(%hit)) {
			%this.cheatHits = 0;
			break;
		}
	}

	if(%this.cheatHits > 20) {
		%this.kill();
		messageClient(%this.client, '', "\c6You were killed because the server thought you may have been cheating.");
	}

	dbg("Cheat hits on" SPC %this.client.name @ ":" @ %this.cheatHits, "cheat.cs", 3, true);
	%this.cheatHits++;
}

package FallingPlatformsCheatPackage {
	function GameConnection::spawnPlayer(%this) {
		%r = parent::spawnPlayer(%this);

		%this.player.cheatLoop = %this.player.schedule(300, cheatLoop);

		return %r;
	}

	// rip onplayertouch loooool fix your stupid game

	function fxDTSBrick::onAdd(%this) {
		%this.enableTouch = 1;
		return parent::onAdd(%this);
	}
};
activatePackage(FallingPlatformsCheatPackage);