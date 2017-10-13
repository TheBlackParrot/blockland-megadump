$Pref::Take::SaveDir = "config/server/brickochet/saves";

function GameConnection::saveTakeGame(%this) {
	%file = new FileObject();
	%file.openForWrite($Pref::Take::SaveDir @ "/" @ %this.bl_id);

	%file.writeLine("client" TAB "score" TAB (%this.score | 0) TAB "int");
	%file.writeLine("client" TAB "highestCombo" TAB (%this.highestCombo | 0) TAB "int");
	%file.writeLine("client" TAB "wins" TAB (%this.wins | 0) TAB "int");
	%file.writeLine("client" TAB "losses" TAB (%this.losses | 0) TAB "int");
	%file.writeLine("client" TAB "enableFloat" TAB (%this.enableFloat | 0) TAB "bool");

	%file.close();
	%file.delete();
}

function GameConnection::loadTakeGame(%this) {
	%file = new FileObject();
	%file.openForRead($Pref::Take::SaveDir @ "/" @ %this.bl_id);

	while(!%file.isEOF()) {
		%line = %file.readLine();

		// identifiers
		%client = %this;
		%player = %this.player;

		%data[identifier] = stripMLControlChars(getField(%line,0));
		%data[variable] = stripMLControlChars(getField(%line,1));
		%data[value] = stripMLControlChars(getField(%line,2));
		%data[type] = stripMLControlChars(getField(%line,3));
		if(%data[type] $= "str") {
			%data[value] = "\"" @ %data[value] @ "\"";
		}

		eval("%" @ %data[identifier] @ "." @ %data[variable] @ " = " @ %data[value] @ ";");
	}

	%file.close();
	%file.delete();
}