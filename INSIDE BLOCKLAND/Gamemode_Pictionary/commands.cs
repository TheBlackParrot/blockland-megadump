function GameConnection::checkCooldown(%this, %cmd, %secs) {
	%cooldown = %this.lastUsedCommand[%cmd];

	if($Sim::Time - %cooldown < %secs) {
		return false;
	}

	%this.lastUsedCommand[%cmd] = $Sim::Time;
	return true;
}

function serverCmdQueue(%client) {
	if(!%client.checkCooldown("queue", 2)) {
		return;
	}

	%queue = $Pictionary::DrawerQueue;
	for(%i=0;%i<getFieldCount(%queue);%i++) {
		if(%client.bl_id == getField(%queue, %i)) {
			%client.playSound(errorSound);
			messageClient(%client, '', "\c0You are already in the queue!");
			return;
		}
	}

	$Pictionary::DrawerQueue = trim($Pictionary::DrawerQueue TAB %client.bl_id);
	messageClient(%client, '', "\c2You are now in queue at position" SPC getFieldCount($Pictionary::DrawerQueue));

	if(getFieldCount($Pictionary::DrawerQueue) == 1 && !$Pictionary::CanGuess) {
		cancel($DefaultMinigame.mainSched);
		$DefaultMinigame.nextRound();
	}
}

//temp
function serverCmdStartGame(%client) {
	if(%client.isSuperAdmin) {
		$DefaultMinigame.nextRound();
	}
}

function serverCmdSkip(%client) {
	if(!%client.isAdmin) {
		return;
	}
	if(!$Pictionary::CanGuess) {
		return;
	}

	messageAll('', "\c4" @ %client.name SPC "\c6has forcefully ended the round.");
	$DefaultMinigame.endRound();
}

function serverCmdViewQueue(%client) {
	if(!%client.checkCooldown("viewQueue", 2)) {
		return;
	}

	%queue = $Pictionary::DrawerQueue;
	%count = 1;

	for(%i=0;%i<getFieldCount(%queue);%i++) {
		%bl_id = getField(%queue, %i);
		%who = findClientByBL_ID(%bl_id);

		if(isObject(%who)) {
			messageClient(%client, '', "\c4" @ %count @ ".\c6" SPC %who.name);
			%count++;
		}
	}
}

function serverCmdNewWord(%client) {
	if(!%client.canDraw) {
		return;
	}

	if(!%client.checkCooldown("newWord", 2)) {
		return;
	}

	if($Sim::Time > ($Pictionary::RoundDuration/1000/4) + $Pictionary::StartedDrawingAt) {
		messageClient(%client, '', "\c0You cannot request a new word this late into the round.");
		%client.playSound(errorSound);
		return;
	}

	messageAll('', "\c4The drawer has requested a new word.");
	$Pictionary::CurrentWord = PictionaryWordDB.getNextEntry();
}

function serverCmdWord(%client) {
	if(!%client.isAdmin || !$Pictionary::CanGuess) {
		return;
	}

	if(!%client.guessed) {
		dbg("An admin needed to view the current word, they can no longer guess.", "commands.cs");
		%client.guessed = true;
	}

	messageClient(%client, '', "\c4" @ strReplace($Pictionary::CurrentWord.words, "\t", "/"));
}

function serverCmdAutoQueue(%client) {
	if(!%client.checkCooldown("autoqueue", 1)) {
		return;
	}

	if(%client.autoqueue) {
		%client.autoqueue = false;
	} else if(!%client.autoqueue) {
		%client.autoqueue = true;
	}

	messageClient(%client, '', "\c6You will" SPC (%client.autoqueue ? "now" : "no longer") SPC "be queued automatically at the end of the round.");
}

function serverCmdUnqueue(%client) {
	if(!%client.checkCooldown("unqueue", 2)) {
		return;
	}

	%queue = $Pictionary::DrawerQueue;
	%found = -1;

	for(%i=0;%i<getFieldCount(%queue);%i++) {
		if(%client.bl_id == getField(%queue, %i)) {
			%found = %i;
			break;
		}
	}

	if(%found == 0) {
		$Pictionary::DrawerQueue = getFields($Pictionary::DrawerQueue, 1);
	} else {
		$Pictionary::DrawerQueue = trim(getFields($Pictionary::DrawerQueue, 0, %i-1) TAB getFields($Pictionary::DrawerQueue, %i+1));
	}
	messageClient(%client, '', "\c3You are no longer in the queue.");
}

function serverCmdModRep(%client, %target_v, %amnt) {
	if(!%client.isAdmin) {
		return;
	}

	%target = findClientByBL_ID(%target_v);
	if(!isObject(%target)) {
		%target = findClientByName(%target_v);
		if(!isObject(%target)) {
			messageClient(%client, '', "That user does not exist.");
			return;
		}
	}

	%target.modRep(%amnt);
	messageClient(%target, '', "\c0An admin has modified your reputation score by" SPC %amnt SPC "points.");
	messageClient(%client, '', "done");
}

// shortcuts
function serverCmdQ(%client) { serverCmdQueue(%client); }
function serverCmdDraw(%client) { serverCmdQueue(%client); }

function serverCmdLike(%client) {
	if(%client.canDraw || !$Pictionary::CanGuess) {
		return;
	}

	if(%client.disliked) {
		messageClient(%client, '', "\c6You can't like a drawing you already dislike.");
		return;
	}

	if(%client.liked) {
		messageClient(%client, '', "\c6You no longer like this drawing.");
		%client.liked = false;
	} else {
		messageClient(%client, '', "\c6You liked this drawing.");
		%client.liked = true;
	}
}

function serverCmdDislike(%client) {
	if(%client.canDraw || !$Pictionary::CanGuess) {
		return;
	}

	if(%client.liked) {
		messageClient(%client, '', "\c6You can't dislike a drawing you already like.");
		return;
	}

	if(%client.disliked) {
		messageClient(%client, '', "\c6You no longer dislike this drawing.");
		%client.disliked = false;
	} else {
		messageClient(%client, '', "\c6You disliked this drawing.");
		%client.disliked = true;
	}	
}