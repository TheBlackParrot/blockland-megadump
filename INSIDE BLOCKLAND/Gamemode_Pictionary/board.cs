$Pictionary::Board::StartBrick = _boardStart;
$Pictionary::Board::StopBrick = _boardEnd;
$Pictionary::Board::Height = 40;
$Pictionary::Board::Axis = 0; // 0 or 1
$Pictionary::Board::Direction = -1; // 1 or -1

function generateBoard() {
	%start = $Pictionary::Board::StartBrick.getTransform();
	%end = $Pictionary::Board::StopBrick.getTransform();

	%init_z = getWord(%start, 2);
	%step_z = $Pictionary::Board::StartBrick.getDatablock().brickSizeZ/5;
	%height = $Pictionary::Board::Height * %step_z;

	%step_x = $Pictionary::Board::StartBrick.getDatablock().brickSizeX/2;

	%bgen_start = getWord(%start, $Pictionary::Board::Axis) + (%step_x * $Pictionary::Board::Direction);
	%bgen_end = getWord(%end, $Pictionary::Board::Axis);

	%amnt = 0;
	%cols = 0;
	%p = 0;

	for(%x = %bgen_start; (%bgen_start > %bgen_end ? %x > %bgen_end : %x < %bgen_end); %x += (%step_x * $Pictionary::Board::Direction)) {
		for(%z = %init_z; %z < (%height + %init_z); %z += %step_z) {
			%pos = %x SPC getWord(%start, 1) SPC %z;

			%brick = new fxDTSBrick(BoardBrick : _boardStart) {
				colorFxID = 3;
				colorID = 63;
				// todo: allow any brick to be a board brick
				dataBlock = "brick1x1PrintData";
				// todo: more intelligent board generation direction determining
				position = %pos;
				printID = 60;
			};
			$Pictionary::BrickGroup.add(%brick);
			%brick.setTrusted(1);
			%brick.plant();

			%amnt++;

			$Pictionary::BoardBrick[%cols, %p] = %brick;
			%p++;
		}

		%p = 0;
		%cols++;
	}

	// xpm.cs
	$Pictionary::XPM::Width = %cols;
	$Pictionary::XPM::Height = %amnt/%cols;

	dbg("Generated a" SPC %cols @ "x" @ %amnt/%cols SPC "board.", "board.cs");
	dbg("Generated" SPC %amnt SPC "board bricks.", "board.cs");
}

function regenerateBoard() {
	dbg("Regenerating board...", "board.cs");

	$Pictionary::BrickGroup.deleteAll();
	schedule(1000, 0, generateBoard);
}

function fillBoard(%colorID) {
	if(%colorID < 0 || %colorID > 63 || %colorID $= "") {
		%colorID = 63;
	}

	dbg("Filling board with color" SPC %colorID, "board.cs");
	
	for(%i=0;%i<$Pictionary::BrickGroup.getCount();%i++) {
		%brick = $Pictionary::BrickGroup.getObject(%i);
		%brick.setColor(%colorID);
	}
}

package PictionaryBoardWrapperPackage {
	function ServerLoadSaveFile_End() {
		parent::ServerLoadSaveFile_End();

		if(!isObject(BrickGroup_888889)) {
			new SimGroup(BrickGroup_888889 : BrickGroup_888888) {
				abandonedTime = "0";
				bl_id = "-1";
				chainDeleteSchedule = "0";
				client = "-1";
				isPublicDomain = "1";
				name = "Pictionary Board";
			};
			
			mainBrickGroup.add(BrickGroup_888889);
			$Pictionary::BrickGroup = BrickGroup_888889;

			dbg("Created BrickGroup_888889", "board.cs");

			PictionaryWordDB.loadDB();
			$DefaultMinigame.nextRound();
		}

		regenerateBoard();
	}
};
activatePackage(PictionaryBoardWrapperPackage);