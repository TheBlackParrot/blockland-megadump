$FallingPlatforms::Debug = false;
$FallingPlatforms::DebugLevel = 3;

function dbg(%msg, %file, %lvl, %echo_only) {
	if(!$FallingPlatforms::Debug) {
		return;
	}

	if(%lvl $= "" || %lvl <= 0) {
		%lvl = 1;
	}

	if(%lvl > $FallingPlatforms::DebugLevel) {
		return;
	}

	echo("\c1[" @ $Sim::Time @ "] \c4[DEBUG] \c5[" @ %file @ "] \c6" @ %msg);
	if(!%echo_only || %echo_only $= "") {
		messageAll('', "\c7[" @ $Sim::Time @ "] \c1[DEBUG] \c5[" @ %file @ "] \c6" @ strReplace(%msg, "\t", "^"));
	}
}

datablock PlayerData(PlayerPlatforms : PlayerStandardArmor) {
	airControl = 0.3;
	jumpForce = 850;
	maxForwardSpeed = 12;
	maxSideSpeed = 12;
	maxBackwardSpeed = 12;
	runForce = 8000;
	groundImpactShakeAmp = "0.5 0.5 0.5";
	groundImpactMinSpeed = 13;
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	uiName = "Platforms Player";
	showEnergyBar = false;
};

exec("./support.cs");
exec("./board.cs");
exec("./system.cs");
exec("./commands.cs");
exec("./sounds.cs");
exec("./stats.cs");
exec("./saving.cs");
exec("./leaderboard.cs");
exec("./cheat.cs");

exec("./lib/rgb2hex.cs");

if($FallingPlatforms::InitLoadLayout $= "") {
	$FallingPlatforms::InitLoadLayout = true;
	loadLayout();
}

// actual game lel					X
// special rounds					X
//		follow the text, not color	X
//		follow the color, not text	X
//		crumbling arena				X
//		color falls instead			X
//		random selection falls		X
// /join command					X