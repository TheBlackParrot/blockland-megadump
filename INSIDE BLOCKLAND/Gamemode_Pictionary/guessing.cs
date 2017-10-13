// modded from the original version
// it too was my code so bugger off lol

function GameConnection::guessWord(%this, %guess) {
	%guess = strLwr(stripChars(%guess, "!@#$%^&*()_+-={}|[]\\:\";',./<>?~`0123456789"));
	%correctAll = trim($Pictionary::CurrentWord.words TAB $Pictionary::CurrentWord.plurals TAB $Pictionary::CurrentWord.nospaces);
	%mg = $DefaultMinigame;

	for(%i=0;%i<getFieldCount(%correctAll);%i++) {
		%correct = getField(%correctAll, %i);
		if(stripos(%guess, %correct) != -1) {
			if(%this.canDraw) {
				return 1;
			}

			if(!%this.guessed) {
				%this.guessed = 1;
				messageClient(%this, '', "\c2CORRECT! \c6The word was \c3" @ strReplace($Pictionary::CurrentWord.words, "\t", "/") @ "\c6.");
				%this.play2D(errorSound);
				messageAll('', "\c2" @ %this.name SPC "\c6guessed the word!");

				%this.modRep(1);
				%mg.currentDrawer.modRep(1);

				%mg.checkGuessed();
			}
			return 1;
		}
	}
	return 0;
}

function MinigameSO::checkGuessed(%this) {
	for(%i=0;%i<ClientGroup.getCount();%i++) {
		%client = ClientGroup.getObject(%i);
		if(%client.canDraw) {
			continue;
		}

		if(!%client.afk) {
			if(!%client.guessed) {
				return;
			}
		}
	}

	cancel(%this.mainSched);
	%this.endRound();
}

function GameConnection::spamLoop(%this) {
	cancel(%this.spamSched);
	%this.spamSched = %this.schedule(2000, spamLoop);

	%this.msgCount--;
	if(%this.msgCount < 0) {
		%this.msgCount = 0;
	}
}

package PictionaryGuessingPackage {
	function serverCmdMessageSent(%client, %msg) {
		if(%client.rep == 0) { %client.clanPrefix = "\c7[\c6+0\c7]" SPC %client.oldPrefix; }
		else if(%client.rep < 0) { %client.clanPrefix = "\c7[\c0" @ %client.rep @ "\c7]" SPC %client.oldPrefix; }
		else if(%client.rep > 0) { %client.clanPrefix = "\c7[\c2+" @ %client.rep @ "\c7]" SPC %client.oldPrefix; }

		if(isObject(%client.player)) {
			dbg("Message was sent out, should've reset AFK timer on" SPC %client, "afk.cs", 3);
			%client.player.resetAFK();
		}

		if(!$Pictionary::CanGuess) {
			return parent::serverCmdMessageSent(%client, %msg);
		}

		%format = '\c7%1\c3%2\c7%3\c6: %4';

		%firstWord = getWord(%msg, 0);
		if(%client.isAdmin && stripos(%firstWord, "@d") != -1) {
			for(%i=0;%i<ClientGroup.getCount();%i++) {
				%tmp = ClientGroup.getObject(%i);
				if(%tmp.canDraw) {
					%tmp.playSound(errorSound);
					commandToClient(%tmp, 'chatMessage', %tmp, '', '', %format, "\c7[\c2ADMIN->\c4DRAWER\c7]" SPC %client.clanPrefix, %client.name, %client.clanSuffix, "<color:cccccc>" @ stripMLControlChars(%msg));
				}
			}
			commandToClient(%client, 'chatMessage', %tmp, '', '', %format, "\c7[\c2ADMIN->\c4DRAWER\c7]" SPC %client.clanPrefix, %client.name, %client.clanSuffix, "<color:cccccc>" @ stripMLControlChars(%msg));
			return;
		}

		if(%client.guessed) {
			if(%client.lastMsg $= %msg) {
				return;
			}
			%client.lastMsg = %msg;

			%client.msgCount++;
			if(%client.msgCount > 3) {
				return;
			}

			for(%i=0;%i<ClientGroup.getCount();%i++) {
				%tmp = ClientGroup.getObject(%i);
				if(%tmp.guessed || %tmp.canDraw || %tmp.isAdmin) {
					commandToClient(%tmp, 'chatMessage', %tmp, '', '', %format, "\c7[\c0GUESSED\c7]" SPC %client.clanPrefix, %client.name, %client.clanSuffix, "<color:cccccc>" @ stripMLControlChars(%msg));
				}
			}
			return;
		}
		
		if(!isEventPending(%client.spamSched)) {
			%client.spamLoop();
		}

		%msg = stripMLControlChars(%msg);

		if(%client.canDraw) {
			//messageClient(%client, '', "\c4[DRAWER] \c3" @ %client.name @ "\c6: " @ %msg);
			%format = '\c7%1\c3%2\c7%3\c6: %4';
			for(%i=0;%i<ClientGroup.getCount();%i++) {
				%tmp = ClientGroup.getObject(%i);
				if(%tmp.isAdmin || %tmp.guessed || %tmp == %client) {
					// for now
					commandToClient(%tmp, 'chatMessage', %tmp, '', '', %format, "\c7[\c4DRAWER\c7]" SPC %client.clanPrefix, %client.name, %client.clanSuffix, stripMLControlChars(%msg));

					if(%client.guessWord(strLwr(%msg))) {
						dbg("Admins can no longer guess, the word was revealed through drawer chat.", "guessing.cs");
						for(%j=0;%j<ClientGroup.getCount();%j++) {
							%jj = ClientGroup.getObject(%j);
							if(%jj.isAdmin) {
								%jj.guessed = true;
							}
						}
					}
				}
			}
			return;
		}

		if(!%client.guessWord(%msg)) {
			return parent::serverCmdMessageSent(%client, %msg);
		}
	}

	function serverCmdTeamMessageSent(%client, %msg) {
		return;
	}
};
activatePackage(PictionaryGuessingPackage);