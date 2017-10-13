$FallingPlatforms::BoardDirs = "Add-Ons/Gamemode_Falling_Platforms/boards\tconfig/server/Falling Platforms/boards";
$FallingPlatforms::ColorNames = "red\torange\tyellow\tgreen\tcyan\tblue\tpurple\tpink\tbrown\tblack\tgrey\twhite";
$FallingPlatforms::WarnString = "<font:Arial Black:48><just:right>%col<br>%col<br>%col";

function getFPColor(%col) {
	return getField($FallingPlatforms::ColorOrder, %col);
}

function getFPColorName(%col) {
	return getField($FallingPlatforms::ColorNames, getFPColor(%col));
}

function loadBoards() {
	%folders = $FallingPlatforms::BoardDirs;
	$FallingPlatforms::BoardCount = %count = 0;

	for(%i=0;%i<getFieldCount(%folders);%i++) {
		%folder = getField(%folders, %i);
		%pattern = %folder @ "/*.bls";

		dbg("Searching folder" SPC %folder, "board.cs");

		for(%file = findFirstFile(%pattern); isFile(%file); %file = findNextFile(%pattern)) {
			$FallingPlatforms::Board[%count] = %file;
			%count++;
			dbg("Found save" SPC %file @ "...", "board.cs");
		}
	}

	$FallingPlatforms::BoardCount = %count;
	dbg("Found" SPC %count SPC "saves.", "board.cs");
}
if($FallingPlatforms::InitBoards $= "") {
	$FallingPlatforms::InitBoards = true;
	loadBoards();
}

function loadLayout(%which) {
	%group = BrickGroup_888888;

	$FallingPlatforms::LoadingLayout = true;
	%group.deleteAll();

	messageAll('',"\c6Loading a new layout...");

	if(%which $= "") {
		if($FallingPlatforms::CurrentLayout $= "") {
			$FallingPlatforms::CurrentLayout = %which = 0;
		} else {
			$FallingPlatforms::CurrentLayout++;
			%which = $FallingPlatforms::CurrentLayout;
		}
	}

	if(%which >= $FallingPlatforms::BoardCount || %which < 0) {
		$FallingPlatforms::CurrentLayout = %which = 0;
	} else {
		$FallingPlatforms::CurrentLayout = %which;
	}

	%save = $FallingPlatforms::Board[%which];
	if(!isFile(%save)) {
		dbg("Save file doesn't exist!", "board.cs");
		return;
	}

	if(isObject(FallingPlatformsBrickList)) {
		FallingPlatformsBrickList.clear();
	}

	$LoadingBricks_PositionOffset = "0 0 15";
	$LoadingBricks_BrickGroup = %group;
	$LoadingBricks_Client = -1;
	$LoadingBricks_ColorMethod = 3; //should always be 3
	$LoadingBricks_FileName = %save;
	$LoadingBricks_Silent = true;
	$LoadingBricks_StartTime = getSimTime();
	ServerLoadSaveFile_Start(%save);
}

function getColorSetLength() {
	%count = 0;
	while(isObject("color" @ %count @ "SprayCanImage")) {
		%count++;
	}
	return %count;
}
$FallingPlatforms::ColorMax = getColorSetLength();

function shuffleBoard(%cols) {
	%group = BrickGroup_888888;

	if(!isObject(FallingPlatformsBrickList)) {
		%list = new GuiTextListCtrl(FallingPlatformsBrickList);
	} else {
		%list = FallingPlatformsBrickList;
		%list.clear();
	}

	for(%i=0;%i<%group.getCount();%i++) {
		%brick = %group.getObject(%i);

		%list.addRow(%brick.getID(), %brick.getID() TAB getRandom(-999999, 999999));
	}

	%list.sortNumerical(1, getRandom(0, 1));

	%rows = %list.rowCount();

	if(%cols > $FallingPlatforms::ColorMax) {
		%cols = $FallingPlatforms::ColorMax;
	}

	%col = 0;
	for(%row=0;%row<%rows;%row++) {
		%brick = getField(%list.getRowText(%row), 0);
		%brick.setColor(getFPColor(%col));

		%col++;
		if(%col >= %cols) {
			%col = 0;
		}
	}
}

function boardWarn(%col) {
	%name = getFPColorName(%col);

	dbg("Warning for color \"" SPC %name SPC "\"", "board.cs");
	%str = strReplace($FallingPlatforms::WarnString, "%col", "<color:" @ _FP_RGBToHex(getColorIDTable(getFPColor(%col))) @ ">" @ strUpr(%name) @ " ");
	centerPrintAll(%str, 4);
	
	for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
		%brick = BrickGroup_888888.getObject(%i);
		if(%brick.colorID == getFPColor(%col)) {
			%brick.doWarn();
		}
	}

	serverPlay2D(brickPlantSound);
	if($FallingPlatforms::Rounds < 40) {
		schedule(400, 0, serverPlay2D, brickPlantSound);
	}

	%warn_sound = "warn" @ getRandom(1, $FallingPlatforms::WarnSoundCount);
	for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
		%client = $DefaultMinigame.member[%i];
		if(%client.noSillySounds) {
			%client.playSound(warn1);
		} else {
			%client.playSound(%warn_sound);
		}
	}
}

function boardFall(%col, %invert) {
	for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
		%brick = BrickGroup_888888.getObject(%i);
		if(%brick.colorID == getFPColor(%col)) {
			if(%invert) {
				%brick.doFall();
			}
		} else {
			if(!%invert) {
				%brick.doFall();
			}
		}
	}

	%fall_sound = "fall" @ getRandom(1, $FallingPlatforms::FallSoundCount);
	for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
		%client = $DefaultMinigame.member[%i];
		if(%client.noSillySounds) {
			%client.playSound(brickBreakSound);
		} else {
			%client.playSound(%fall_sound);
		}
	}
}

function fxDTSBrick::doWarn(%this) {
	%this.setColorFX(3);
	%this.schedule(200, setColorFX, 0);
	if($FallingPlatforms::Rounds < 40) {
		%this.schedule(400, setColorFX, 3);
		%this.schedule(600, setColorFX, 0);	
	}
}

function fxDTSBrick::doFall(%this) {
	if($FallingPlatforms::Rounds >= 20) {
		%this.setColorFX(6);
		%this.setColliding(false);
		%this.schedule(2000, setColorFX, 0);
		%this.schedule(2000, setColliding, true);
	} else {
		%this.fakeKillBrick("0 0 -1", 3);
	}
}

function doBoardShuffleLoop() {
	cancel($FallingPlatforms::BoardShuffleLoopSched);
	$FallingPlatforms::BoardShuffleLoopSched = schedule(2000, 0, doBoardShuffleLoop);

	shuffleBoard(getRandom(2, $FallingPlatforms::ColorMax));
}

package FallingPlatformsBoardPackage {
	function ServerLoadSaveFile_End() {
		parent::ServerLoadSaveFile_End();

		$FallingPlatforms::LoadingLayout = false;
		$DefaultMinigame.beginPreGame();
	}
};
activatePackage(FallingPlatformsBoardPackage);