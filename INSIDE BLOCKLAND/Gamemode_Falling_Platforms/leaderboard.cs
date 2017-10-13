function modLeaderboard(%client, %pb) {
	if(%pb $= "") {
		%list = FallingPlatformsLeaderboard;
	} else {
		%list = FallingPlatformsPBLeaderboard;
	}

	%data = %list.getRowTextByID(%client.bl_id);
	
	if(%pb $= "") {
		if(%data $= "") {
			%list.addRow(%client.bl_id, %client.name @ "\t" @ (%client.totalScore || 0) @ "\t" @ %client.bl_id);
		} else {
			%list.setRowByID(%client.bl_id, %client.name @ "\t" @ (%client.totalScore || 0) @ "\t" @ %client.bl_id);
		}
	} else {
		if(%data $= "") {
			%list.addRow(%client.bl_id, %client.name @ "\t" @ (%client.roundRecord || 0) @ "\t" @ %client.bl_id);
		} else {
			%list.setRowByID(%client.bl_id, %client.name @ "\t" @ (%client.roundRecord || 0) @ "\t" @ %client.bl_id);
		}		
	}

	%list.sortNumerical(1, 0);
}

function initLeaderboard() {
	if(!isObject(FallingPlatformsLeaderboard)) {
		new GuiTextListCtrl(FallingPlatformsLeaderboard);
	}

	if(!isObject(FallingPlatformsPBLeaderboard)) {
		new GuiTextListCtrl(FallingPlatformsPBLeaderboard);
	}

	%list = FallingPlatformsLeaderboard;
	%pblist = FallingPlatformsPBLeaderboard;
	%pattern = "config/server/FallingPlatforms/saves/*";

	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		%bl_id = fileBase(%filename);
		%file = new FileObject();
		%file.openForRead(%filename);

		// garbage
		%file.readLine(); // date
		
		%record = %file.readLine();
		%score = %file.readLine();
		%name = %file.readLine();
		if(%name $= "") {
			%name = "BL_ID" SPC %bl_id;
		}

		%data = %list.getRowTextByID(%bl_id);
		if(%data $= "") {
			%list.addRow(%bl_id, %name @ "\t" @ (%score || 0) @ "\t" @ %bl_id);
		} else {
			%list.setRowByID(%bl_id, %name @ "\t" @ (%score || 0) @ "\t" @ %bl_id);
		}

		%data = %pblist.getRowTextByID(%bl_id);
		if(%data $= "") {
			%pblist.addRow(%bl_id, %name @ "\t" @ (%record || 0) @ "\t" @ %bl_id);
		} else {
			%pblist.setRowByID(%bl_id, %name @ "\t" @ (%record || 0) @ "\t" @ %bl_id);
		}
	}

	%list.sortNumerical(1, 0);
	%pblist.sortNumerical(1, 0);
	
	$FallingPlatforms::initLeaderboard = true;
}
if(!$FallingPlatforms::initLeaderboard) {
	initLeaderboard();
}

package FallingFallingPlatformsLeaderboardPackage {
	function serverCmdMessageSent(%client, %msg) {
		%row = FallingPlatformsLeaderboard.getRowNumByID(%client.bl_id);
		if(%row == -1) {
			return parent::serverCmdMessageSent(%client, %msg);
		}
		%row++;

		%count = FallingPlatformsLeaderboard.rowCount();
		%col = "FFFFFF";

		if(%row > mFloor(%count/2)) {
			%endCol = "255 255 255";
			%startCol = "0 255 0";
			%weight = mClampF((%row-(%count/2))/(%count/2), 0, 1);

			%col = _FP_RGBToHex(interpolateColor(%startCol, %endCol, %weight, 1));
		} else {
			%endCol = "0 255 0";
			%startCol = "0 255 255";
			%weight = mClampF(%row/(%count/2), 0, 1);

			//talk(%weight);

			%col = _FP_RGBToHex(interpolateColor(%startCol, %endCol, %weight, 1));
		}

		%client.clanPrefix = "\c7[<color:" @ %col @ ">" @ getPositionString(%row) @ "\c7]" SPC %client.original_prefix;
		return parent::serverCmdMessageSent(%client, %msg);
	}
};
activatePackage(FallingFallingPlatformsLeaderboardPackage);