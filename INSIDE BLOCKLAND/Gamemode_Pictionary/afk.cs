$Pictionary::MaxTimeAFK = 60;

function Player::resetAFK(%this) {
	%client = %this.client;
	if(%client.afk) {
		dbg("No longer considering BL_ID" SPC %client.bl_id SPC "as AFK.", "afk.cs");
		messageClient(%client, '', "\c6You are no longer AFK.");
		%client.afk = false;
	}

	%this.afkCount = 0;
}

function Player::checkAFK(%this) {
	%client = %this.client;

	if(%this.afkCount > $Pictionary::MaxTimeAFK && !%client.afk) {
		dbg("Considering BL_ID" SPC %client.bl_id SPC "as AFK.", "afk.cs");
		messageClient(%client, '', "\c6You are now considered AFK.");
		%client.afk = true;
	}

	if(%client.canDraw && $Pictionary::CanGuess && %client.afk) {
		messageAll('', "\c4The round has ended early because the drawer is AFK.");
		$DefaultMinigame.endRound();
	}
}

function Player::startAFKDetection(%this) {
	cancel(%this.afkLoop);
	%this.afkLoop = %this.schedule(1000, startAFKDetection);

	%client = %this.client;

	%new = %this.getTransform();
	if(%new $= %this.oldTransform) {
		%this.afkCount++;
	} else {
		%this.resetAFK();
	}
	%this.oldTransform = %new;

	dbg("BL_ID " SPC %client.bl_id SPC "afkCount:" SPC %this.afkCount, "afk.cs", 3);

	%this.checkAFK();
}

package PictionaryAFKPackage {
	function GameConnection::spawnPlayer(%this) {
		parent::spawnPlayer(%this);

		%player = %this.player;
		%player.startAFKDetection();
	}
};
activatePackage(PictionaryAFKPackage);