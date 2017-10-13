$FallingPlatforms::WaitDelay = 30;
$FallingPlatforms::StartDelay = 5;
$FallingPlatforms::MinDelay = 5000;
$FallingPlatforms::Init::CurrentDelay = 6000;
$FallingPlatforms::AddColorEveryXRounds = 6;
$FallingPlatforms::ShuffleEveryXRounds = mCeil($FallingPlatforms::AddColorEveryXRounds/2);

function resetGameVars() {
	$FallingPlatforms::CurrentDelay = $FallingPlatforms::Init::CurrentDelay;
	$FallingPlatforms::ActivePlayers = 0;
	if($DefaultMinigame.isPBAbove20()) {
		$FallingPlatforms::CurrentColorAmount = 2 + mFloor(10/$FallingPlatforms::AddColorEveryXRounds);
		$FallingPlatforms::Rounds = 10;
		messageAll('', "\c6Everyone's personal best is above 20 rounds, the game will start at round 10.");
	} else {
		$FallingPlatforms::CurrentColorAmount = 2;
		$FallingPlatforms::Rounds = 1;
	}
}

function MinigameSO::isPBAbove20(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			if(%client.roundRecord < 20) {
				return false;
			}
		}
	}

	return true;
}

function MinigameSO::beginPreGame(%this) {
	%delay = $FallingPlatforms::WaitDelay;

	cancel(%this.gameStartSched);
	%this.gameStartSched = %this.schedule(%delay*1000, beginGame);

	messageAll('', "\c6You have" SPC %delay SPC "seconds to join the game! Use \c4/join \c6to join.");

	$FallingPlatforms::Playing = false;
	$FallingPlatforms::AcceptingPlayers = true;

	$FallingPlatforms::ColorOrder = "";
	%x = new GuiTextListCtrl();
	for(%i=0;%i<$FallingPlatforms::ColorMax;%i++) {
		%x.addRow(%i, %i TAB getRandom(-999999, 999999));
	}
	%x.sortNumerical(1, getRandom(0, 1));

	%m = %x.rowCount();
	for(%i=0;%i<%m;%i++) {
		$FallingPlatforms::ColorOrder = trim($FallingPlatforms::ColorOrder TAB getField(%x.getRowText(%i), 0));
	}
	dbg("Set color order as" SPC $FallingPlatforms::ColorOrder, "system.cs");

	doBoardShuffleLoop();
}

function MinigameSO::beginGame(%this) {
	%count = 0;

	dbg("Waiting on players...", "system.cs/step1");

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];	

		if(isObject(%client.player)) {
			%count++;
		}
	}

	if(!%count) {
		%this.beginPreGame();
		return;
	}

	resetGameVars();

	cancel($FallingPlatforms::BoardShuffleLoopSched);

	$FallingPlatforms::AcceptingPlayers = false;
	$FallingPlatforms::ActivePlayers = %count;

	messageAll('MsgAdminForce', "\c6Alright! Let's begin.");
	if(%count == 1) {
		messageAll('', "\c6Only one player? Keep going until you fall out!");
	}

	shuffleBoard($FallingPlatforms::CurrentColorAmount);

	cancel(%this.gameStartSched);
	%this.gameStartSched = %this.schedule($FallingPlatforms::StartDelay*1000, gameLoopStep1);
}

function MinigameSO::gameLoopStep1(%this) {
	cancel(%this.gameStartSched);

	%overall_delay = 0;
	%shuffle = false;

	if(!$FallingPlatforms::Playing) {
		$FallingPlatforms::StartedAt = $Sim::Time;
	}
	$FallingPlatforms::Playing = true;

	%rounds = $FallingPlatforms::Rounds;
	if(%rounds % $FallingPlatforms::AddColorEveryXRounds == 0 && $FallingPlatforms::CurrentColorAmount < $FallingPlatforms::ColorMax) {
		dbg("Adding a color...", "system.cs/step1");
		$FallingPlatforms::CurrentColorAmount++;
		%shuffle = true;
	} else if(%rounds % $FallingPlatforms::ShuffleEveryXRounds == 0) {
		%shuffle = true;
	}

	if(%shuffle) {
		dbg("Shuffling the board...", "system.cs/step1");
		%overall_delay += 1250;
		shuffleBoard($FallingPlatforms::CurrentColorAmount);
	}

	$FallingPlatforms::ActiveColor = getRandom(0, $FallingPlatforms::CurrentColorAmount-1);
	dbg("Set the active color to" SPC $FallingPlatforms::ActiveColor, "system.cs/step1");

	%this.gameStartSched = %this.schedule(%overall_delay, gameLoopStep2);
}

function MinigameSO::gameLoopStep2(%this) {
	cancel(%this.gameStartSched);
	
	dbg("Warning players of active color...", "system.cs/step2");
	boardWarn($FallingPlatforms::ActiveColor);

	%rounds = $FallingPlatforms::Rounds;

	%delay = $FallingPlatforms::CurrentDelay;
	%exp = mPow(%rounds, -0.32);
	%offset = ((%rounds - 1) * (650 * %exp)) + (125 * %rounds);

	if(%offset > 5500) {
		%offset = 5500;
	}

	dbg("%offset set to" SPC %offset @ ".", "system.cs/step2");

	%this.gameStartSched = %this.schedule(%delay - %offset, gameLoopStep3);
}

function MinigameSO::gameLoopStep3(%this) {
	cancel(%this.gameStartSched);

	dbg("Letting the board fall...", "system.cs/step3");
	boardFall($FallingPlatforms::ActiveColor);
	centerPrintAll("", 1);

	if($FallingPlatforms::Rounds < 20) {
		%offset = 700 - (($FallingPlatforms::Rounds-1) * 35);
		if(%offset < 0) {
			%offset = 0;
		}
		%this.gameStartSched = %this.schedule(4800 + %offset, gameLoopStep4);
	} else if($FallingPlatforms::Rounds >= 20 && $FallingPlatforms::Rounds < 40) {
		%offset = ($FallingPlatforms::Rounds - 20) * 50;
		if(%offset > 700) {
			%offset = 700;
		}
		%this.gameStartSched = %this.schedule(3000 - %offset, gameLoopStep4);
	} else {
		%offset = ($FallingPlatforms::Rounds - 40) * 33;
		if(%offset > 550) {
			%offset = 550;
		}
		%this.gameStartSched = %this.schedule(2500 - %offset, gameLoopStep4);		
	}
}

function MinigameSO::gameLoopStep4(%this) {
	cancel(%this.gameStartSched);

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			// players fall out if they stand in an intersection of bricks if this isn't executed, i do not know why
			// this does not affect movement, it's too subtle
			%client.player.addVelocity("0 0 0.1");

			%mult = mCeil($FallingPlatforms::Rounds/20);
			if(%mult > 3) {
				%mult = 3;
			}
			%client.totalScore += ($FallingPlatforms::CurrentColorAmount - 1) * %mult;
			%client.saveData();
			modLeaderboard(%client);
		}
	}

	switch(%this.isOneRemaining()) {
		case 0:
			// more than one left
			dbg("More than 1 player remaining.", "system.cs/step4");

			$FallingPlatforms::Rounds++;

			%this.gameLoopStep1();
			return;

		case 1:
			// one left
			dbg("1 player is remaining.", "system.cs/step4");

			if($FallingPlatforms::ActivePlayers > 1) {
				// winner declared
				dbg("Declaring winner...", "system.cs/step4");

				%winner = %this.getFirstAliveClient();
				messageAll('MsgAdminForce', "\c4" @ %winner.name SPC "\c6is the winner this round! They will receive a\c4" SPC $FallingPlatforms::Rounds * 2 SPC "point bonus!");
				messageAll('', "Starting a new game in 10 seconds...");
				%winner.totalScore += $FallingPlatforms::Rounds * 2;

				if(%winner.roundRecord < $FallingPlatforms::Rounds) {
					%winner.roundRecord = $FallingPlatforms::Rounds;
					messageAll('', "\c4" @ %winner.name SPC "\c6was alive for\c4" SPC %winner.roundRecord SPC "rounds,\c6 a new personal best!");
					modLeaderboard(%winner, "pb");
					%winner.saveData();
				}
			} else if($FallingPlatforms::ActivePlayers == 1) {
				// only one player played anyways, keep going
				dbg("Only 1 player joined.", "system.cs/step4");

				$FallingPlatforms::Rounds++;

				%this.gameLoopStep1();
				return;
			}
			// TODO: determine bonus score

		case -1:
			// no one alive
			dbg("No one is alive.", "system.cs/step4");

			if($FallingPlatforms::ActivePlayers > 1) {
				messageAll('', "\c6No one's around? That's odd. Starting a new game in 10 seconds...");
			} else {
				messageAll('', "\c6Nice job! Starting a new game in 10 seconds...");
			}

	}

	// both 1/-1 should do the same general stuff
	%this.gameStartSched = %this.schedule(10000, gameLoopStep5);
}

function MinigameSO::gameLoopStep5(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%player = %client.player;
		%cam = %client.camera;

		if(%client.getControlObject().getID() != %cam.getID()) {
			%client.setControlObject(%cam);
		}

		if(isObject(%player)) {
			%cam.setTransform(%player.getTransform());
			%player.delete();
		}
	}

	$FallingPlatforms::Playing = false;
	loadLayout(); // <-- loops back to beginPreGame when done, see board.cs
}

function MinigameSO::getFirstAliveClient(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(isObject(%client.player)) {
			return %client;
		}
	}

	return -1;
}

function MinigameSO::isOneRemaining(%this) {
	// blockland does too much with checkLastManStanding by default
	%count = 0;

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		dbg("looking at" SPC %i, "system.cs", 2);

		if(isObject(%client.player)) {
			if(!%count) {
				dbg("%count is " SPC %count @ ", incrementing...", "system.cs", 2);
				%count++;
			} else {
				dbg("More than 1 alive", "system.cs");
				return false;
			}
		}
	}

	if(%count) {
		return true;
	} else {
		// no one is alive
		return -1;
	}
}

package FallingPlatformsSystemPackage {
	function MinigameSO::checkLastManStanding(%this) {
		// preventing blockland from doing default stuff
		return false;
	}

	function PlayerPlatforms::onEnterLiquid(%data,%player,%coverage,%type) {
		%client = %player.client;

		dbg(%client SPC "entered a liquid.", "system.cs");

		if(!$FallingPlatforms::Playing) {
			%player.setVelocity("0 0 0");

			%spawn = BrickGroup_888888.getObject(getRandom(0, BrickGroup_888888.getCount()-1));
			%player.setTransform(vectorAdd("0 0 5", %spawn.getTransform()));
		} else {
			%player.kill();

			%mg = $DefaultMinigame;
			for(%i=0;%i<%mg.numMembers;%i++) {
				%t = %mg.member[%i];
				if(isObject(%t.player)) {
					%t.totalScore += 5;
				}
			}

			if(%client.roundRecord < $FallingPlatforms::Rounds) {
				%client.roundRecord = $FallingPlatforms::Rounds;
				messageAll('', "\c4" @ %client.name SPC "\c6was alive for\c4" SPC %client.roundRecord SPC "rounds,\c6 a new personal best!");
				modLeaderboard(%client, "pb");
				%client.saveData();
			}
		}
		
		return parent::onEnterLiquid(%data,%player,%coverage,%type);
	}
};
activatePackage(FallingPlatformsSystemPackage);