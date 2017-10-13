exec("./sounds.cs");

$Pref::Server::Minesweeper::MaxWidth = 64;
$Pref::Server::Minesweeper::Maxlength = 64;

function initMinesweeper() {
	$Minesweeper::GridPrints[-1] = $printNameTable["1x1/Minesweeper1x1GridButton"];
	for(%i=0;%i<9;%i++) {
		$Minesweeper::GridPrints[%i] = $printNameTable["1x1/Minesweeper1x1Grid" @ %i];
	}

	$Minesweeper::Slots = 0;
}
if($Minesweeper::Init $= "") {
	$Minesweeper::Init = true;
	initMinesweeper();
}

function generateMines(%width, %length, %mines, %slot) {
	deleteVariables("$Minesweeper::IsMine" @ %slot @ "*");
	$Minesweeper::NewGame[%slot] = true;
	$Minesweeper::MaxX[%slot] = %width;
	$Minesweeper::MaxY[%slot] = %length;
	$Minesweeper::Mines[%slot] = %mines;

	for(%x=0;%x<%width;%x++) {
		for(%y=0;%y<%length;%y++) {
			$Minesweeper::IsMine[%slot, %x, %y] = false;
		}
	}

	for(%i=0;%i<%mines;%i++) {
		%x = getRandom(0, %width-1);
		%y = getRandom(0, %length-1);

		while($Minesweeper::IsMine[%slot, %x, %y]) {
			%x = getRandom(0, %width-1);
			%y = getRandom(0, %length-1);
		}

		$Minesweeper::IsMine[%slot, %x, %y] = true;
	}

	for(%x=0;%x<%width;%x++) {
		for(%y=0;%y<%length;%y++) {
			%brick = nameToID("MinesweeperBrick" @ %slot @ "_" @ %x @ "_" @ %y);
			%brick.surroundingMines = 0;

			for(%xx=%x-1;%xx<=%x+1;%xx++) {
				for(%yy=%y-1;%yy<=%y+1;%yy++) {
					%brick.surroundingMines += $Minesweeper::IsMine[%slot, %xx, %yy];
				}
			}
		}
	}

	$Minesweeper::StartedAt[%slot] = $Sim::Time;
}

function drawMinesweeper(%width, %length, %mines, %slot, %client) {
	if(%width < 8) { %width = 8; }
	if(%length < 8) { %length = 8; }

	if(%width > $Pref::Server::Minesweeper::MaxWidth) {
		%width = $Pref::Server::Minesweeper::MaxWidth;
	}

	if(%length > $Pref::Server::Minesweeper::Maxlength) {
		%length = $Pref::Server::Minesweeper::Maxlength;
	}

	%total = %width * %length;
	
	if(%mines / %total > 0.5) {
		%mines = mFloor(%total / 2);
	}

	if(%mines < 8) {
		%mines = 8;
	}

	%offset = 1000 + (%slot * 50);

	%group = "BrickGroup_Minesweeper" @ %slot;
	%group = nameToID(%group);
	if(!isObject(%group)) {
		%group = new SimGroup(("BrickGroup_Minesweeper" @ %slot) : BrickGroup_888888) {
			bl_id = 888887;
			name = "\c1Minesweeper\c0";
		};
		MainBrickgroup.add(%group);
	}

	$Minesweeper::MaxX[%slot] = %width;
	$Minesweeper::MaxY[%slot] = %length;
	$Minesweeper::Mines[%slot] = %mines;

	for(%x=0;%x<%width;%x++) {
		for(%y=0;%y<%length;%y++) {
			$Minesweeper::IsMine[%slot, %x, %y] = false;

			%brick = new fxDTSBrick("MinesweeperBrick" @ %slot @ "_" @ %x @ "_" @ %y) {
				angleID = 0;
				colorFxID = 3;
				colorID = 0;
				printID = $Minesweeper::GridPrints[-1];
				dataBlock = "brick1x1FPrintData";
				isBasePlate = 0;
				isPlanted = 1;
				position = vectorAdd(%loadOffset, (brick1x1PrintData.brickSizeX / 2)*%y SPC (brick1x1PrintData.brickSizeY / 2)*%x SPC %offset);
				printID = 0;
				scale = "1 1 1";
				shapeFxID = 0;
				stackBL_ID = 888887;
				enableTouch = 0;
				slot = %slot;
				minesweeper = true;
				uncovered = false;
				slot_owner = %client;
				x = %x;
				y = %y;
				flagged = false;
			};
			%brick.setTrusted(1);
			%brick.plant();
			%brick.setPrint($Minesweeper::GridPrints[-1]);
			%group.add(%brick);
		}
	}

	for(%x=-4;%x<%width+4;%x++) {
		for(%y=-4;%y<%length+4;%y++) {
			if((%x >= %width || %y >= %length) || %x < 0 || %y < 0) {
				%brick = new fxDTSBrick("MinesweeperBorder") {
					angleID = 0;
					colorFxID = 0;
					colorID = %slot % 9;
					dataBlock = "brick1x1Data";
					isBasePlate = 0;
					isPlanted = 1;
					position = vectorAdd(%loadOffset, (brick1x1PrintData.brickSizeX / 2)*%y SPC (brick1x1PrintData.brickSizeY / 2)*%x SPC %offset);
					printID = 0;
					scale = "1 1 1";
					shapeFxID = 0;
					stackBL_ID = 888887;
					enableTouch = 0;
					minesweeper = false;
				};
				%brick.setTrusted(1);
				%brick.plant();
				%group.add(%brick);
			}
		}
	}

	generateMines(%width, %length, %mines, %slot);

	if(%slot == $Minesweeper::Slots || $Minesweeper::Slots $= "") {
		$Minesweeper::Slots++;
	}
}

function showMines(%slot) {
	for(%x=0;%x<$Minesweeper::MaxX[%slot];%x++) {
		for(%y=0;%y<$Minesweeper::MaxY[%slot];%y++) {
			%brick = nameToID("MinesweeperBrick" @ %slot @ "_" @ %x @ "_" @ %y);

			if($Minesweeper::IsMine[%slot, %x, %y]) {
				if(!%brick.flagged) {
					%brick.setPrint($printNameTable["1x1/Minesweeper1x1GridMine"]);
				}
			} else {
				if(%brick.flagged) {
					%brick.disappear(-1);
				}
			}
		}
	}
}

function fxDTSBrick::MS_sweep(%this, %client) {
	if(!%client.allowedToHelp[%this.slot]) {
		if(%this.slot != %client.minesweeperSlot) {
			%client.play2D(errorSound);
			return;
		}
	}

	if(%this.slot_owner.win) {
		return;
	}

	if(%this.uncovered) {
		return;
	}

	if(%this.flagged) {
		return;
	}

	if($Minesweeper::IsMine[%this.slot, %this.x, %this.y]) {
		if($Minesweeper::NewGame[%this.slot]) {
			generateMines($Minesweeper::MaxX[%this.slot], $Minesweeper::MaxY[%this.slot], $Minesweeper::Mines[%this.slot], %this.slot);
			%this.schedule(10, MS_sweep, %client);
			talk("regen mines");
			return;
		} else {
			showMines(%this.slot);
			%this.slot_owner.win = true;
			%this.playSound(MinesweeperExplode);			
		}
		return;
	}
	$Minesweeper::NewGame[%this.slot] = false;

	%this.uncovered = true;

	for(%x=%this.x-1;%x<=%this.x+1;%x++) {
		for(%y=%this.y-1;%y<=%this.y+1;%y++) {
			%surround_brick = "MinesweeperBrick";
			%surround_brick = nameToID(%surround_brick @ %this.slot @ "_" @ %x @ "_" @ %y);
			if(!isObject(%surround_brick) || %x > $Minesweeper::MaxX[%this.slot] || %y > $Minesweeper::MaxY[%this.slot] || %x < 0 || %y < 0) {
				continue;
			}

			if(!$Minesweeper::IsMine[%this.slot, %x, %y]) {
				if(%this.surroundingMines == 0) {
					%surround_brick.MS_sweep(%client);
				}
				%this.setPrint($Minesweeper::GridPrints[%this.surroundingMines]);
			}
		}
	}
}

function serverCmdMinesweeper(%client, %width, %length, %mines) {
	if(getSimTime() - %client.lastcmd["minesweeper"] < 3000) {
		return;
	}
	%client.lastcmd["minesweeper"] = getSimTime();

	if(%client.doingMinesweeper) {
		messageClient(%client, '', "\c6Use the \c4/stopMinesweeper \c6or \c4/stopm \c6commands to end your game.");
		return;
	}

	if(%width $= "") { %width = 32; }
	if(%length $= "") { %length = 32; }
	if(%mines $= "") { %mines = 128; }

	if(%width < 8) { %width = 8; }
	if(%length < 8) { %length = 8; }

	%total = %width * %length;

	if(%mines / %total > 0.66) {
		%mines = mFloor(%total / 1.5);
	}

	if(%mines < 8) {
		%mines = 8;
	}

	%client.doingMinesweeper = true;

	for(%i=0;%i<$Minesweeper::Slots;%i++) {
		%group = "BrickGroup_Minesweeper" @ %i;
		if(%group.getCount() == 0) {
			%slot = %i;
			break;
		}
	}

	if(%slot $= "") {
		%slot = $Minesweeper::Slots;
	}
	%client.minesweeperSlot = %slot;
	%client.placedFlags = 0;
	%client.win = false;

	drawMinesweeper(%width, %length, %mines, %slot, %client);

	%client.schedule(1, instantRespawn);

	%client.minesweeperFlagsLeft = %client.minesweeperFlags = %mines;
}

function serverCmdStopMinesweeper(%client) {
	if(getSimTime() - %client.lastcmd["stopminesweeper"] < 3000) {
		return;
	}
	%client.lastcmd["stopminesweeper"] = getSimTime();

	if(!%client.doingMinesweeper) {
		return;
	}

	if(%client.minesweeperSlot $= "") {
		return;
	}

	for(%i=0;%i<ClientGroup.getCount();%i++) {
		%t = ClientGroup.getObject(%i);
		if(%t.allowedToHelp[%client.minesweeperSlot]) {
			%t.allowedToHelp[%client.minesweeperSlot] = false;
		}
	}

	%group = nameToID("BrickGroup_Minesweeper" @ %client.minesweeperSlot);
	%group.chainDeleteAll();

	%client.doingMinesweeper = false;
	%client.minesweeperSlot = "";

	%client.instantRespawn();
}

function serverCmdResetMinesweeper(%client) {
	if(getSimTime() - %client.lastcmd["resetminesweeper"] < 5000) {
		return;
	}
	%client.lastcmd["resetminesweeper"] = getSimTime();

	if(!%client.doingMinesweeper) {
		return;
	}

	if(%client.minesweeperSlot $= "") {
		return;
	}

	%group = nameToID("BrickGroup_Minesweeper" @ %client.minesweeperSlot);

	for(%i=0;%i<%group.getCount();%i++) {
		%brick = %group.getObject(%i);
		%brick.setPrint($Minesweeper::GridPrints[-1]);
		%brick.flagged = false;
		%brick.uncovered = false;
		%brick.disappear(0);
		%brick.surroundingMines = 0;
	}

	%client.minesweeperFlagsLeft = %client.minesweeperFlags;
	%client.placedFlags = 0;
	%client.win = false;

	deleteVariables("$Minesweeper::IsMine" @ %client.minesweeperSlot @ "*");
	generateMines($Minesweeper::MaxX[%client.minesweeperSlot], $Minesweeper::MaxY[%client.minesweeperSlot], $Minesweeper::Mines[%client.minesweeperSlot], %client.minesweeperSlot);
}

function serverCmdAddMinesweeperPlayer(%client, %who) {
	if(getSimTime() - %client.lastcmd["addminesweeperplayer"] < 500) {
		return;
	}
	%client.lastcmd["addminesweeperplayer"] = getSimTime();

	if(!%client.doingMinesweeper) {
		return;
	}

	if(%client.minesweeperSlot $= "") {
		return;
	}

	%target = findClientByName(%who);
	if(!isObject(%target)) {
		%target = findClientByBL_ID(%who);
		if(!isObject(%target)) {
			%client.play2D(errorSound);
			messageClient(%client, '', "This player does not exist.");
			return;
		}
	}

	%target.allowedToHelp[%client.minesweeperSlot] = true;
	messageClient(%client, '', "\c6You have allowed\c4" SPC %target.name SPC "\c6to assist you in your Minesweeper game.");
	messageClient(%target, '', "\c4" @ %client.name SPC "\c6has allowed you to assist them in their Minesweeper game. Use \c4/tpm" SPC %client.minesweeperSlot SPC "\c6to join them.");
}

function serverCmdRemoveMinesweeperPlayer(%client, %who) {
	if(getSimTime() - %client.lastcmd["removeminesweeperplayer"] < 500) {
		return;
	}
	%client.lastcmd["removeminesweeperplayer"] = getSimTime();

	if(!%client.doingMinesweeper) {
		return;
	}

	if(%client.minesweeperSlot $= "") {
		return;
	}

	%target = findClientByName(%who);
	if(!isObject(%target)) {
		%target = findClientByBL_ID(%who);
		if(!isObject(%target)) {
			%client.play2D(errorSound);
			messageClient(%client, '', "This player does not exist.");
			return;
		}
	}

	if(!%target.allowedToHelp[%client.minesweeperSlot]) {
		return;
	}

	%target.allowedToHelp[%client.minesweeperSlot] = false;
	messageClient(%client, '', "\c4" @ %target.name SPC "\c6can no longer assist you in your Minesweeper game.");
	messageClient(%target, '', "\c4" @ %client.name SPC "\c6has no longer allowed you to assist them in their Minesweeper game.");	
}

function serverCmdTPM(%client, %slot) {
	if(%slot $= "" && %client.doingMinesweeper) {
		%slot = %client.minesweeperSlot;
	}

	if(%client.allowedToHelp[%slot] || (%slot == %client.minesweeperSlot && %client.doingMinesweeper)) {
		if(isObject(%client.player)) {
			%group = nameToID("BrickGroup_Minesweeper" @ %slot);
			%client.player.setTransform(vectorAdd(%group.getObject(getRandom(0, %group.getCount()-1)).getPosition(), "0 0 5"));
		}
	}
}

function serverCmdMines(%client, %width, %length, %mines) { serverCmdMinesweeper(%client, %width, %length, %mines); }
function serverCmdStopM(%client) { serverCmdStopMinesweeper(%client); }
function serverCmdMStop(%client) { serverCmdStopMinesweeper(%client); }
function serverCmdStopMines(%client) { serverCmdStopMinesweeper(%client); }
function serverCmdMinesStop(%client) { serverCmdStopMinesweeper(%client); }
function serverCmdResetM(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdMReset(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdResetMines(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdMinesReset(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdRestartM(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdMRestart(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdRestartMines(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdMinesRestart(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdRestartMinesweeper(%client) { serverCmdResetMinesweeper(%client); }
function serverCmdAMP(%client, %who) { serverCmdAddMinesweeperPlayer(%client, %who); }
function serverCmdMInvite(%client, %who) { serverCmdAddMinesweeperPlayer(%client, %who); }
function serverCmdMinesInvite(%client, %who) { serverCmdAddMinesweeperPlayer(%client, %who); }
function serverCmdInviteM(%client, %who) { serverCmdAddMinesweeperPlayer(%client, %who); }
function serverCmdRMP(%client, %who) { serverCmdRemoveMinesweeperPlayer(%client, %who); }
function serverCmdMKick(%client, %who) { serverCmdRemoveMinesweeperPlayer(%client, %who); }
function serverCmdMinesKick(%client, %who) { serverCmdRemoveMinesweeperPlayer(%client, %who); }
function serverCmdKickM(%client, %who) { serverCmdRemoveMinesweeperPlayer(%client, %who); }
function serverCmdMTeleport(%client, %slot) { serverCmdTPM(%client, %slot); }
function serverCmdMinesTeleport(%client, %slot) { serverCmdTPM(%client, %slot); }

function serverCmdMinesweeperHelp(%client) {
	messageClient(%client, '', "\c6=== \c0MINESWEEPER HELP \c6===");
	messageClient(%client, '', "\c6Click the grid and try not to click any mines! Flag the squares you know are mines with your plant brick or light key, find all of them to win!");
	messageClient(%client, '', "\c4/minesweeper [width] [length] [mines] \c7-- \c6Start a Minesweeper game, defaults to 32x32 and 128 mines");
	messageClient(%client, '', "\c4/stopMinesweeper \c7-- \c6Ends your Minesweeper game");
	messageClient(%client, '', "\c4/resetMinesweeper \c7-- \c6Restarts your Minesweeper game and reshuffles mines");
	messageClient(%client, '', "\c4/addMinesweeperPlayer [player] \c7-- \c6Lets players help you with your Minesweeper game");
	messageClient(%client, '', "\c4/removeMinesweeperPlayer [player] \c7-- \c6Kick a player from your Minesweeper game");
	messageClient(%client, '', "\c4/tpm [slot or blank] \c7-- \c6Teleports you back to your Minesweeper game, or a friend's");
}

function Player::doMinesweeperThing(%player) {
	%client = %player.client;
	%eye = vectorScale(%player.getEyeVector(), 5);
	%pos = %player.getEyePoint();
	%mask = $TypeMasks::FXBrickObjectType;
	%hit = getWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %player), 0);

	if(isObject(%hit) && (%client.minesweeperSlot == %hit.slot || %client.allowedToHelp[%hit.slot])) {
		if(%hit.minesweeper && !%hit.slot_owner.win) {
			%owner = %hit.slot_owner;
			if(!%hit.uncovered) {
				%isMine = $Minesweeper::IsMine[%hit.slot, %hit.x, %hit.y];

				if(%hit.flagged) {
					%hit.flagged = false;
					%hit.setPrint($printNameTable["1x1/Minesweeper1x1GridButton"]);
					if(%isMine) {
						%owner.minesweeperFlagsLeft++;
					}
					%owner.placedFlags--;
					%hit.playSound(MinesweeperFlagRemove);
				} else {
					%hit.flagged = true;
					%hit.setPrint($printNameTable["1x1/Minesweeper1x1GridFlagged"]);
					if(%isMine) {
						%owner.minesweeperFlagsLeft--;
					}
					%owner.placedFlags++;
					%hit.playSound(MinesweeperFlagPlace);
				}

				if(%owner.minesweeperFlagsLeft <= 0 && !%owner.win) {
					messageAll('', "\c4" @ %owner.name SPC "\c6has beaten a Minesweeper board!");
					%owner.win = true;
				}

				%str = "\c3" @ %owner.placedFlags SPC "/" SPC %owner.minesweeperFlags;
				%str = %str @ "<just:right>\c2" @ getTimeString(mFloatLength($Sim::Time - $Minesweeper::StartedAt[%hit.slot], 1)) @ " ";
				if(%owner != %client) {
					%owner.bottomPrint(%str, 5, 1);
					%player.client.bottomPrint(%str, 5, 1);
				} else {
					%player.client.bottomPrint(%str, 5, 1);
				}
				return true;
			}
			return true;
		}
	}

	return false;
}

package MinesweeperPackage {
	function fxDTSBrick::onActivate(%this, %player) {
		if(%this.minesweeper) {
			if(!%this.slot_owner.win) {
				if(%this.uncovered) {
					%this.playSound(MinesweeperClickNo);
				} else if(!%this.uncovered) {
					%this.playSound(MinesweeperClick);
				}
			}
			%this.MS_sweep(%player.client);
		}
		return parent::onActivate(%this, %player);
	}

	function serverCmdLight(%client) {
		%player = %client.player;
		if(!isObject(%player)) {
			return parent::serverCmdLight(%client);
		}

		if(!%player.doMinesweeperThing()) {
			return parent::serverCmdLight(%client);
		}
	}

	function serverCmdPlantBrick(%client) {
		%player = %client.player;
		if(!isObject(%player)) {
			return parent::serverCmdPlantBrick(%client);
		}

		if(!%player.doMinesweeperThing()) {
			return parent::serverCmdPlantBrick(%client);
		}
	}

	function GameConnection::onClientLeaveGame(%this) {
		if(%this.doingMinesweeper) {
			%group = nameToID("BrickGroup_Minesweeper" @ %this.minesweeperSlot);
			%group.chainDeleteAll();
		}

		serverCmdStopMinesweeper(%client);

		return parent::onClientLeaveGame(%this);
	}

	function GameConnection::spawnPlayer(%this) {
		%r = parent::spawnPlayer(%this);
		
		if(%this.doingMinesweeper) {
			%group = nameToID("BrickGroup_Minesweeper" @ %this.minesweeperSlot);
			%this.player.setTransform(vectorAdd(%group.getObject(getRandom(0, %group.getCount()-1)).getPosition(), "0 0 5"));
		}

		return %r;
	}
};
activatePackage(MinesweeperPackage);