// hey it's me ur optimizer pls don't use zebase for score databases thanks <3

function GameConnection::loadData(%this) {
	// it originally checked in config/SuperCreeper and i find that irritating
	// the server folder exists for a reason, put it there

	%filename = "config/server/SuperCreeper/saves" @ %this.bl_id;
	if(isFile(%filename)) {
		%file = new FileObject();
		%file.openForRead(%filename);

		%this.points = %this.score = %file.readLine();
		%this.creepKillLvl = %file.readLine();
		%this.creepBombLvl = %file.readLine();

		%file.close();
		%file.delete();
	} else {
		// fucking end me, why are you starting level variables at ONE
		// start at ZERO so i don't have to do THIS CRAP
		%this.creepKillLvl = 1;
	}
}

function GameConnection::saveData(%this) {
	%filename = "config/server/SuperCreeper/saves" @ %this.bl_id;

	%file = new FileObject();
	%file.openForWrite(%filename);

	%file.writeLine(%this.points);
	%file.writeLine(%this.creepKillLvl);
	%file.writeLine(%this.creepBombLvl);

	%file.close();
	%file.delete();
}