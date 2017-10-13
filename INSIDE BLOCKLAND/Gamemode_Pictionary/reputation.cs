// +1 for each successful guess
// -x*2 if > 50% of non-afk players fail to guess the picture
//		e.g. out of 14 active players, if 5 guess it, drawer loses 18 (9*2) points
// x*0.5 if > 50% of non-afk players guess the picture
// x*2 if all players guess the picture
//		e.g. out of 14 active players, if all guess it, drawer gains 28 (14*2) points

function MinigameSO::determineExtraRep(%this) {
	%max = 0;
	%guessed = 0;

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(%client.canDraw || (%client.afk && !%client.guessed)) {
			continue;
		}

		%max++;

		if(%client.guessed) {
			%guessed++;
		}
	}

	dbg(%guessed SPC "/" SPC %max SPC "players managed to guess the word.", "reputation.cs");

	%amnt = 0;
	%ratio = %guessed/%max;

	if(%ratio < 0.5) {
		%amnt = (%max - %guessed) * 2 * -1;
	} else if(%ratio >= 0.5 && %ratio < 1) {
		%amnt = mCeil(%guessed * 0.75);
	} else if(%ratio >= 1) {
		%amnt = %max * 2;
	}

	dbg("Player should receive a" SPC %amnt SPC "rep modification.", "reputation.cs");
	return %amnt;
}

function GameConnection::modRep(%this, %amnt) {
	dbg("Modded reputation on" SPC %this.bl_id SPC "by" SPC %amnt, "reputation.cs", 3);
	%this.rep += %amnt;

	%this.saveRep();
	%this.score = %this.rep;
}

function GameConnection::loadRep(%this) {
	%filename = "config/server/Pictionary/reputation/" @ %this.bl_id;
	if(!isFile(%filename)) {
		%this.rep = 0;
		return;
	}

	%file = new FileObject(%this);
	%file.openForRead(%filename);

	%this.rep = %file.readLine();
	%this.score = %this.rep;
	messageClient('', "Your reputation score from\c4" SPC %file.readLine() SPC "\c6was loaded successfully.");

	%file.close();	
}

function GameConnection::saveRep(%this) {
	%file = new FileObject(%this);
	%file.openForWrite("config/server/Pictionary/reputation/" @ %this.bl_id);

	%file.writeLine(%this.rep);
	%file.writeLine(getDateTime());

	%file.close();
}

package PictionaryRepPackage {
	function GameConnection::autoAdminCheck(%this) {
		%this.loadRep();

		%this.oldPrefix = %this.clanPrefix;

		return Parent::autoAdminCheck(%this);
	}
};
activatePackage("PictionaryRepPackage");