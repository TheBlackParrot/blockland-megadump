function MinigameSO::getFirstAlive(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%player = %client.player;

		if(isObject(%player)) {
			return %player;
		}
	}

	return -1;
}

function MinigameSO::checkAmountRemaining(%this) {
	%alive = 0;

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%player = %client.player;

		if(isObject(%player)) {
			%alive++;
		}
	}

	return %alive;
}

function MinigameSO::checkForWinner(%this) {
	dbg("Checking for a winner...", "system.cs", "checkForWinner", %this);

	%remain = %this.checkAmountRemaining();
	%reset = false;

	dbg(%remain SPC "players remain.", "system.cs", "checkForWinner", %this);

	if($CrumblingArena::OldAmountRemaining !$= "") {
		if($CrumblingArena::OldAmountRemaining == %remain && %remain > 1) {
			return;
		}
	}
	$CrumblingArena::OldAmountRemaining = %remain;

	switch(%remain) {
		case 1:
			%winner = %this.getFirstAlive().client;
			messageAll('', "\c3" @ %winner.name SPC "\c5has won this round! Resetting in 10 seconds...");
			%reset = true;

		case 0:
			messageAll('', "\c5No one has won this round. Resetting in 10 seconds...");
			%reset = true;

		default:
			messageAll('', "\c5" @ %remain SPC "players remain...");
	}

	if(%reset) {
		%this.resetVariables();

		dbg("Game will reset.", "system.cs", "checkForWinner", %this);

		cancel($CrumblingArena::GameSched);
		$CrumblingArena::GameSched = %this.schedule(10000, reset);
		$CrumblingArena::Playing = false;
	}
}

function MinigameSO::onBoardFinishGen(%this) {
	dbg("Board has finished generating.", "system.cs", "onBoardFinishGen", %this);
	$CrumblingArena::CanJoin = true;

	%this.preGameLoop();

	$CrumblingArena::CurrentEnvironment = getRandom(1, 11);
	%filename = $CrumblingArena::Root @ "/environment/" @ $CrumblingArena::CurrentEnvironment @ ".txt";
	dbg("Using" SPC %filename SPC "for the environment.", "system.cs", "onBoardFinishGen", %this);
	loadEnvironment(%filename);
}

function MinigameSO::preGameLoop(%this) {
	dbg("Running pre-game loop...", "system.cs", "preGameLoop", %this);
	cancel($CrumblingArena::GameSched);

	%amount_joined = 0;
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			%amount_joined++;
		}
	}

	if(!%amount_joined) {
		dbg("No one has joined. Running the loop again...", "system.cs", "preGameLoop", %this);
		messageAll('', "\c6No one has joined! Use \c4/join \c6to join the game.");
		$CrumblingArena::GameSched = %this.schedule(30000, preGameLoop);
		return;
	}

	dbg(%amount_joined SPC "players have joined the game.", "system.cs", "preGameLoop", %this);
	messageAll('', "\c6Nice! Starting the game in 5 seconds...");

	$CrumblingArena::CanJoin = false;
	$CrumblingArena::PlayerCount = %amount_joined;
	$CrumblingArena::Bricks::FallDelay = 1500;

	%this.gameStep1();
}

function MinigameSO::gameStep1(%this) {
	cancel($CrumblingArena::GameSched);
	$CrumblingArena::GameSched = %this.schedule(5000, gameStep2);

	dbg("Step 1 running...", "system.cs", "gameStep1", %this);

	for(%i=0;%i<5;%i++) {
		schedule(%i*1000, 0, centerPrintAll, "<font:Impact:52><color:ffff00>" @ 5 - %i, 2);
		schedule(%i*1000, 0, serverPlay2D, CA_countdownSound);
	}
}

function MinigameSO::gameStep2(%this) {
	dbg("Step 2 running...", "system.cs", "gameStep2", %this);
	
	centerPrintAll("<font:Impact:64><color:00ff00>GO!", 2);
	serverPlay2D(CA_goSound);
	
	$CrumblingArena::Playing = true;

	$CrumblingArena::GameSched = %this.schedule(30000, gameStep3);
}

function MinigameSO::gameStep3(%this) {
	cancel($CrumblingArena::GameSched);
	$CrumblingArena::GameSched = %this.schedule(20000, gameStep3);

	dbg("Step 3 running...", "system.cs", "gameStep3", %this);

	%this.resetVariables();

	%this.setItem(PrintGun, 1);
	%this.setItem("", 0);

	%mode = getRandom(0, 6);
	while(%mode == $CrumblingArena::LastMode) {
		%mode = getRandom(0, 6);
	}
	$CrumblingArena::LastMode = %mode;

	%formatting = "<font:Impact:40>";

	switch(%mode) {
		case 0:
			messageAll('MsgAdminForce', %formatting @ "\c4Guns\c6 have been given out!");
			%this.setItem(GunItem, 0);

		case 1:
			messageAll('MsgAdminForce', %formatting @ "\c4Swords\c6 have been given out!");
			%this.setItem(SwordItem, 0);

		case 2:
			messageAll('MsgAdminForce', %formatting @ "\c4Push Brooms\c6 have been given out!");
			%this.setItem(PushbroomItem, 0); 

		case 3:
			messageAll('MsgAdminForce', %formatting @ "\c4Spears \c6have been given out!");
			%this.setItem(SpearItem, 0);

		case 4:
			messageAll('MsgAdminForce', %formatting @ "\c6It's a bit dark isn't it?");
			loadEnvironment($CrumblingArena::Root @ "/environment/dark.txt");

		case 5:
			messageAll('MsgAdminForce', %formatting @ "\c6The bricks have become reactive! More will fall around you!");
			$CrumblingArena::ExplodingBricks = true;

		case 6:
			messageAll('MsgAdminForce', %formatting @ "\c6Click the bricks to make them fall!");
			$CrumblingArena::Spleef = true;
	}
}

function MinigameSO::resetVariables(%this) {
	dbg("Resetting variables...", "system.cs", "resetVariables", %this);

	$CrumblingArena::ExplodingBricks = false;
	$CrumblingArena::Spleef = false;

	%filename = $CrumblingArena::Root @ "/environment/" @ $CrumblingArena::CurrentEnvironment @ ".txt";
	loadEnvironment(%filename);
}

function updateFallDelay() {
	$CrumblingArena::Bricks::FallDelay = mInterpolate(0, 1500, $DefaultMinigame.checkAmountRemaining() / $CrumblingArena::PlayerCount);
	dbg("Fall delay on bricks was set to" SPC $CrumblingArena::Bricks::FallDelay, "system.cs", "updateFallDelay");
}

deactivatePackage(CrumblingArenaPackage);
package CrumblingArenaPackage {
	function MinigameSO::reset(%this) {
		dbg("Minigame should be resetting...", "system.cs", "reset", %this);

		%width = %length = 10 + (mFloor(%this.numMembers/1.5)-1);
		if(%width > 32) { %width = 32; }
		if(%length > 32) { %length = 32; }
		%height = getRandom((%this.numMembers > 3 ? 3 : 1), 15);

		%allowed = $Pref::Server::CrumblingArena::AllowedBricks;
		%db = getWord(%allowed, getRandom(0, getWordCount(%allowed)-1));

		newBoard(%width, %length, %height, %db);

		for(%i=0;%i<%this.numMembers;%i++) {
			%client = %this.member[%i];
			if(isObject(%client.player)) {
				%client.setControlObject(%client.camera);
				%client.player.delete();
			}
		}

		return parent::reset(%this);
	}

	function MinigameSO::checkLastManStanding(%this) {
		if($CrumblingArena::Init $= "") {
			$CrumblingArena::Init = 1;
			%this.reset();
		}
		return;
	}

	function gameConnection::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea) {
		%r = parent::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea);
		dbg("Player died.", "system.cs", "onDeath", %this);
		
		if($CrumblingArena::Playing) {
			$DefaultMinigame.checkForWinner();
			updateFallDelay();
		}

		return %r;
	}

	function gameConnection::onClientLeaveGame(%this) {
		%r = parent::onClientLeaveGame(%this);

		if($CrumblingArena::Playing) {
			$DefaultMinigame.checkForWinner();
			updateFallDelay();
		}

		return %r;
	}
};
activatePackage(CrumblingArenaPackage);