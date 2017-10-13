exec("./support.cs");
exec("./build.cs");
exec("./projectile.cs");
exec("./commands.cs");
exec("./saving.cs");

$Pref::Take::VerticalOffset = 0;
$Pref::Take::DefaultColor = 0;
$Pref::Take::PlayAreaSize = 375;
$Pref::Take::PlayAreaHeight = 175;

$Take::Version = "v0.1.9-2";

datablock AudioProfile(takeJumpSound:combo1) { filename = "./sounds/jump.wav"; };
datablock AudioProfile(floathum:combo1) {
	filename = "./sounds/float_hum.wav";
	description = AudioClosestLooping3d;
};

PlayerStandardArmor.airControl = 1;
PlayerStandardArmor.horizMaxSpeed = 272;
PlayerStandardArmor.jumpForce = 1024;
PlayerStandardArmor.maxBackwardSpeed = 32;
PlayerStandardArmor.maxForwardSpeed = 72;
PlayerStandardArmor.maxJumpSpeed = 100;
PlayerStandardArmor.runForce = 5000;
PlayerStandardArmor.drag = 0.1;
PlayerStandardArmor.upMaxSpeed = 160;
PlayerStandardArmor.mass = 25;
PlayerStandardArmor.jumpDelay = 0;
PlayerStandardArmor.jumpSound = takeJumpSound;

function GameConnection::getPercentageValue(%this,%which) {
	if(%which $= "") {
		%which = "players";
	}
	switch$(%which) {
		case "players":
			for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
				%client = $DefaultMinigame.member[%i];
				%total += %client.amountHas;
			}
			return (%this.amountHas/%total)*100;
		case "all":
			return (%this.amountHas/BrickGroup_888888.getCount())*100;
	}
}

function getClientFromRank(%rank_v) {
	for(%j=0;%j<$DefaultMinigame.numMembers;%j++) {
		%highest[value] = -1;
		for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
			%client = $DefaultMinigame.member[%i];
			if(%client.amountHas > %highest[value] && !%selected[%client]) {
				%highest[value] = %client.amountHas;
				%highest[client] = %client;
				%rank[%client] = %j+1;
				%r_client[%rank[%client]] = %client;
			}
		}
		%selected[%highest[client]] = 1;
	}
	return %r_client[%rank_v];
}

// bring this function over to Falling Platforms, holy crap
function GameConnection::getRank(%this) {
	for(%j=0;%j<$DefaultMinigame.numMembers;%j++) {
		%highest[value] = -1;
		for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
			%client = $DefaultMinigame.member[%i];
			if(%client.amountHas > %highest[value] && !%selected[%client]) {
				%highest[value] = %client.amountHas;
				%highest[client] = %client;
				%rank[%client] = %j+1;
			}
		}
		%selected[%highest[client]] = 1;
	}
	return %rank[%this];
}

// for things that need to be called only every so often
function GameConnection::doStatValuesLoop(%this) {
	cancel(%this.statsLoop2);
	%this.statsLoop2 = %this.schedule(1000,doStatValuesLoop);

	%this.perc[1] = mFloor(%this.getPercentageValue("players")*10)/10;
	if(%this.perc[1] < 0) {
		%this.perc[1] = 0;
	}
}

function GameConnection::doBottomStats(%this) {
	cancel(%this.statsLoop);
	%this.statsLoop = %this.schedule(100,doBottomStats);

	%color = "<color:" @ RGBToHex(getColorIDTable(%this.color)) @ ">";
	%rank = %this.getRank();
	if(%rank != %this.oldRank && %this.oldRank !$= "") {
		messageClient(%this,'',"\c6You are now\c3" SPC getPositionString(%rank));
		%other = getClientFromRank(%this.oldRank);
		messageClient(%this,'',"<color:" @ RGBToHex(getColorIDTable(%other.color)) @ ">" @ %other.name SPC "\c6is now\c3" SPC getPositionString(%this.oldRank));
	}
	%shadow = "<shadow:3:3><shadowcolor:" @ RGBToHex(getColorIDTable(%this.color)) @ ">";
	%time = getSubStr(getTimeString(mFloor(($DefaultMinigame.endRoundAt - getSimTime())/100)/10),0,6);
	%time = %time @ getSubStr("0:00.0",strLen(%time),6-strLen(%time));
	if(stripos(%time,"-") > -1) {
		%time = "0:00.0";
	}

	if(isObject(%this.projectile)) {
		%projtime = mFloatLength((getSimTime() - %this.firedAt)/1000,1);
		%vel = mFloatLength(vectorLen(%this.projectile.getVelocity()),1);
		%projinfo = "\c7" @ %projtime @ "  \c3" @ %vel @ "tu  \c0" @ (%this.projectile.combo | 0) @ "x";
	} else {
		%projinfo = "\c2" @ (%this.projectile.combo | 0) @ "x";
	}
	%this.bottomPrint("<font:Arial Bold:48>" @ %color @ getPositionString(%rank) @ "    <font:Arial Bold:32><color:ffffff>" @ %shadow @ %this.amountHas @ "<shadow:0:0><font:Arial Bold:24>" @ %color @ " [" @ %this.perc[1] @ "%]<just:right><font:Arial Bold:18>" @ %projinfo @ "    <font:Arial:14>\c6" SPC %time @ " ",3,1);

	%this.oldRank = %this.getRank();
}

function MinigameSO::showRoundStats(%this) {
	%count = %this.numMembers;
	if(%count > 3) {
		%count = 3;
	}
	%str = "<font:Arial Bold:48><just:center>\c3ROUND" SPC %this.rounds SPC "FINISHED<br><font:Arial:20>";
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		// clever code
		%rank_v = %client.getRank();
		%rank[%rank_v] = %client;
	}
	for(%i=1;%i<=%count;%i++) {
		%str = %str @ "<just:left>\c2#" @ %i @ ".\c6" SPC %rank[%i].name @ "<just:right>\c3" @ %rank[%i].amountHas SPC "\c7[" @ mFloatLength(%rank[%i].getPercentageValue("players"),1) @ "%]";
	}
	if(%this.numMembers > 3) {
		messageAll('',"-- OTHER POSITIONS --");
		for(%i=4;%i<%this.numMembers+1;%i++) {
			%this.messageAll('',"<just:left>\c2#" @ %i @ ".\c6" SPC %rank[%i].name @ "\c7 -- \c3" @ %rank[%i].amountHas SPC "\c7[" @ mFloatLength(%rank[%i].getPercentageValue("players"),1) @ "%]");
		}
	}
	%this.centerPrintAll(%str,10);
}

function MinigameSO::endRound(%this) {
	%highest[client] = getClientFromRank(1);
	%highest[val] = %highest[client].amountHas;
	%highest[client].wins++;

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.projectile)) {
			%client.projectile.explode();
		}
		if(%client != %highest[client]) {
			%client.losses++;
		}
	}

	for(%i=0;%i<BrickochetShrapnelSet.getCount();%i++) {
		%row = BrickochetShrapnelSet.getObject(%i);
		if(isObject(%row.projectile)) {
			%row.projectile.delete();
		}
	}
	BrickochetShrapnelSet.clear();

	%this.allowProjectiles = 0;
	if(%this.numMembers > 0) {
		%this.resetSchedule = %this.schedule(10000,reset);
	}

	%this.showRoundStats();

	cancel(%this.bombAddLoop);

	%color = "<color:" @ RGBToHex(getColorIDTable(%highest[client].color)) @ ">";
	%this.messageAll('',%color @ %highest[client].name SPC "\c6wins this round, owning\c3" SPC mFloor(%highest[client].getPercentageValue("players")*10)/10 @ "% \c6of the board. They have won\c3" SPC %highest[client].wins SPC "time(s).");
	%this.messageAll('',"\c5Resetting in \c310 seconds...");
}

function MinigameSO::addBombs(%this,%override) {
	cancel(%this.bombAddLoop);
	%this.bombAddLoop = %this.schedule(45000,addBombs);

	%amount = %override | mCeil(%this.numMembers/4) * 2;
	%this.messageAll('',"\c3" @ %amount SPC "more bombs\c6 have spawned!");
	spawnRandomCubes(%amount,1);
}

function Player::checkInsideBrick(%this) {
	if(!%this.client.enableFloat) {
		return;
	}

	cancel(%this.brickInLoop);
	%this.brickInLoop = %this.schedule(70,checkInsideBrick);

	initContainerBoxSearch(%this.getPosition(),"0.1 0.1 0.1",$TypeMasks::FXBrickObjectType);
	if((%targetObject = containerSearchNext()) != 0 && isObject(%targetObject)) {
		%this.addVelocity(vectorScale("0 0 1", (%this.getDatablock().jumpForce / %this.getDatablock().mass)/4));
		if(!%this.playingFloat) {
			%this.playingFloat = 1;
			%this.playAudio(0,floathum);
		}
		return;
	} else {
		if(%this.playingFloat) {
			%this.stopAudio(0);
			%this.playingFloat = 0;
		}
	}
}

package TakeGamePackage {
	function GameConnection::autoAdminCheck(%this) {
		for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
			%client = $DefaultMinigame.member[%i];
			// adding C makes things a lot easier later
			%taken_colors = "C" @ %client.color SPC %taken_colors;
		}
		%color = getRandom(1,63);
		while(stripos(%taken_colors,"C" @ %color) != -1) {
			%color = getRandom(1,63);
		}
		%this.color = %color;

		%this.loadTakeGame();

		messageClient(%this,'',"\c4HOW TO PLAY: \c6Left click to fire lasers that bounce off of cubes in the arena. Whoever owns the most after 10 minutes wins the round. See /help for more info.");
		return parent::autoAdminCheck(%this);
	}

	function GameConnection::spawnPlayer(%this) {
		parent::spawnPlayer(%this);
		%this.player.setShapeNameColor(getColorIDTable(%this.color));
		%this.player.setShapeNameDistance(2000);
		%this.player.setTransform(getRandom($Pref::Take::PlayAreaSize/4,$Pref::Take::PlayAreaSize/2) SPC getRandom($Pref::Take::PlayAreaSize/4,$Pref::Take::PlayAreaSize/2) SPC 3);
		%this.player.setPlayerScale("0.25 0.25 0.25");
		%this.player.checkInsideBrick();
		if(!isEventPending(%this.statsLoop)) {
			%this.doBottomStats();
			%this.doStatValuesLoop();
		}
		$DefaultMinigame.checkLastManStanding();
	}

	function serverCmdMessageSent(%this,%msg) {
		%color = "<color:" @ RGBToHex(getColorIDTable(%this.color)) @ ">";
		%this.clanPrefix = %color @ %this.clanSymbol SPC "\c7" @ strReplace(%this.originalPrefix,"%NULL%","");
		return parent::serverCmdMessageSent(%this,%msg);
	}

	function Player::activateStuff(%this) {
		//Player::spawnProjectile(%speed,%data,%spread,%scale,%client)
		if(isObject(%this.client.projectile)) {
			messageClient(%this.client,'',"You already have a trail going!");
			%this.client.playSound(errorSound);
			return parent::activateStuff(%this);
		}
		if(!%this.client.minigame.allowProjectiles) {
			return parent::activateStuff(%this);
		}
		%this.spawnProjectile(200,"takeGameProjProjectile" @ %this.client.color,0,1,%this.client);
		%this.client.firedAt = getSimTime();

		serverPlay3D(takeProjFire,%this.getPosition());
		return parent::activateStuff(%this);
	}

	function armor::onTrigger(%db,%obj,%slot,%val) {
		if(%obj.getClassName() $= "Player") {
			if(!isObject(%obj.client.minigame)) {
				return;
			}
			if(getWord(%obj.getPosition(),2) < 0) {
				%obj.setVelocity("0 0 5");
				%obj.setTransform(getWords(%obj.getPosition(),0,1) SPC 0);
				return;
			}
			if(%val == 1 && %slot == 2) {
				initContainerBoxSearch(%obj.getPosition(),"5 5 5",$TypeMasks::FXBrickObjectType);
				if((%targetObject = containerSearchNext()) != 0 && isObject(%targetObject)) {
					serverPlay3D(%obj.getDatablock().JumpSound,%obj.getPosition());
					%obj.addVelocity(vectorScale("0 0 1", (%obj.getDatablock().jumpForce / %obj.getDatablock().mass)/2));
				}
			}
		}

		return Parent::onTrigger(%db,%obj,%slot,%val);
	}

	function MinigameSO::reset(%this) {
		while(isEventPending(%this.resetSchedule)) {
			cancel(%this.resetSchedule);
		}
		while(isEventPending(%this.endSchedule)) {
			cancel(%this.endSchedule);
		}
		if(!%this.loading) {
			%this.loading = 1;
			loadPhase1();
		} else {
			%this.loading = 0;
			%this.endRoundAt = getSimTime() + 600000;
			%this.endSchedule = %this.schedule(600000,endRound);
			%this.addBombs();
			%this.rounds++;
			messageAll('',"\c5Beginning \c3round" SPC %this.rounds);
			for(%i=0;%i<%this.numMembers;%i++) {
				%this.member[%i].amountHas = 0;
			}
			%this.respawnAll();
			%this.allowProjectiles = 1;
		}
	}

	function MinigameSO::checkLastManStanding(%this) {
		%reset = 1;
		for(%i=0;%i<%this.numMembers;%i++) {
			%reset = 0;
		}
		if(BrickGroup_888888.getCount() <= 0 && !%this.loading) {
			%reset = 1;
		}
		if(%this.endRoundAt - getSimTime() < -15000) {
			%reset = 1;
		}

		if(%reset) {
			%this.reset();
		}
	}

	function GameConnection::onClientLeaveGame(%this) {
		for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
			%brick = BrickGroup_888888.getObject(%i);
			if(%brick.takenBy == %this) {
				%brick.takenBy = "";
				%brick.setColor($Pref::Take::DefaultColor);
			}
		}
		return parent::onClientLeaveGame(%this);
	}

	function serverCmdMessageSent(%this,%msg) {
		%color = "<color:" @ RGBToHex(getColorIDTable(%this.color)) @ ">";
		%rank = getPositionString(%this.getRank());
		if(!%rank) {
			return parent::serverCmdMessageSent(%this,%msg);
		}

		%temp_pre = %this.clanPrefix;
		%this.clanPrefix = "\c7[" @ %color @ %rank @ "\c7]" SPC %this.clanPrefix;
		parent::serverCmdMessageSent(%this,%msg);
		%this.clanPrefix = %temp_pre;
	}

	function onServerDestroyed() {
		deleteVariables("$Take::*");
		cancel($loadPhase2Loop);
		return parent::onServerDestroyed();
	}

	function serverCmdLight(){}

	function fxDTSBrick::onAdd(%this) {
		if(%this.shapeFxID) {
			%this.blinkBrick();
		}
		return parent::onAdd(%this);
	}

	function fxDTSBrick::blinkBrick(%this) {
		cancel(%this.blinkBrickLoop);
		%this.blinkBrickLoop = %this.schedule(300,blinkBrick);

		switch(%this.colorID) {
			case 0:
				%this.setColor(1);
				return;
			case 1:
				%this.setColor(0);
		}
	}
};
activatePackage(TakeGamePackage);

function loadPhase1() {
	BrickGroup_888888.chainDeleteAll();
	loadPhase2();
}

function loadPhase2() {
	cancel($loadPhase2Loop);
	if(BrickGroup_888888.getCount() > 0) {
		$loadPhase2Loop = schedule(100,0,loadPhase2);
		return;
	}

	spawnRandomCubes(getRandom(2048,5632));
	$DefaultMinigame.reset();
}