function createLeaderboard(%cols, %defaults, %default_sort) {
	%board = new GuiTextListCtrl(LeaderboardObject) {
		created = getUTC();
		modified = getUTC();
		cols = "NAME\tBL_ID\tMODIFIED\t" @ strReplace(%cols, ";;", "\t");
		defaults = "UNKNOWN\t-1\t" @ getUTC() TAB strReplace(%defaults, ";;", "\t");
		default_sort = (%default_sort $= "" ? 3 : %default_sort);
	};

	return %board;
}

function LeaderboardObject::getColumnIdx(%this, %col) {
	%cols = %this.cols;
	%idx = 0;
	%exists = false;

	// ...TIL
	while("" !$= %cols) {
		%cols = nextToken(%cols, "selected_col", "\t");
		echo(%selected_col);

		if(trim(%selected_col) $= %col) {
			%exists = true;
			break;
		} else {
			%idx++;
		}
	}

	if(!%exists) {
		return -1;
	}
	return %idx;
}

function LeaderboardObject::update(%this, %bl_id, %col, %value) {
	%row_text = %this.getRowTextByID(%bl_id);
	echo("defaults: " @ %this.defaults);

	if(%row_text $= "") {
		%client = findClientByBL_ID(%bl_id);
		if(!isObject(%client)) {
			%name = "Blockhead" @ %bl_id @ "?";
		} else {
			%name = %client.getPlayerName();
		}

		%defs = setField(%this.defaults, 0, %name);
		%defs = setField(%defs, 1, %bl_id);
		%defs = setField(%defs, 2, getUTC());

		%this.addRow(%bl_id, %defs);
		%row_text = %this.getRowTextByID(%bl_id);
	}

	%idx = %this.getColumnIdx(%col);
	if(%idx == -1) {
		echo("column" SPC %col SPC "does not exist");
		return;
	}

	%row_text = setField(%row_text, 2, getUTC());
	%this.setRowByID(%bl_id, setField(%row_text, %idx, %value));

	%this.modified = getUTC();
}

function LeaderboardObject::getColValue(%this, %bl_id, %col) {
	%idx = %this.getColumnIdx(%col);
	if(%idx == -1) {
		echo("column" SPC %col SPC "does not exist");
		return false;
	}

	%row_text = %this.getRowTextByID(%bl_id);
	if(%row_text $= "") {
		return false;
	}

	return getField(%row_text, %idx);
}

function LeaderboardObject::lbsort(%this, %col, %order) {
	%idx = %this.getColumnIdx(%col);
	if(%idx == -1) {
		echo("column" SPC %col SPC "does not exist");
		return false;
	}

	%this.sortNumerical(%idx, %order);
	return true;
}