function GameConnection::saveData(%this) {
	%filename = "config/server/FallingPlatforms/saves/" @ %this.bl_id;

	%file = new FileObject();
	%file.openForWrite(%filename);

	%file.writeLine(getDateTime());
	%file.writeLine(%this.roundRecord);
	%file.writeLine(%this.totalScore);
	%file.writeLine(%this.name);
	%file.writeLine(%this.noSillySounds);

	%file.close();
	%file.delete();
}

function GameConnection::loadData(%this) {
	%filename = "config/server/FallingPlatforms/saves/" @ %this.bl_id;

	if(!isFile(%filename)) {
		return;
	}

	%file = new FileObject();
	%file.openForRead(%filename);

	messageClient(%this, '', "\c6Your save data from\c4" SPC %file.readLine() SPC "\c6has been loaded.");
	%this.roundRecord = %file.readLine();
	%this.totalScore = %file.readLine();
	%file.readLine(); // garbage
	%this.noSillySounds = %file.readLine();

	%file.close();
	%file.delete();
}