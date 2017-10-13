function initSampleLeaderboard() {
	if(isObject($TestLeaderboard)) {
		return;
	}

	%lb = $TestLeaderboard = createLeaderboard("KILLS;;DEATHS", "0;;0", 3);

	for(%i=0;%i<100;%i++) {
		%bl_id = getRandom(100, 300000);
		%lb.update(%bl_id, "KILLS", getRandom(0, 500));
		%lb.update(%bl_id, "DEATHS", getRandom(0, 500));
	}
}
initSampleLeaderboard();

function serverCmdLeaderboard(%client, %col, %order) {
	%lb = $TestLeaderboard;

	if(%col $= "columns" || %col $= "cols" || %col $= "col") {
		%out = "";
		for(%i=3;%i<getFieldCount(%lb.cols);%i++) {
			%out = trim(%out SPC getField(%lb.cols, %i));
		}
		messageClient(%client, '', "\c6" @ %out);
		return;
	}

	%order = (%order $= "" ? 0 : %order);

	if(%lb.lbsort(%col, %order)) {
		%count = %lb.rowCount();
		for(%i=0;%i<%count;%i++) {
			%name = %lb.getColValue(%lb.getRowID(%i), "NAME");
			%val = %lb.getColValue(%lb.getRowID(%i), %col);

			if(%val $= "") {
				%i--;
				%count--;
				continue;
			}

			messageClient(%client, '', "\c2" @ (%i+1) @ ".\c6" SPC %name SPC "\c7--\c3" SPC %val);
		}
	} else {
		messageClient(%client, '', "\c0That column does not exist.");
	}
}