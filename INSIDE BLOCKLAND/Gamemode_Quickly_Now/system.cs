function MinigameSO::preGameLoop(%this) {
	cancel($QuicklyNow::GameSched);

	$QuicklyNow::CanShoot = false;
	$QuicklyNow::InProgress = false;
	$QuicklyNow::AcceptingPlayers = true;
	$QuicklyNow::Mode = 0;

	%count = 0;
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			if(%client.inGame) {
				%count++;
			}
		}
	}

	switch(%count) {
		case 0:
			messageAll('', "\c6No one has joined the game! Use \c4/join \c6to join.");
			$QuicklyNow::GameSched = %this.schedule(15000, preGameLoop);
			return;

		default:
			if(%count == 1) {
				$QuicklyNow::Mode = 1;
				$QuicklyNow::Rounds = 0;
			}
			messageAll('', "\c6Alright, let's begin!");
			$QuicklyNow::GameSched = %this.schedule(5000, gameStep1);
			$QuicklyNow::AcceptingPlayers = false;
	}
}

function MinigameSO::gameStep1(%this) {
	cancel($QuicklyNow::GameSched);
	cancel($QuicklyNow::TickTockSched);

	cancel($QuicklyNow::EliminateAllNonHitSched);

	$QuicklyNow::CanShoot = false;
	$QuicklyNow::InProgress = true;

	%count = 0;
	%dir = getRandom(-5, 5);
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		if(isObject(%client.player)) {
			%client.player.setTransform(%count*10 SPC "0 15 1 1 1" SPC %dir);
			%client.dummyCamera.setTransform(%client.player.getTransform());
			%client.setControlObject(%client.dummyCamera);

			if(%client.inGame) {
				%count++;
				%client.didHit = false;
			} else {
				%client.player.delete(); // should never trigger, hopefully
			}
		}
	}

	if($QuicklyNow::Mode == 0) {
		if(%count < 2) {
			%reset = 1;
		} else {
			regenCubes(%count - 1);
		}
	} else if($QuicklyNow::Mode == 1) {
		$QuicklyNow::Rounds++;

		if(%count < 1) {
			%reset = 1;
		} else {
			%amount = $QuicklyNow::Rounds * 2;
			if(%amount > 128) {
				%amount = 128;
			}
			regenCubes(%amount);
		}
	}

	if(%reset) {
		messageAll('', "\c6Not enough players are present to continue. Resetting...");
		$QuicklyNow::GameSched = %this.schedule(5000, gameReset);

		for(%i=0;%i<%this.numMembers;%i++) {
			%client = %this.member[%i];
			%client.inGame = false;

			if(isObject(%client.player)) {
				%client.player.delete();
			}
		}		
	}

	$QuicklyNow::GameSched = %this.schedule(1000, gameStep2);
}

function MinigameSO::gameStep2(%this) {
	cancel($QuicklyNow::GameSched);

	$QuicklyNow::CanShoot = false;

	if($QuicklyNow::Mode == 1) {
		messageAll('', "\c6Round\c4" SPC $QuicklyNow::Rounds);
	}

	bottomPrintAll("<just:center><color:ffffff><font:Impact:36>5", 2);
	serverPlay2D(tick);
	schedule(1000, 0, bottomPrintAll, "<just:center><color:ffffff><font:Impact:36>4", 2);
	schedule(2000, 0, bottomPrintAll, "<just:center><color:ffffff><font:Impact:36>3", 2);
	schedule(3000, 0, bottomPrintAll, "<just:center><color:ffffff><font:Impact:36>2", 2);
	schedule(4000, 0, bottomPrintAll, "<just:center><color:ffffff><font:Impact:36>1", 2);

	for(%i=1000;%i<5000;%i+=1000) {
		schedule(%i, 0, serverPlay2D, tick);
	}

	$QuicklyNow::GameSched = %this.schedule(5000, gameStep3);
}

function MinigameSO::gameStep3(%this) {
	cancel($QuicklyNow::GameSched);

	bottomPrintAll("<just:center><color:00ffff><font:Impact:36>BEGIN", 2);
	serverPlay2D(start);

	$QuicklyNow::CanShoot = true;

	$QuicklyNow::EliminateAllNonHitSched = %this.schedule(15000, eliminateAllNonHit);

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(isObject(%client.player)) {
			if(%client.inGame) {
				%client.setControlObject(%client.player);
			}
		}
	}

	if($QuicklyNow::Mode == 1) {
		$QuicklyNow::Ticks = 4 + mCeil($QuicklyNow::Rounds * 0.8);
		%this.tickTock();
	}
}

function MinigameSO::eliminateAllNonHit(%this) {
	cancel($QuicklyNow::EliminateAllNonHitSched);

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(isObject(%client.player)) {
			if(%client.inGame) {
				if(!%client.didHit) {
					%client.player.kill();
					messageClient(%client, '', "\c6You did not snap a brick in time!");
				}
			}
		}
	}
}

function MinigameSO::amountInGame(%this) {
	%count = 0;
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(%client.inGame) {
			%count++;
		}
	}

	return %count;
}

function MinigameSO::getLastAlive(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		if(isObject(%client.player)) {
			if(%client.inGame) {
				return %client;
			}
		}
	}

	return -1;
}

function MinigameSO::gameStep4(%this) {
	cancel($QuicklyNow::GameSched);

	$QuicklyNow::CanShoot = false;

	%last = %this.getLastAlive();
	if(%last == -1) {
		messageAll('', "No one is alive, stopping the game...");
		%this.gameReset();
		return;
	}

	switch(%this.amountInGame()) {
		case 1:
			if($QuicklyNow::Mode == 0) {
				messageAll('', "\c4" @ %last.name SPC "\c6is the winner! (TODO: score crap/etc)");
				messageAll('', "\c6Resetting in 5 seconds...");
				
				$QuicklyNow::GameSched = %this.schedule(5000, gameReset);
			} else if($QuicklyNow::Mode == 1) {
				%this.gameStep1();
			}

		case 0:
			messageAll('', "No one is alive, stopping the game...");
			%this.gameReset();
			return;

		default:
			%this.gameStep1();
	}
}

function MinigameSO::gameReset(%this) {
	cancel($QuicklyNow::GameSched);
	cancel($QuicklyNow::TickTockSched);

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		%client.inGame = false;

		if(isObject(%client.player)) {
			%client.camera.setTransform(%client.player.getHackPosition());
			%client.setControlObject(%client.camera);
			%client.player.delete();
		}
	}

	regenCubes(getRandom(64, 128)); // just for show
	%this.preGameLoop();
}

function MinigameSO::tickTock(%this) {
	cancel($QuicklyNow::TickTockSched);
	$QuicklyNow::TickTockSched = %this.schedule(1000, tickTock);

	bottomPrintAll("<just:center><color:ffffff><font:Impact:36>" @ $QuicklyNow::Ticks, 2);
	serverPlay2D(tick);

	if($QuicklyNow::Ticks == 0) {
		%this.eliminateAllNonHit();
		cancel($QuicklyNow::TickTockSched);
		cancel($QuicklyNow::GameSched);

		messageAll('', "\c6You lasted" SPC $QuicklyNow::Rounds SPC "rounds!");
		%this.schedule(5000, gameReset);
	}

	$QuicklyNow::Ticks--;
}

package QuicklyNowPackage {
	function MinigameSO::checkLastManStanding(%this) {
		return;
	}

	function gameConnection::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea) {
		if($QuicklyNow::InProgress) {
			BrickGroup_888888.getObject(getRandom(0, BrickGroup_888888.getCount()-1)).delete();
		}
		%this.inGame = false;
		%this.isSpectator = true;

		if($DefaultMinigame.amountInGame() == 0) {
			cancel($QuicklyNow::EliminateAllNonHitSched);
			cancel($QuicklyNow::GameSched);
		}
		return parent::onDeath(%this,%sourceObject,%sourceClient,%damageType,%damageArea);
	}
};
activatePackage(QuicklyNowPackage);