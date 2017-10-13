$QuicklyNow::ValidBricks = "brick16xCubeData brick32xCubeData brick64xCubeData";
$QuicklyNow::ValidSingleBricks = "brick32xCubeData";

function spawnCube(%type) {
	// ha ha GameCube
	%brick = new fxDTSBrick(GameCube) {
		angleID = 0;
		colorFxID = 3;
		colorID = getRandom(0, 12);
		dataBlock = %type;
		isBasePlate = 1;
		isPlanted = 1;
		position = getRandom(-200, 200) SPC getRandom(-200, 200) SPC getRandom(-200, 200);
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = 888888;
	};
	BrickGroup_888888.add(%brick);
	%brick.setTrusted(1);
	%brick.setColliding(0);

	%error = %brick.plant();

	return %error SPC %brick.getID();
}

function regenCubes(%count) {
	BrickGroup_888888.chainDeleteAll();
	waitForCubesCleared(%count);
}

function waitForCubesCleared(%count) {
	cancel($cubeClearLoop);
	if(BrickGroup_888888.getCount() > 0) {
		$cubeClearLoop = schedule(33, 0, waitForCubesCleared, %count);
		return;
	}

	generateCubes(%count);
}

function generateCubes(%count) {
	%which = ($QuicklyNow::Mode ? $QuicklyNow::ValidSingleBricks : $QuicklyNow::ValidBricks);
	%type = getWord(%which, getRandom(0, getWordCount(%which)-1));
	
	for(%i=0;%i<%count;%i++) {
		%error_check = spawnCube(%type);
		%val = getWord(%error_check, 0);
		%brick = getWord(%error_check, 1);

		while(%val && %val != 2) {
			%brick.delete();
			%error_check = spawnCube(%type);
			%val = getWord(%error_check, 0);
			%brick = getWord(%error_check, 1);
		}
	}
}