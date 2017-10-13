$Pictionary::RoundDuration = 120; //seconds
$Pictionary::RoundDuration = $Pictionary::RoundDuration*1000;

function MinigameSO::noOneInQueue(%this) {
	messageAll('', "\c6No one is in the queue to draw! Use \c4/queue \c6to queue.");
	%this.mainSched = %this.schedule(30000, nextRound);
}

function MinigameSO::nextRound(%this) {
	if(getFieldCount($Pictionary::DrawerQueue) == 0) {
		%this.noOneInQueue();
		return;
	}

	dbg("Round has started.", "system.cs", 2);

	fillBoard();

	$Pictionary::CurrentWord = PictionaryWordDB.getNextEntry();

	%client = -1;
	while(!isObject(%client)) {
		%next = getField($Pictionary::DrawerQueue, 0);
		dbg("BL_ID" SPC %next SPC "was chosen.", "system.cs");
		%client = findClientByBL_ID(%next);
		
		if(getFieldCount($Pictionary::DrawerQueue) == 0) {
			%this.noOneInQueue();
			return;
		}

		$Pictionary::DrawerQueue = getFields($Pictionary::DrawerQueue, 1);

		// lol
		if(isObject(%client)) {
			if(%client.afk) {
				messageClient(%client, '', "\c0You were skipped because the server detected you as being AFK.");
				%client = -1;
			}
		}
	}
	%this.setDrawer(%client);

	$Pictionary::StartedDrawingAt = $Sim::Time;
	cancel(%this.mainSched);
	%this.mainSched = %this.schedule($Pictionary::RoundDuration, endRound);

	$Pictionary::CanGuess = true;

	$Pictionary::DrawingSaved = false;
}

function MinigameSO::setDrawer(%this, %client) {
	dbg("Client" SPC %client SPC "was set as the drawer.", "system.cs");
	if(isObject(%this.currentDrawer)) {
		%old = %this.currentDrawer;
		%old.canDraw = false;
		cancel(%old.drawerLoopSched);
	}

	if(!isObject(%client.player)) {
		%client.spawnPlayer();
	}
	%client.player.setTransform(_painterSpawn.getTransform());
	%client.canDraw = true;
	for(%i=0;%i<3;%i++) { %client.schedule(%i*200, playSound, errorSound); }
	%client.drawerLoop();
	%client.setActiveColor(58);
	%client.setBrushRadius(1);

	%this.currentDrawer = %client;

	messageAll('', "\c4" @ %client.name SPC "\c6is the drawer this round!");
}

function GameConnection::drawerLoop(%this) {
	cancel(%this.drawerLoopSched);
	%this.drawerLoopSched = %this.schedule(1000, drawerLoop);

	%words = strReplace($Pictionary::CurrentWord.words, "\t", "/");
	%remain = mFloor(mAbs($Sim::Time - ($Pictionary::StartedDrawingAt + ($Pictionary::RoundDuration/1000))));
	%this.bottomPrint("\c6YOUR WORD IS:<just:right>\c2" @ getTimeString(%remain) @ "<br><just:left>\c3" @ %words, 2, 1);

	switch(strLen(%remain)) {
		case 3:
			_time0.setPrintCount(getSubStr(%remain, 0, 1));
			_time1.setPrintCount(getSubStr(%remain, 1, 1));
			_time2.setPrintCount(getSubStr(%remain, 2, 1));	

		case 2:
			_time0.setPrintCount(0);
			_time1.setPrintCount(getSubStr(%remain, 0, 1));
			_time2.setPrintCount(getSubStr(%remain, 1, 1));	

		case 1:
			_time0.setPrintCount(0);
			_time1.setPrintCount(0);
			_time2.setPrintCount(getSubStr(%remain, 0, 1));				
	}
}

function MinigameSO::endRound(%this) {
	dbg("Round has ended.", "system.cs", 2);
	%client = %this.currentDrawer;

	if(%client.autoqueue) {
		if(!%client.afk) {
			serverCmdQueue(%client);
		} else {
			messageClient(%client, '', "\c6You were not automatically queued because you are considered AFK.");
		}
	}

	cancel(%this.mainSched);

	%rep = %this.determineExtraRep();
	%client.modRep(%rep);

	%liked = "";
	%disliked = "";

	for(%i=0;%i<ClientGroup.getCount();%i++) {
		%t = ClientGroup.getObject(%i);

		if(!%t.afk && !%t.canDraw && !%t.guessed && %rep > 0) {
			%t.modRep(-1);
		}

		%t.guessed = false;

		if(%t.liked) {
			%liked = trim(%liked TAB %t.name);
		}
		if(%t.disliked) {
			%disliked = trim(%disliked TAB %t.name);
		}

		%t.liked = false;
		%t.disliked = false;
	}

	%client.canDraw = false;
	%client.instantRespawn();

	$Pictionary::CanGuess = false;

	$Pictionary::DrawingSaved = false;

	%words = strReplace($Pictionary::CurrentWord.words, "\t", "/");
	messageAll('', "\c6This round has ended! The word was \c3" @ %words);
	messageAll('', "\c6The next round will start in 10 seconds...");
	messageAll('', "\c4" @ %client.name SPC "\c6will receive a" SPC %rep SPC "point modification to their reputation score.");
	if(%liked !$= "") {
		messageAll('', "\c4" @ strReplace(%liked, "\t", ", ") SPC "\c2liked\c6 this drawing.");
	}
	if(%disliked !$= "") {
		messageAll('', "\c4" @ strReplace(%disliked, "\t", ", ") SPC "\c0disliked\c6 this drawing.");
	}

	%this.mainSched = %this.schedule(10000, nextRound);
}