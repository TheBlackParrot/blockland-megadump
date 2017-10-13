// left it as one file since this is a very simple gamemode

function initCTH() {
	if($JumpingSimulator::Seed $= "") {
		$JumpingSimulator::Seed = setMapSeed();
	}
	if($JumpingSimulator::CubeCount $= "") {
		startGeneratingCubes();
	}
}

function setMapSeed() {
	%chars = "0123456789";
	%timestamp = getUTC();
	%seed = "";

	for(%i=0;%i<strLen(%timestamp);%i+=2) {
		%index = mFloor(getSubStr(%timestamp, %i, 2)/10);
		%seed = %seed @ getSubStr(%chars, %index, 1);
	}

	talk("Set seed to" SPC %seed);
	setRandomSeed(%seed);
	return %seed;
}

function startGeneratingCubes() {
	$x = 0;
	$y = 0;
	$z = 50;
	$JumpingSimulator::CubeCount = 0;

	%list = nameToID("JumpingSimulatorLeaderboard");
	if(isObject(%list)) {
		%list.clear();
	} else {
		%list = new GuiTextListCtrl(JumpingSimulatorLeaderboard);
	}

	for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
		%client = $DefaultMinigame.member[%i];
		%client.highestTouched = 0;
		%client.lastTouched = 0;
	}

	setRandomSeed($JumpingSimulator::Seed);
	BrickGroup_888888.chainDeleteAll();
	waitForClearingCubes();
}

function waitForClearingCubes() {
	if(BrickGroup_888888.getCount() > 0) {
		schedule(200, 0, waitForClearingCubes);
		return;
	}

	generateSpawn();
	generateCubes(250);
}

function generateSpawn() {
	$x = 0;
	$y = 0;
	$z = 50;

	%brick = new fxDTSBrick(HeavenSpawn) {
		angleID = 0;
		colorFxID = 0;
		colorID = 0;
		dataBlock = "brick16x16FData";
		isBasePlate = 1;
		isPlanted = 1;
		position = "0 0 50";
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = 888888;
		enableTouch = 1;
	};

	%brick.setTrusted(1);
	%brick.plant();
	BrickGroup_888888.add(%brick);

	$y += nameToID("brick16x16FData").brickSizeY / 3;	
}

function generateCubes(%amount) {
	for(%i=0;%i<%amount;%i++) {
		%new_x = $x;
		%new_y = $y;
		%new_z = $z;

		%brick = $HeavenBrick[$JumpingSimulator::CubeCount++] = new fxDTSBrick(HeavenCube) {
			angleID = 0;
			colorFxID = 0;
			colorID = mFloor($JumpingSimulator::CubeCount / 250) % 32;
			dataBlock = "brick4xCubeData";
			isBasePlate = 1;
			isPlanted = 1;
			position = $x SPC $y SPC $z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			enableTouch = 1;
			idx = $JumpingSimulator::CubeCount;
		};

		%brick.plant();
		%brick.setTrusted(1);
		BrickGroup_888888.add(%brick);

		// i took the lazy way out im sorry pls dont kill
		%iterations = 0;

		%new_x = $x + getRandom(-58, 58)/10;
		%new_y = $y + getRandom(-58, 58)/10;
		%new_z = $z + getRandom(15, 30)/10;

		while(vectorLen(vectorSub($x SPC $y SPC $z, %new_x SPC %new_y SPC %new_z)) > 11.5) {
			%new_x = $x + getRandom(-58, 58)/10;
			%new_y = $y + getRandom(-58, 58)/10;
			%new_z = $z + getRandom(15, 30)/10;
		}

		initContainerRadiusSearch(%new_x SPC %new_y SPC %new_z, 5, $TypeMasks::FXBrickObjectType);
		while(isObject(%object = containerSearchNext()) && %iterations < 100) {
			%new_x = $x + getRandom(-58, 58)/10;
			%new_y = $y + getRandom(-58, 58)/10;
			%new_z = $z + getRandom(15, 30)/10;
			initContainerRadiusSearch(%new_x SPC %new_y SPC %new_z, 5, $TypeMasks::FXBrickObjectType);
			%iterations++;
		}

		$x = %new_x;
		$y = %new_y;
		$z = %new_z;
	}
}

function GameConnection::statsLoop(%this) {
	cancel(%this.statsSched);
	%this.statsSched = %this.schedule(1000, statsLoop);

	%font = "<font:Arial Black:24>";
	%str = %font @ "Current Height\c6:" SPC %this.lastTouched;
	%str = %str @ "<just:right>\c2Your Highest\c6:" SPC %this.highestTouched;
	%str = %str @ " ";
	%font = "<font:Courier New Bold:24>";
	%str = %str @ "<br><just:center>" @ %font @ "\c3" @ %this.timeString;
	%font = "<font:Arial Black:24>";
	%str = %str @ "<br>" @ %font @ "<just:center>\c3" @ %this.getLeaderboardPos();
	%this.bottomPrint(%str, 2, 1);
}

function GameConnection::timeLoop(%this) {
	cancel(%this.timeSched);
	%this.timeSched = %this.schedule(1000, timeLoop);

	%this.timeElapsed++;
	%this.timeString = "";

	if(%this.timeElapsed > 0) {
		%this.timeString = %this.timeElapsed % 60 @ "s";
	}
	if(%this.timeElapsed >= 60) {
		%this.timeString = mFloor(%this.timeElapsed / 60) % 60 @ "m" SPC %this.timeString;
	}
	if(%this.timeElapsed >= 3600) {
		%this.timeString = mFloor(%this.timeElapsed / 3600) % 24 @ "h" SPC %this.timeString;
	}
	if(%this.timeElapsed >= 86400) {
		%this.timeString = mFloor(%this.timeElapsed / 86400) @ "d" SPC %this.timeString;
	}
}

package JumpingSimulatorPackage {
	function fxDTSBrick::onPlayerTouch(%this, %player) {
		if(%this.idx !$= "") {
			if(%player.client.highestTouched < %this.idx) {
				%player.client.highestTouched = %this.idx;
				JumpingSimulatorLeaderboard.mod(%player.client, %this.idx);

				if(%this.idx > $JumpingSimulator::CubeCount - 250) {
					generateCubes(2);
				}
			}

			if(%player.client.lastTouched != %this.idx) {
				%this.playSound(brickRotateSound);
				%this.setColorFX(3);

				$HeavenBrick[%player.client.lastTouched+1].setColorFX(0);

				%player.client.cubesJumpedOn++;
			}

			%player.client.lastTouched = %this.idx;
			%player.client.score = %this.idx;

			%player.client.statsLoop();
		}

		return parent::onPlayerTouch(%this, %player);
	}

	function GameConnection::spawnPlayer(%this) {
		%r = parent::spawnPlayer(%this);

		%this.statsLoop();

		%player = %this.player;

		if(%this.highestTouched $= "") {
			%player.setTransform(vectorAdd(HeavenSpawn.getPosition(), "0 0 3"));
			return %r;
		}

		%warpTo = (%this.highestTouched - (%this.highestTouched % 250)) + 1;
		if(%warpTo <= 0) {
			%player.setTransform(vectorAdd(HeavenSpawn.getPosition(), "0 0 3"));
		} else {
			%player.setTransform(vectorAdd($HeavenBrick[%warpTo].getPosition(), "0 0 3"));
		}

		if(!isEventPending(%this.timeSched)) {
			%this.timeSched = %this.schedule(1000, timeLoop);
		}

		for(%i=0;%i<ClientGroup.getCount();%i++) {
			%client = ClientGroup.getObject(%i);

			if(%client.spectating == %this) {
				%client.camera.setOrbitMode(%this.player, %this.player.getTransform(), 0.5, 10, 20, false);
				%client.setControlObject(%client.camera);
				if(isObject(%client.player)) {
					%client.player.delete();
				}
			}
		}

		return %r;
	}

	function gameConnection::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea) {
		%lastTouched = $HeavenBrick[%this.lastTouched];
		if(isObject(%lastTouched)) {
			%lastTouched.setColorFX(0);
		}
		return parent::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea);
	}

	function GameConnection::onClientLeaveGame(%this) {
		%lastTouched = $HeavenBrick[%this.lastTouched+1];
		if(isObject(%lastTouched)) {
			%lastTouched.setColorFX(0);
		}
		%this.saveData();
		return parent::onClientLeaveGame(%this);
	}

	function GameConnection::autoAdminCheck(%this) {
		%this.loadData();
		%this.oldPrefix = %this.clanPrefix;
		initCTH();
		return parent::autoAdminCheck(%this);
	}

	function onServerDestroyed() {
		deleteVariables("$JumpingSimulator::*");
		parent::onServerDestroyed();
	}

	function serverCmdMessageSent(%client, %msg) {
		%colorID = mFloor(%client.highestTouched / 250) % 32;
		%colorStr = "<color:" @ _JS_RGBToHex(getColorIDTable(%colorID)) @ ">";

		%client.clanPrefix = "\c7[" @ %colorStr @ %client.highestTouched - (%client.highestTouched % 250) @ "\c7]" SPC %client.oldPrefix;
		return parent::serverCmdMessageSent(%client, %msg);
	}

	function SimObject::onCameraEnterOrbit(%obj, %camera) {}
	function SimObject::onCameraLeaveOrbit(%obj, %camera) {}

	function Observer::onTrigger(%this, %camera, %button, %state) {
		%client = %camera.getControllingClient();
		if(%state == 1) {
			%client.spectating = -1;
			%client.spawnPlayer();
		}
		return parent::onTrigger(%this, %camera, %button, %state);
	}
};
activatePackage(JumpingSimulatorPackage);

// ===================
// === LEADERBOARD ===
// ===================

function JumpingSimulatorLeaderboard::mod(%this, %client, %value) {
	%row_text = %client.name TAB %value;
	if(%this.getRowNumByID(%client.bl_id) == -1) {
		%this.addRow(%client.bl_id, %row_text);
	} else {
		%this.setRowByID(%client.bl_id, %row_text);
	}
	%this.sortNumerical(1, 0);
}

function getPositionString(%num) {
	if(strLen(%num)-2 >= 0) {
		%ident = getSubStr(%num,strLen(%num)-2,2);
	} else {
		%ident = %num;
	}
	if(%ident >= 10 && %ident < 20) {
		return %num @ "th";
	}

	%ident = getSubStr(%num,strLen(%num)-1,1);
	switch(%ident) {
		case 1:
			return %num @ "st";
		case 2:
			return %num @ "nd";
		case 3:
			return %num @ "rd";
		default:
			return %num @ "th";	
	}
}

function GameConnection::getLeaderboardPos(%this) {
	%list = JumpingSimulatorLeaderboard;
	%pos = %list.getRowNumByID(%this.bl_id);
	if(%pos == -1) {
		return "???";
	} else {
		return getPositionString(%pos+1);
	}
}

function serverCmdLeaderboard(%this) {
	%list = nameToID("JumpingSimulatorLeaderboard");
	if(!isObject(%list)) {
		return;
	}

	if(getSimTime() - %this.lastleadcmd <= 2000) {
		return;
	}
	%this.lastleadcmd = getSimTime();

	%count = %list.rowCount();
	if(%count > 15) {
		%count = 15;
	}

	for(%i=0;%i<%count;%i++) {
		%row = %list.getRowText(%i);
		while(getField(%row, 1) $= "") {
			if(%i >= %list.rowCount()) {
				break;
			}
			%list.removeRow(%i);
			%row = %list.getRowText(%i);
		}

		if(getField(%row, 1) $= "") {
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
		while(getField(%row, 1) $= "") {
			if(%i >= %list.rowCount()) {
				break;
			}
			%list.removeRow(%i);
			%row = %list.getRowText(%i);
		}

		if(getField(%row, 1) $= "") {
			continue;
		}

		if(%pos == %i) {
			messageClient(%this, '', "\c3" @ %i+1 @ ". \c2" @ getField(%row, 0) SPC "-\c5" SPC getField(%row, 1));
		} else {
			messageClient(%this, '', "\c3" @ %i+1 @ ". \c6" @ getField(%row, 0) SPC "-\c5" SPC getField(%row, 1));
		}
	}
}

// ======================
// === SAVING/LOADING ===
// ======================

$JumpingSimulator::SaveDir = "config/server/JumpingSimulator/saves";
function GameConnection::saveData(%this) {
	%file = new FileObject();
	%filename = $JumpingSimulator::SaveDir @ "/" @ %this.bl_id;
	%file.openForWrite(%filename);

	%file.writeLine(getDateTime());
	%file.writeLine($JumpingSimulator::Seed);
	%file.writeLine(%this.highestTouched);
	%file.writeLine(%this.timeElapsed);
	%file.writeLine(%this.cubesJumpedOn);
	%file.writeLine(%this.name);

	%file.close();
	%file.delete();
}

function GameConnection::loadData(%this) {
	%file = new FileObject();
	%filename = $JumpingSimulator::SaveDir @ "/" @ %this.bl_id;
	if(!isFile(%filename)) {
		%this.highestTouched = 0;
		%this.timeElapsed = 0;
		%this.cubesJumpedOn = 0;
		return;
	}

	%file.openForRead(%filename);

	%date = %file.readLine();
	%seed = %file.readLine();
	if(%seed $= $JumpingSimulator::Seed) {
		%this.highestTouched = %file.readLine();
	} else {
		%file.readLine(); // old data, garbage
		%this.highestTouched = 0;
	}
	%this.timeElapsed = %file.readLine();
	%this.cubesJumpedOn = %file.readLine();
	%name = %file.readLine(); // garbage for now

	JumpingSimulatorLeaderboard.mod(%this, %this.highestTouched);

	%file.close();
	%file.delete();
}

// ================
// === COMMANDS ===
// ================
function serverCmdSeed(%client) {
	messageClient(%client, '', "\c6" @ $JumpingSimulator::Seed);
}

function serverCmdSpectate(%client, %who) {
	if(getSimTime() - %client.lastspectatecmd <= 3000) {
		return;
	}
	%client.lastspectatecmd = getSimTime();

	%target = findClientByName(%who);
	if(!isObject(%target)) {
		%target = findClientByBL_ID(%who);
		if(!isObject(%target)) {
			messageClient(%client, '', "This player does not exist.");
			return;
		}
	}

	if(%client == %target) {
		messageClient(%client, '', "You cannot spectate yourself.");
		return;
	}

	messageClient(%client, '', "\c6You are now spectating\c4" SPC %target.name @ "\c6. Activate to leave spectator mode.");
	messageClient(%target, '', "\c4" @ %client.name SPC "\c6is now spectating you.");

	if(isObject(%target.player)) {
		%client.camera.setOrbitMode(%target.player, %target.player.getTransform(), 0.5, 10, 20, false);
		%client.setControlObject(%client.camera);
		if(isObject(%client.player)) {
			%client.player.delete();
		}
	}

	%client.spectating = %target;
}

// =====================
// === MISCELLANEOUS ===
// =====================
function _JS_RGBToHex(%rgb) {
	%rgb = getWords(%rgb,0,2);
	for(%i=0;%i<getWordCount(%rgb);%i++) {
		%dec = mFloor(getWord(%rgb,%i)*255);
		%str = "0123456789ABCDEF";
		%hex = "";

		while(%dec != 0) {
			%hexn = %dec % 16;
			%dec = mFloor(%dec / 16);
			%hex = getSubStr(%str,%hexn,1) @ %hex;    
		}

		if(strLen(%hex) == 1)
			%hex = "0" @ %hex;
		if(!strLen(%hex))
			%hex = "00";

		%hexstr = %hexstr @ %hex;
	}

	if(%hexstr $= "") {
		%hexstr = "FF00FF";
	}
	return %hexstr;
}