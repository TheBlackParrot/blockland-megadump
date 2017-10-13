function createCube(%x,%y,%z,%bomb) {
	if((!getRandom(0,80) && %z >= $Pref::Take::PlayAreaHeight/3) || %bomb) {
		%shapeFX = 1;
	}
	%brick = new fxDTSBrick(TakeBrick) {
		angleID = 0;
		colorFxID = 3;
		colorID = $Pref::Take::DefaultColor;
		//colorID = getRandom(1,63);
		dataBlock = "brick16xCubeData";
		isBasePlate = 1;
		isPlanted = 1;
		position = %x SPC %y SPC %z;
		printID = 0;
		scale = "1 1 1";
		shapeFxID = (%shapeFX | 0);
		stackBL_ID = 888888;
		takenBy = -1;
		enableTouch = 1;
	};
	BrickGroup_888888.add(%brick);
	%brick.setTrusted(1);
	%error = %brick.plant();
	%brick.setColliding(0);

	return %error SPC %brick;
}

function spawnRandomCubes(%amount,%bombs) {
	while(%amount > 0) {
		%error_check = createCube(getRandom(0,$Pref::Take::PlayAreaSize),getRandom(0,$Pref::Take::PlayAreaSize),getRandom(0,$Pref::Take::PlayAreaHeight)+$Pref::Take::VerticalOffset,%bombs);
		%error_val = getWord(%error_check,0);
		%brick = getWord(%error_check,1);
		while(%error_val && %error_val != 2) {
			%brick.delete();
			%error_check = createCube(getRandom(0,$Pref::Take::PlayAreaSize),getRandom(0,$Pref::Take::PlayAreaSize),getRandom(0,$Pref::Take::PlayAreaHeight)+$Pref::Take::VerticalOffset,%bombs);
			%error_val = getWord(%error_check,0);
			%brick = getWord(%error_check,1);
		}
		%amount--;
	}
}