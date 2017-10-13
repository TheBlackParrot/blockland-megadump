//exec("Add-Ons/Gamemode_Cubes_To_Heaven/server.cs");
//startGeneratingCubes(400);
if($CubesToHeaven::CubeCount $= "") {
	$CubesToHeaven::CubeCount = 0;
}
if($CubesToHeaven::Seed $= "") {
	$CubesToHeaven::Seed = setMapSeed();
}
$CubesToHeaven::ScaleFactor = 1;

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

function startGeneratingStages() {
	$x = 0;
	$y = 0;
	$z = 50;
	$areas = 0;
	$requiredforStage = $oldRequire = 0;
	$rot = "0 0 -1 90";

	BrickGroup_888888.chainDeleteAll();
	waitForClearingStages();
}

function waitForClearingStages() {
	if(BrickGroup_888888.getCount() > 0) {
		schedule(200, 0, waitForClearingStages);
		return;
	}

	generateStage();
}

function generateSpawn() {
	$areas++;

	$db = "brick16x16FData";
	
	$x += $db.brickSizeX / 3;
	$y += $db.brickSizeY / 3;

	%brick = new fxDTSBrick(HeavenSpawn) {
		angleID = 0;
		colorFxID = 0;
		colorID = 0;
		dataBlock = $db;
		isBasePlate = 1;
		isPlanted = 1;
		position = $x SPC $y SPC $z;
		printID = 0;
		scale = "1 1 1";
		shapeFxID = 0;
		stackBL_ID = 888888;
		enableTouch = 1;
		rotation = $rot;
		area = $areas;
	};
	while(%brick.plant() == 1) {
		%brick.delete();
		$x += 0.3;
		$y += 0.3;

		%brick = new fxDTSBrick(HeavenSpawn) {
			angleID = 0;
			colorFxID = 0;
			colorID = 0;
			dataBlock = $db;
			isBasePlate = 1;
			isPlanted = 1;
			position = $x SPC $y SPC $z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			enableTouch = 1;
			rotation = $rot;
			area = $areas;
		};
	}
	%brick.setTrusted(1);
	BrickGroup_888888.add(%brick);

	$x += $db.brickSizeX / 3;
	$y += $db.brickSizeY / 3;	
}

function generateStage() {
	generateSpawn();

	%new_x = $x;
	%new_y = $y;
	%new_z = $z;

	%stage = getRandom(0, 11);

	switch(%stage) {
		case 0:
			%static = 95;
			%areas_dyn = 5;
			%db = "brick4xCubeData";

		case 1:
			%static = 140;
			%areas_dyn = 5;
			%db = "brick4xCubeData";

		case 2:
			%static = 94;
			%areas_dyn = 2;
			%db_poss = "brick1x1fRoundData brick2x2Data brick1x1Data";
			%db = getWord(%db_poss, getRandom(0, getWordCount(%db_poss)-1));

		case 3:
			%static = 180;
			%areas_dyn = 5;
			%db = "brick4xCubeData";

		case 4:
			%static = 500;
			%areas_dyn = 20;
			%db = "brick1x1FData";

		case 5:
			%static = 100;
			%areas_dyn = 3;
			%db = "brick8x8FData";

		case 6:
			%static = 100;
			%areas_dyn = 3;
			%db = "brick2x12x5Data";

			%brick = new fxDTSBrick(HeavenCube) {
				angleID = 0;
				colorFxID = 0;
				colorID = %stage;
				dataBlock = "brick8x8FData";
				isBasePlate = 1;
				isPlanted = 1;
				position = $x-2 SPC $y SPC $z-1.6;
				printID = 0;
				scale = "1 1 1";
				shapeFxID = 0;
				stackBL_ID = 888888;
				enableTouch = 1;
				rotation = $rot;
			};
			%brick.setTrusted(1);
			%brick.plant();
			BrickGroup_888888.add(%brick);

		case 7:
			%static = 100;
			%areas_dyn = 5;
			%way_length = getRandom(3, 10);
			%db_poss = "brick4x4FData brick2x4FData brick2x2FData brick1x1FData";
			%db = getWord(%db_poss, getRandom(0, getWordCount(%db_poss)-1));

		case 8:
			%static = 80;
			%areas_dyn = 7;
			%db = "brick8x8FData";
			%which = getRandom(0, 1);

		case 9:
			%static = 80;
			%areas_dyn = 5;
			%db = "brick2x2DiscData";
			%last_x = 0;

		case 10:
			%static = 15;
			%areas_dyn = 1;
			%db = "brick6x6FData";
			%which = getRandom(0, 1);

		case 11:
			%static = 7;
			%areas_dyn = 1;
			%db = "brick4x3RampData";
			$rot = "1 0 0 0";			
	}

	%oldCount = $CubesToHeaven::CubeCount;
	%requiredforStage = mCeil((%static + ($areas * %areas_dyn)) * $CubesToHeaven::ScaleFactor);
	%midCheckpoint = $CubesToHeaven::CubeCount + mCeil((%requiredforStage/2) * $CubesToHeaven::ScaleFactor);

	%end = false;

	for(%i=0;%i<%requiredforStage;%i++) {
		if(%i == %requiredforStage-1) {
			%end = true;
		}

		%brick = $HeavenBrick[$CubesToHeaven::CubeCount] = new fxDTSBrick(HeavenCube) {
			angleID = 0;
			colorFxID = 0;
			colorID = %stage;
			dataBlock = %db;
			isBasePlate = 1;
			isPlanted = 1;
			position = $x SPC $y SPC $z;
			printID = 0;
			scale = "1 1 1";
			shapeFxID = 0;
			stackBL_ID = 888888;
			enableTouch = 1;
			rotation = $rot;
			checkpoint = (%i >= %midCheckpoint ? %midCheckpoint : %oldCount);
		};

		while(%brick.plant() == 1) {
			%brick.delete();
			$x = %new_x = $x + 0.3;
			$y = %new_y = $y + 0.3;
			%brick = $HeavenBrick[$CubesToHeaven::CubeCount] = new fxDTSBrick(HeavenCube) {
				angleID = 0;
				colorFxID = 0;
				colorID = %stage;
				dataBlock = %db;
				isBasePlate = 1;
				isPlanted = 1;
				position = $x SPC $y SPC $z;
				printID = 0;
				scale = "1 1 1";
				shapeFxID = 0;
				stackBL_ID = 888888;
				enableTouch = 1;
				rotation = $rot;
				checkpoint = (%i >= %midCheckpoint ? %midCheckpoint : %oldCount);
			};
		}
		%brick.setTrusted(1);
		BrickGroup_888888.add(%brick);

		// i took the lazy way out im sorry pls dont kill
		%iterations = 0;

		switch(%stage) {
			case 0 or 2:
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

			case 1:
				%new_x = $x + getRandom(-70, 70)/10;
				%new_y = $y + getRandom(0, 110)/10;
				%new_z = $z;

				while(vectorLen(vectorSub($x SPC $y SPC $z, %new_x SPC %new_y SPC %new_z)) > 11.5) {
					%new_x = $x + getRandom(-70, 70)/10;
					%new_y = $y + getRandom(0, 110)/10;
					%new_z = $z;
				}

				initContainerRadiusSearch(%new_x SPC %new_y SPC %new_z, 5, $TypeMasks::FXBrickObjectType);
				while(isObject(%object = containerSearchNext()) && %iterations < 100) {
					%new_x = $x + getRandom(-70, 70)/10;
					%new_y = $y + getRandom(0, 110)/10;
					%new_z = $z;
					initContainerRadiusSearch(%new_x SPC %new_y SPC %new_z, 5, $TypeMasks::FXBrickObjectType);
					%iterations++;
				}

			case 3:
				%new_x = $x + getRandom(-30, 30)/10;
				%new_y = $y + (%db.brickSizeY / 2);
				%new_z = $z + (%db.brickSizeZ / 5);

			case 4:
				%new_x = $x + (%db.brickSizeX / 2);
				%new_y = $y + getRandom(0, 20)/10;
				%new_z = $z;

			case 5:
				%new_x = $x + (%db.brickSizeX / 2) + (getRandom(-40, 0)/10 * (getRandom(0, 1) ? 1 : -1));
				%new_y = $y - (%db.brickSizeY / 2);
				%new_z = $z + (%db.brickSizeZ / 5);

			case 6:
				%new_z += (%db.brickSizeZ / 5);

				switch((%i - %startedAt) % 4) {
					case 0:
						$rot = "0 0 1 180";
						%new_x -= 2.5; // hardcoded for now sorry
						%new_y += 2.5;
					case 1:
						$rot = "0 0 -1 90";
						%new_x += 2.5;
						%new_y += 2.5;
					case 2:
						$rot = "1 0 0 0";
						%new_x += 2.5;
						%new_y -= 2.5;
					case 3:
						$rot = "0 0 -1 90";
						%new_x -= 2.5;
						%new_y -= 2.5;
				}

			case 7:
				%new_z += (%db.brickSizeZ / 5) * 8;

				switch(mFloor(%i/%way_length) % 4) {
					case 0:
						%new_x += 8;

					case 1:
						%new_y += 8;

					case 2:
						%new_x -= 8;

					case 3:
						%new_y -= 8;
				}

			case 8:
				%part = %i % 13;
				%dir = %i % 26;

				if(%part < 6) {
					%new_z += ((%db.brickSizeZ / 5) * %part) * 3;
					if(%which) {
						if(%dir < 13) {
							%new_x -= %db.brickSizeX / 2;	
						} else {
							%new_x += %db.brickSizeX / 2;	
						}
					} else {
						if(%dir < 13) {
							%new_x += %db.brickSizeX / 2;	
						} else {
							%new_x -= %db.brickSizeX / 2;	
						}
					}				
				} else {
					%new_z += ((%db.brickSizeZ / 5) * mAbs(%part-12)) * 3;
					if(%which) {
						if(%dir < 13) {
							%new_x += %db.brickSizeX / 2;	
						} else {
							%new_x -= %db.brickSizeX / 2;	
						}
					} else {
						if(%dir < 13) {
							%new_x -= %db.brickSizeX / 2;	
						} else {
							%new_x += %db.brickSizeX / 2;	
						}
					}	
				}

			case 9:
				%new_z += 1;
				if(%i % 2) {
					%last_x = getRandom(-4, 4);
				} else {
					%last_x = %last_x * -1;
				}

				%new_x += %last_x;
				%new_y += %db.brickSizeY / 2;

			case 10:
				%new_z = $z;

				if(%which) {
					%new_x = $x;
					%new_y += 12.5;
				} else {
					%new_y = $y;
					%new_x += 12.5;
				}

			case 11:
				%new_z = $z;
				%new_x = $x;

				if(%i % 2) {
					$rot = "1 0 0 0";
					%new_y += 1.5;
				} else {
					$rot = "0 0 1 180";
					%new_y += 10.5;
				}


		}

		$CubesToHeaven::CubeCount++;

		if(!%end) {
			$x = %new_x;
			$y = %new_y;
			$z = %new_z;
		}
	}
}

package CubesToHeavenPackage {
	function fxDTSBrick::onActivate(%this, %player) {
		if(%this.area $= "") {
			return parent::onActivate(%this, %player);
		}

		%client = %player.client;

		if(%client.onArea $= "" || %this.area > %client.onArea) {
			%client.onArea = %this.area;
		}

		return parent::onActivate(%this, %player);
	}
};