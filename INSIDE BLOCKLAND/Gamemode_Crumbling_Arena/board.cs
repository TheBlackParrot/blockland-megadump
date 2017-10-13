function newBoard(%width, %length, %height, %db) {
	dbg("Clearing bricks...", "board.cs", "newBoard");

	$CrumblingArena::Board::Width = %width;
	$CrumblingArena::Board::Length = %length;
	$CrumblingArena::Board::Height = %height;
	$CrumblingArena::Board::Datablock = %db;

	BrickGroup_888888.chainDeleteCallback = "generateBoard();";
	BrickGroup_888888.chainDeleteAll();
}

function generateBoard(%width, %length, %height, %db) {
	if(%width $= "") { %width = $CrumblingArena::Board::Width; }
	if(%length $= "") { %length = $CrumblingArena::Board::Length; }
	if(%height $= "") { %height = $CrumblingArena::Board::Height; }
	if(%db $= "") { %db = $CrumblingArena::Board::Datablock; }

	%db = "brick" @ %db @ "Data";

	%col_inv = getRandom(0, 1);
	setGradient(getRandom(0, $CrumblingArena::Gradients.length - 1));

	dbg("Generating a new board...", "board.cs", "generateBoard");

	$CrumblingArena::SpawnBox::Start = 0 SPC 0 SPC 12 + (%height * (%db.brickSizeZ/5));
	$CrumblingArena::SpawnBox::End = (%width-1) * (%db.brickSizeX/2) SPC (%length-1) * (%db.brickSizeY/2) SPC 12 + (%height * (%db.brickSizeZ/5));
	dbg("Spawn box set to" SPC $CrumblingArena::SpawnBox::Start @ ", " @ $CrumblingArena::SpawnBox::End, "board.cs", "generateBoard");

	for(%z=0;%z<%height;%z++) {
		%color = getGradientPart(%z, %height, %col_inv);
		for(%x=0;%x<%width;%x++) {
			for(%y=0;%y<%length;%y++) {
				%pos = %x * (%db.brickSizeX/2) SPC %y * (%db.brickSizeY/2) SPC 10 + (%z * (%db.brickSizeZ/5));

				%brick = new fxDTSBrick(CABrick) {
					angleID = 0;
					colorFxID = 0;
					colorID = %color;
					dataBlock = %db;
					isBasePlate = 0;
					isPlanted = 1;
					position = %pos;
					printID = 0;
					scale = "1 1 1";
					shapeFxID = 0;
					stackBL_ID = 888888;
					enableTouch = 1;
				};
				%brick.setTrusted(1);
				%brick.plant();
				BrickGroup_888888.add(%brick);
			}
		}
	}

	$DefaultMinigame.onBoardFinishGen();
}