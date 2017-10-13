function Player::checkForCheats(%this) {
	cancel(%this.cheatLoop);
	%this.cheatLoop = %this.schedule(1500, checkForCheats);

	if(!$CrumblingArena::Active) {
		return;
	}

	if(%this.getTransform() $= %this.oldTransform) {
		%this.kill();
	}
	%this.oldTransform = %this.getTransform();
}