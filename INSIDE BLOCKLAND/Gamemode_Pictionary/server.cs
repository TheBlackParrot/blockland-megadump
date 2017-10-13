$Pictionary::Debug = false;
$Pictionary::DebugLevel = 2;

function dbg(%msg, %file, %lvl) {
	if(!$Pictionary::Debug) {
		return;
	}

	if(%lvl $= "" || %lvl <= 0) {
		%lvl = 1;
	}

	if(%lvl > $Pictionary::DebugLevel) {
		return;
	}

	echo("\c1[" @ $Sim::Time @ "] \c4[DEBUG] \c5[" @ %file @ "] \c6" @ %msg);
	messageAll('', "\c7[" @ $Sim::Time @ "] \c1[DEBUG] \c5[" @ %file @ "] \c6" @ %msg);
}

exec("./board.cs");
exeC("./xpm.cs");
exec("./draw.cs");
exec("./words.cs");
exec("./system.cs");
exec("./afk.cs");
exec("./guessing.cs");
exec("./reputation.cs");
exec("./commands.cs");
exec("./help.cs");

// extra stuff if BLBrowser is present on the server
if(isFile("Add-Ons/Brick_Screen.zip") && !$Pictionary::AWS::Init) {
	// todo: better way of determining this
	$Pictionary::AWS::Init = true;
	exec("./extra/aws.cs");
}

// extra stuff for my own server
if(isFile("Add-Ons/Server_CloudPrefs/server.cs") && !$Pictionary::CloudPrefs::Init) {
	$Pictionary::CloudPrefs::Init = true;
	exec("Add-Ons/Server_CloudPrefs/server.cs");
}

// delete everything upon server shutdown
package PictionaryDestroyPackage {
	function onServerDestroyed() {
		deleteVariables("$Pictionary::*");
		return parent::onServerDestroyed();
	}
};
activatePackage(PictionaryDestroyPackage);

// drawer respawns twice					O
// /autoqueue command						O
// add raycasting to walls
// command shortcuts						O
// need a way to unqueue					O
// players want a way to see the time		O
// afk detection							O
// add a command to forcefully be afk
// brush shape
// /newword still works late into the round	O
// drawing bans
// clear variables on server shutdown		O
// /giveup command
// command to disable music (about:blank)
// show help on spawn
// modded chat isn't resetting the afk timer