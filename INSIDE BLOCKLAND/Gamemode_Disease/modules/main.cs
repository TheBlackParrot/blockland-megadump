function Player::cough(%this)
{
	cancel(%this.cough);
	
	if(%this.getState() $= "Dead")
	{
		return;
	}
	
	%sound = "Disease_Sound_cough_" @ getRandom(1, 3);
	%this.playAudio(1, %sound);
	
	%this.spawnExplosion(diseaseSpore, 1);
	%this.playthread(1, plant);
	%this.playThread(2, armreadyboth);
	
	%this.cough = %this.schedule(getRandom(3000, 7600), cough);
}

function Disease_MegaAnnounce(%msg)
{
	MessageAll('MsgAdminForce', "\c5" @ %msg);
	CenterPrintAll("\c5" @ %msg);
	BottomPrintAll("\c5" @ %msg);
}

function Player::infect(%this, %infected)
{
	%client = %this.client;
	
	if(%infected $= "START" && !isObject(%client))
	{
		schedule(750, 0, Disease_selectOrigin, 1);
		return;
	}
	
	if(!isObject(%client) || (%infected !$= "START" && !isObject(%infected)))
	{
		return;
	}
	
	%this.isInfected = true;
	if(%infected $= "START")
	{
		%this.isDiseaseOrigin = true;
		%this.setDatablock(PlayerPatientZero);
		MessageAll('', "\c3" @ %client.name @ "\c0 has been selected as the \c3Patient Zero\c0.");
	}
	else
	{
		MessageAll('', "\c3" @ %infected.name @ "\c0 has spread the disease to \c3" @ %client.name @ "\c0.");
		%infected.addScore(2, "Spreading the disease");
	}
	
	ServerPlay3D(Disease_Sound_note_haunt, %this.getHackPosition());
	commandToClient(%client, 'SetVignette', 0, "54 128 20 223");
	centerPrint(%client, "<font:palatino linotype:40>You have become infected with the disease.<br><color:FFFF00><font:palatino linotype:25>Click healthy players to infect them.", 5);
	
	Disease_checkForEnd();
	%this.setshapenamecolor("1 0 0");
	%this.playThread(2, armreadyboth);
	%this.unmountImage(0);
	%this.cough = %this.schedule(100, cough);
}

function Disease_onRoundEnd()
{
	cancel($Disease::StartingTick);
	cancel($Disease::HelicopterArrivingTick);
	cancel($Disease::SelectOrigin);
	
	ServerPlay2D(Disease_Sound_transition_end);
	for(%a = 0; %a < $DefaultMinigame.numMembers; %a++)
	{
		%client = $DefaultMinigame.member[%a];
		commandToClient(%client, 'SetVignette', 1, "255 255 255 255");
	}
	schedule(6000, 0, Disease_clearVignette);
	Disease_heliTakeoff();
}

function Disease_clearVignette()
{
	for(%a = 0; %a < $DefaultMinigame.numMembers; %a++)
	{
		%client = $DefaultMinigame.member[%a];
		commandToClient(%client, 'SetVignette', 0, 0);
		
		if(isObject(%player = %client.player))
		{
			cancel(%player.cough);
		}
	}
}

function Disease_onStart()
{
	ServerPlay2D(Disease_Sound_transition_start);
	Disease_startingTick();
	if(!isObject(DiseaseBounds))
	{
		%triggerScale = 240;
		%triggerPosition = "-" @ %triggerScale / 2 SPC %triggerScale / 2 SPC 0;
		new trigger(DiseaseBounds)
		{
			position = %triggerPosition;
			rotation = "1 0 0 0";
			scale = "1 1 3";
			datablock = DiseaseTriggerData;
			polyhedron = "0.0000000 0.0000000 0.0000000 " @ %triggerScale @ " 0.0000000 0.0000000 0.0000000 -" @ %triggerScale @ " 0.0000000 0.0000000 0.0000000 " @ %triggerScale @ ".0000000";
		};
	}
}

function Disease_selectOrigin(%retry)
{
	if($DefaultMinigame.numMembers < 2)
	{
		MessageAll('', "\c0There must be \c3Two \c0players for a \c3Patient Zero \c0to be selected!");
		ServerPlay2D(Disease_Sound_note_haunt);
		return;
	}
	
	%random = getRandom(0, $DefaultMinigame.numMembers);
	
	%client = $DefaultMinigame.member[%random];
	
	if(!isObject(%player = %client.player))
	{
		$Disease::SelectOrigin = schedule(750, 0, Disease_selectOrigin, 1);
		return;
	}
	%player.infect("START");
}

function Disease_startingTick(%a)
{
	cancel($Disease::StartingTick);
	
	if(%a == 15)
	{
		Disease_selectOrigin();
		Disease_helicopterArrivingTick();
		$Disease::isStarting = false;
		return;
	}
	else
	{
		BottomPrintAll("\c0The \c3Patient Zero \c0will be picked in \c3" @ 15 - %a @ " \c0second" @ ((15 - %a) > 1 ? "s" : "") @ ".", 1.1, 1);
	}
	
	$Disease::StartingTick = schedule(1000, 0, Disease_startingTick, %a++);
}

function Disease_helicopterArrivingTick(%a)
{
	cancel($Disease::HelicopterArrivingTick);
	
	if(%a == 60)
	{
		Disease_spawnHelicopter();
		return;
	}
	else
	{
		BottomPrintAll("\c0The \c3Helicopter \c0will arrive in \c3" @ 60 - %a @ " \c0second" @ ((60 - %a) > 1 ? "s" : "") @ ".", 1.1, 1);
	}
	
	$Disease::HelicopterArrivingTick = schedule(1000, 0, Disease_helicopterArrivingTick, %a++);
}

function Disease_checkForEnd()
{
	if($Disease::Resetting || $Disease::isStarting)
	{
		return;
	}
	for(%a = 0; %a < $DefaultMinigame.numMembers; %a++)
	{
		%client = $DefaultMinigame.member[%a];
		if(isObject(%player = %client.player) && !%player.isInfected)
		{
			%healthyCount++;
		}
		if(isObject(%player = %client.player) && %player.isInfected)
		{
			%infectedCount++;
		}
	}
	
	if(%healthyCount == 0)
	{
		MessageAll('', "\c0There are no more survivors on the ground!");
		$DefaultMinigame.scheduleReset();
	}
	if(%infectedCount == 0)
	{
		MessageAll('', "\c0There are no more infected left! The disease is \c3eradicated\c0.");
		for(%a = 0; %a < $DefaultMinigame.numMembers; %a++)
		{
			%miniPlayer = $DefaultMinigame.member[%a];
			if(isObject(%miniPlayer.player))
			{
				if(!%miniPlayer.player.isInfected)
				{
					%miniPlayer.addScore(2, "Surviving eradication");
				}
			}
		}
		$DefaultMinigame.scheduleReset();
	}
}

function Disease_spawnHelicopter()
{
	if(isObject(helicopter))
	{
		return;
	}
	%bot = new AIPlayer(helicopterBot)
	{
		datablock = BlockheadHoleBot;
	};
	%bot.schedule(100, applyPilotAvatar);
	
	%xy[%xyc = 1] = "-19.867 -80.5971";
	%xy[%xyc++] = "-42.7127 -82.5906";
	%xy[%xyc++] = "-67.1489 -52.2427";
	%xy[%xyc++] = "-75.6646 -18.1384";
	%xy[%xyc++] = "-11.7638 12.18";
	%xy[%xyc++] = "2.68198 -83.9179";
	%xy[%xyc++] = "-47.2515 -0.576858";
	
	%trans = %xy[getRandom(1,%xyc)] SPC 69;
	%trans = VectorAdd(%trans, "31 31 0");
	%heli = new FlyingVehicle(helicopter)
	{
		datablock = CityHeliMedVehicle;
	};
	%heli.setNodeColor("ALL", "0.38 0.57 0.49 1.00");
	%heli.setTransform(%trans);
	%heli.mountobject(%bot, 0);
	%heli.landingTick();
	%heli.pushAwayTick();
	
	MessageAll('', "\c0The \c3Helicopter \c0is here! All living survivors must get on it!");
	ServerPlay2D(Disease_Sound_note_haunt);
	
	schedule(getRandom(1000, 2000), 0, ServerPlay2D, Disease_Sound_note_page);
}

function AIPlayer::applyPilotAvatar(%obj)
{
	%obj.Accent = "0";
	%obj.AccentColor = "0.000 0.200 0.640 0.700";
	%obj.Chest = "1";
	%obj.ChestColor = "0.200 0.200 0.200 1.000";
	%obj.DecalName = "Mod-Pilot";
	%obj.FaceColor = "0";
	%obj.FaceName = "base/data/shapes/player/faces/smiley.png";
	%obj.Hat = "7";
	%obj.HatColor = "0 0.397196 0.257009 1";
	%obj.HeadColor = "1 0.878431 0.611765 1";
	%obj.Hip = "0";
	%obj.HipColor = "0.0784314 0.0784314 0.0784314 1";
	%obj.LArm = "0";
	%obj.LArmColor = "0.200 0.200 0.200 1.000";
	%obj.LHand = "0";
	%obj.LHandColor = "1 0.878431 0.611765 1";
	%obj.LLeg = "0";
	%obj.LLegColor = "0.105882 0.458824 0.768627 1";
	%obj.Pack = "0";
	%obj.PackColor = "0 0.435323 0.831776 1";
	%obj.RArm = "0";
	%obj.RArmColor = "0.200 0.200 0.200 1.000";
	%obj.RHand = "0";
	%obj.RHandColor = "1 0.878431 0.611765 1";
	%obj.RLeg = "0";
	%obj.RLegColor = "0.105882 0.458824 0.768627 1";
	%obj.SecondPack = "0";
	%obj.SecondPackColor = "0 1 0 1";
	%obj.TorsoColor = "0.200 0.200 0.200 1.000";
	
	%obj.player = %obj;
	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function FlyingVehicle::landingTick(%this)
{
	%this.isLanding = true;
	initContainerRadiusSearch(VectorAdd(%this.getPosition(), "0 0 -4"), 1, $TypeMasks::FxBrickObjectType);
	
	if(containerSearchNext())
	{
		%this.isLanding = false;
		return;
	}
	%this.setVelocity("0 0 -6");
	%this.schedule(40, landingTick);
}

function FlyingVehicle::pushAwayTick(%this)
{
	%pos = %this.getPosition();
	initContainerRadiusSearch(%pos, 13, $TypeMasks::PlayerObjectType);
	
	while(%obj = containerSearchNext())
	{
		if(%obj.getClassName() $= "Player")
		{
			%ppos = %obj.getPosition();
			if(%this.isLanding || %obj.isInfected)
			{
				if(!isObject(getWord(containerRaycast(%this.getPosition(), %obj.getPosition(), $TypeMasks::FxBrickObjectType, %this), 0)))
				{
					%dir = vectorNormalize(vectorSub(%obj.getHackPosition(), %this.getPosition()));
					%obj.addVelocity(vectorScale(%dir, (%obj.isCrouched() ? 1 : 2)));
					
					if(%this.isLanding)
					{
						if(isObject(%obj.client))
						{
							centerPrint(%obj.client, "The helicopter is still landing and producing too much wind!", 2);
						}
					}
					else if(%obj.isInfected)
					{
						if(isObject(%obj.client))
						{
							centerPrint(%obj.client, "Infected are too sensetive to high winds!", 2);
						}
					}
				}
			}
		}
	}
	
	%this.pushAwayTick = %this.schedule(40, pushAwayTick);
}

function Disease_removeHelicopter()
{
	if(isObject(helicopter)) {
		helicopter.delete();
	}
	if(isObject(helicopterBot)) {
		helicopterBot.delete();
	}
}

function Disease_enterHelicopter(%player)
{
	if(!isObject(%player))
	{
		return;
	}
	%client = %player.client;
	
	if(!isObject(%client))
	{
		return;
	}
	
	if(%player.isInfected)
	{
		centerPrint(%client, "Infected cannot enter the helicopter!", 2);
		return;
	}
	
	//echo("doing message");
	MessageAll('', "\c3" @ %client.name @ " \c0has entered the helicopter!");
	//echo("doing points");
	%client.addScore(4, "Surviving the disease");
	
	//echo("playing sound");
	//ServerPlay2D(Disease_Sound_note_page);
	//echo("doing camera");
	%camera = %client.camera;
	%pos = getWords(helicopter.getTransform(), 0, 2);
	%pos = VectorAdd(%pos, "16.3826 -14.974 9.4074");
	%camera.setTransform(%pos SPC "0.274232 0.114423 -0.954832 0.823959");
	//%camera.setClampMode(helicopter, "16.3826 -14.974 9.4074");
	%camera.setFlyMode();
	%camera.mode = "Observer";
	%client.setControlObject(%camera);
	//echo("deleting player");
	%player.delete();
	
	//echo("checking for end");
	Disease_checkForEnd();
}

function Disease_heliTakeoff()
{
	%this = helicopter;
	
	if(!isObject(%this))
	{
		return;
	}
	
	//initContainerRadiusSearch(%this.getPosition(), 9, $TypeMasks::PlayerObjectType);
	//
	//while(%obj = containerSearchNext())
	//{
	//	if(%obj.getClassName() $= "Player")
	//	{
	//		%obj.setVelocity(vectorAdd(%obj.getVelocity(), vectorAdd(vectorScale(%this.getForwardVector(), 20), "0 0 20")));
	//	}
	//}
	
	schedule(750, 0, Disease_heliTakeoffBegin, %this);
}

function Disease_heliTakeoffBegin(%this)
{
	%this.setVelocity("0 0 20");
	%this.schedule(800, forwardTick);
}

function FlyingVehicle::forwardTick(%this)
{
	%this.setVelocity("0 6 6");
	%this.setAngularVelocity("-0.021 0 0");
	
	//initContainerRadiusSearch(%this.getPosition(), 9, $TypeMasks::PlayerObjectType);
	//
	//while(%obj = containerSearchNext())
	//{
	//	if(%obj.getClassName() $= "Player")
	//	{
	//		%obj.setVelocity(vectorAdd(%obj.getVelocity(), vectorAdd(vectorScale(%this.getForwardVector(), 20), "0 0 20")));
	//	}
	//}
	
	%this.schedule(23, forwardTick);
}

function GameConnection::addScore(%this, %add, %msg)
{
	if(%this.minigame == $DefaultMinigame)
	{
		%this.score += %add;
		$Disease_PlayerScore_[%this.BL_ID] = %this.score;
	}
	messageClient(%this, '', "\c3+" @ %add @ " points \c0- " @ %msg);
	export("$Disease_PlayerScore_*", "config/server/Disease/score.cs");
}

function Disease_getRandomMeleeItem()
{
	%a[%b = 0] = combatKnifeItem;
	%a[%b++] = iceAxeItem;
	%a[%b++] = L4BbatonItem;
	%a[%b++] = L4BcrowbarItem;
	%a[%b++] = L4BpanItem;
	%a[%b++] = L4BguitarItem;
	%a[%b++] = L4BkatanaItem;
	%a[%b++] = L4BmacheteItem;
	%a[%b++] = L4BspadeItem;
	%a[%b++] = pipeItem;
	%a[%b++] = sledgehammerItem;
	
	return %a[getRandom(0, %b)];
}

function Disease_LeaveBounds(%obj, %client)
{
	if(!isObject(%obj) || !isObject(%client) || $Disease::Resetting)
	{
		return;
	}
	
	%obj.kill();
	MessageAll('', "\c3" @ %client.name @ " \c0has been killed for leaving the map bounds.");
}

function serverCmdHelp(%client)
{
	messageClient(%client, '', "\c3Disease \c0by \c3Zapk");
	messageClient(%client, '', "\c0  - \c3help\c0 - Display a list of commands.");
	messageClient(%client, '', "\c0  - \c3rules\c0 - Open the server rules.");
	messageClient(%client, '', "\c0  - \c3who\c0 - Prints a list of all living players.");
	messageClient(%client, '', "\c0  - \c3hat\c0 - Manage your hats and buy them with points!");
}

function serverCmdWho(%client)
{
	messageClient(%client, '', "\c3Players Alive\c0:");
	for(%a = 0; %a < $DefaultMinigame.numMembers; %a++)
	{
		%miniPlayer = $DefaultMinigame.member[%a];
		if(isObject(%miniPlayer.player))
		{
			%num++;
			if(%miniPlayer.player.isInfected)
			{
				messageClient(%client, '', "\c6" @ %num @ ". \c0" @ %miniPlayer.name);
			}
			else
			{
				messageClient(%client, '', "\c6" @ %num @ ". \c3" @ %miniPlayer.name);
			}
		}
	}
}

function serverCmdRules(%client)
{
	%title = "Disease Rules";
	%rules = "<just:left><font:arial:15>1. Do not provoke or bully other users.";
	%rules = %rules NL "2. Do not link porn, phishing, screamers, etc.";
	%rules = %rules NL "3. Do not spam the chat.";
	%rules = %rules NL "4. Respect the gameplay.";
	%rules = %rules NL "Failure to follow these rules will be subject to an admin's judgement.";
	
	commandToClient(%client, 'MessageBoxOK', %title, %rules);
}

function serverCmdHat(%client, %cmd, %hat)
{
	if(%cmd $= "")
	{
		messageClient(%client, '', "\c0Hat Index:");
		messageClient(%client, '', "\c3/hat list \c0- \c3Display a list of available hats and their prices.");
		messageClient(%client, '', "\c3/hat buy (hat) \c0- \c3Buy the desired hat for points.");
		messageClient(%client, '', "\c3/hat set (hat) \c0- \c3Set your current hat.");
		messageClient(%client, '', "\c3/hat remove \c0- \c3Remove your current hat.");
		return;
	}
	if(%cmd $= "list")
	{
		%hats[%hc = 0] = "Bandage";
		%hats[%hc++] = "Builder";
		%hats[%hc++] = "Cigarette";
		%hats[%hc++] = "Swag";
		%hats[%hc++] = "Headphones";
		%hats[%hc++] = "Jason";
		%hats[%hc++] = "Cowboy";
		%hats[%hc++] = "PaperBag";
		%hats[%hc++] = "Beret";
		%hats[%hc++] = "Nails";
		%hats[%hc++] = "Shades";
		%hats[%hc++] = "Pirate";
		%hats[%hc++] = "Bandit";
		%hats[%hc++] = "Doctor";
		//%hats[%hc++] = "Golden";
		messageClient(%client, '', "\c0Available Hats:");
		for(%a = 0; %a <= %hc; %a++)
		{
			%thishat = %hats[%a];
			messageClient(%client, '', "\c6 - \c0" @ getHatDatablock(%thishat).cost @ "p \c6- \c3" @ %thishat @ ($Disease_PlayerScore_Hat_OwnedBy[%client.bl_id, %thishat] ? " \c6(Owned)" : ""));
		}
		return;
	}
	if(%cmd $= "buy")
	{
		if(!isHat(%hat))
		{
			messageClient(%client, '', "\c0That hat doesn't exist!");
			return;
		}
		%hat = getRealHatName(%hat);
		if($Disease_PlayerScore_Hat_OwnedBy[%client.bl_id, %hat])
		{
			messageClient(%client, '', "\c0You already own that hat!");
			return;
		}
		%db = getHatDatablock(%hat);
		if(%client.score < %db.cost)
		{
			messageClient(%client, '', "\c0You need \c3" @ %db.cost @ " \c0points to buy a hat!");
			return;
		}
		%client.score -= %db.cost;
		$Disease_PlayerScore_[%client.BL_ID] = %client.score;
		$Disease_PlayerScore_Hat_OwnedBy[%client.bl_id, %hat] = true;
		messageAllExcept(%client, '', "\c3" @ %client.name @ " \c0has bought the \c3" @ %hat @ " \c0hat.");
		messageClient(%client, '', "You have bought the \c3" @ %hat @ " \c0hat for \c3" @ %db.cost @ " \c0points. Equip it with \c3/hat set " @ %hat @ "\c0.");
		
		export("$Disease_PlayerScore_*", "config/server/Disease/score.cs");
		return;
	}
	if(%cmd $= "set")
	{
		if(!isHat(%hat))
		{
			messageClient(%client, '', "\c0That hat doesn't exist!");
			return;
		}
		%hat = getRealHatName(%hat);
		if(!$Disease_PlayerScore_Hat_OwnedBy[%client.bl_id, %hat])
		{
			messageClient(%client, '', "\c0You don't own that hat! Type \c3/hat buy " @ %hat @ " \c0to buy it.");
			return;
		}
		if($Disease_PlayerScore_Hat_CurrentHat[%client.bl_id] $= %hat)
		{
			messageClient(%client, '', "\c0That is already your current hat!");
			return;
		}
		$Disease_PlayerScore_Hat_CurrentHat[%client.bl_id] = %hat;
		messageClient(%client, '', "\c0You have set your current hat to \c3" @ %hat @ "\c0.");
		mountHat(%client);
		
		export("$Disease_PlayerScore_*", "config/server/Disease/score.cs");
		return;
	}
	if(%cmd $= "remove")
	{
		if($Disease_PlayerScore_Hat_CurrentHat[%client.bl_id] $= "")
		{
			messageClient(%client, '', "\c0You do not have a hat equipped!");
			return;
		}
		$Disease_PlayerScore_Hat_CurrentHat[%client.bl_id] = "";
		messageClient(%client, '', "\c0You have removed your hat.");
		mountHat(%client);
		return;
	}
	messageClient(%client, '', "\c0Unknown hat command.");
}

function isHat(%this)
{
	if(isObject("Disease_Hat_" @ %this))
	{
		return true;
	}
	return false;
}

function getRealHatName(%this)
{
	return strreplace(nameToID("Disease_Hat_" @ %this).getName(), "Disease_Hat_", "");
}

function getHatDatablock(%this)
{
	%this = strreplace(%this, "Disease_Hat_", ""); //just incase
	return nameToID("Disease_Hat_" @ %this);
}

function mountHat(%client)
{
	%player = %client.player;
	if(!isObject(%player))
	{
		return;
	}
	
	%name = $Disease_PlayerScore_Hat_CurrentHat[%client.bl_id];
	if(%name $= "")
	{
		%player.unmountImage(2);
		%client.applyBodyParts();
		%client.applyBodyColors(1);
		return;
	}
	
	%hat = nameToID("Disease_Hat_" @ %name);
	if(!isObject(%hat))
	{
		return;
	}
	
	%player.unmountImage(2);
	%player.mountImage(%hat, 2);
	
	for(%a = 0; $hat[%a] !$= ""; %a++)
	{
		%player.hideNode($hat[%a]);
		%player.hideNode($accent[%a]);
	}
}

function Disease_Tip()
{
	cancel($Disease::Tip);
	
	%tip[%tipc = 0] = "To easily trick far-away infected, type /hug!";
	%tip[%tipc++] = "Cover each-other! The more infected, the higher chance you'll turn too!";
	%tip[%tipc++] = "As Patient Zero, try to infect as many people as possible!";
	%tip[%tipc++] = "Don't just hang around! Entering the helicopter gets you points!";
	%tip[%tipc++] = "Crouch to avoid footstep sounds and stealth past infected!";
	%tip[%tipc++] = "Did you know you can buy hats with points? Type /hat!";
	%tip[%tipc++] = "When infected, crouching can reduce effects of helicopter wind.";
	
	if(%tip[$Disease::CurrentTip] $= "")
	{
		$Disease::CurrentTip = 0;
	}
	
	MessageAll('', "\c0TIP: \c3" @ %tip[$Disease::CurrentTip]);
	
	$Disease::CurrentTip++;
	$Disease::Tip = schedule(30000, 0, Disease_Tip);
}

//Thanks, Greek2Me!
function AFKK_Tick()
{
	cancel($AFKK::tickSchedule);
	$AFKK::tickSchedule = schedule(30000,0,AFKK_Tick);

	echo("Doing AFK tick.");
	%playerCount = clientGroup.getCount();
	for(%i=0; %i < %playerCount; %i++)
	{
		%cl = clientGroup.getObject(%i);

		//Let's not kill the server.
		if(%cl.isLocalConnection())
			continue;

		//They are still loading. It would be rude to kick them.
		if(!%cl.hasSpawnedOnce)
			continue;

		//Check if they are privileged enough to escape punishment.
		if(%cl.isSuperAdmin)
			continue;

		//Let's see if they've moved in a while.
		%pl = %cl.player;
		%ca = %cl.camera;
		if(isObject(%pl) && %cl.getControlObject() == %pl)
		{
			%ev = %pl.getEyeVector();
			%trans = %pl.getTransform();
			if(%ev $= %cl.lastEV && %trans $= %cl.lastTransform)
				%cl.afkTick ++;
			else
				%cl.afkTick = 0;

			%cl.lastEV = %ev;
			%cl.lastTransform = %trans;
		}
		else if(isObject(%ca) && %cl.getControlObject() == %ca)
		{
			//If they're in orbit mode, only check their eye vector
			if(%ca.mode $= "corpse" || isObject(%ca.getOrbitObject()))
			{
				%ev = %ca.getEyeVector();
				if(%ev $= %cl.lastEV)
					%cl.afkTick ++;
				else
					%cl.afkTick = 0;

				%cl.lastEV = %ev;
			}
			else //they are in free mode
			{
				%ev = %ca.getEyeVector();
				%trans = %ca.getTransform();
				if(%ev $= %cl.lastEV && %trans $= %cl.lastTransform)
					%cl.afkTick ++;
				else
					%cl.afkTick = 0;

				%cl.lastEV = %ev;
				%cl.lastTransform = %trans;
			}
		}
		else //we have no real way to tell other than their chat, so label them AFK
			%cl.afkTick ++;

		//This is located here because we still want to be identifying AFK
		//players, even when we're disabled. That way we can get to work
		//immediately when someone enables us.

		//They're AFK, so give them the boot.
		if(%cl.afkTick >= 5 + 1 && %cl.afkWarned)
		{
			MessageAll('',"\c3CONSOLE \c2kicked \c3" @ %cl.getPlayerName() @ "\c2(ID:" SPC %cl.getBLID() @ ") for being AFK.");
			%cl.delete("You were kicked from the server for being <a:www.urbandictionary.com/define.php?term=Afk>AFK</a>.\nYou may rejoin at any time.");
		}
		else if(%cl.afkTick >= 5) //notify client that they're about to be kicked
		{
			commandToClient(%cl,'MessageBoxOKCancel',"Alert!","You will be kicked from the game in 30 seconds unless you click \"OK\" below.",'notAFK');
			%cl.afkWarned = 1;
		}
	}
}

function AFKK_clientActive(%client)
{
	%client.afkTick = 0;
	%client.afkWarned = 0;
	%client.lastEV = 0;
	%client.lastTransform = 0;
}

function serverCmdNotAFK(%client)
{
	AFKK_clientActive(%client);
}

AFKK_Tick();
Disease_Tip();

if(isFile("Add-Ons/System_Zapkraft/server.cs"))
{
	exec("Add-Ons/System_Zapkraft/server.cs");
}