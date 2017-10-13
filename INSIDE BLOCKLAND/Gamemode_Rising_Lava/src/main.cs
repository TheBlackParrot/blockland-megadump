//  _____  _     _               _                      
// |  __ \(_)   (_)             | |                     
// | |__) |_ ___ _ _ __   __ _  | |     __ ___   ____ _ 
// |  _  /| / __| | '_ \ / _` | | |    / _` \ \ / / _` |
// | | \ \| \__ \ | | | | (_| | | |___| (_| |\ V / (_| |
// |_|  \_\_|___/_|_| |_|\__, | |______\__,_| \_/ \__,_|
//                        __/ |                         
//                       |___/                          

//	OFFICIAL RELEASE ...
//	AUGUST 17, 2014

//	DEBLOAT/DEPRECATION UPDATE ...
//	JULY 31, 2017

// AIConnection was deprecated -- TBP
exec("./env.cs");

// only doing this lava if the addon is present -- TBP
if(isFile("Add-Ons/Water_BrickLava.zip")) {
	$EnvGuiServer::WaterFile = "Add-Ons/Water_BrickLava/bricklava.water";
}

function RL_Reload(%val)
{
	exec("./main.cs");
	messageAll('', "\c6The \c4Rising Lava \c6code has been reloaded by the host.");

	if(!%val)
	{
		return;
	}
	
	%file = new fileObject();
	%dir = "config/server/Rising Lava/changes.txt";
	%file.openForRead(%dir);
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		if(%line !$= "")
		{
			messageAll('', "\c4 - \c6" @ %line);
		}
	}
	%file.close();
	%file.delete();
}

function RL_Exec(%file)
{
	%file = "Add-Ons/GameMode_Rising_Lava/src/" @ %file @ ".cs";

	if(isFile(%file))
	{
		exec(%file);
	}
}

function RL_Reboot(%reason)
{
	if(%reason $= "")
	{
		return error("RL_Reboot(): No reason specified.");
	}
	messageAll('MsgAdminForce', "\c4Warning\c6: The server will reboot in \c410 \c6seconds.");
	messageAll('', "\c4Reason\c6: " @ %reason);
	schedule(5000, 0, messageAll, 'MsgAdminForce', "\c4Warning\c6: The server will reboot in \c45 \c6seconds.");
	schedule(5000, 0, messageAll, 'MsgAdminForce', "\c4Reason\c6: " @ %reason);
	schedule(10000, 0, RL_RebootStageTwo, %reason);
}

function RL_RebootStageTwo(%reason)
{
	messageAll('MsgAdminForce', "\c4Warning\c6: The server is now rebooting.");
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		ClientGroup.getObject(%a).delete("The server needed to reboot for the following reason:" NL %reason NL "It should be back up within seconds.");
	}
	schedule(2000, 0, changeGameMode, "Add-Ons/GameMode_Rising_Lava/gamemode.txt");
}

function RL_OrientLava()
{
	waterPlane.setTransform("0 0 -0.5 1 0 0 0");
	waterZone.setScale("1000000 1000000 1000");
	waterZone.setTransform("-500000 500000 -1000 1 0 0 0");
	waterPlane.sendUpdate();
}

function RL_SetLavaHeight(%height)
{
	waterPlane.setTransform("0 0" SPC %height - 0.5 SPC "1 0 0 0");
	waterZone.setTransform("-500000 500000" SPC %height - 1000 SPC "1 0 0 0");
	waterPlane.sendUpdate();
}

function RL_StartInfoClock()
{
	return; // turned off for release -- TBP
	echo("Started the Rising Lava info clock.");
	schedule(500, 0, RL_InfoClockLoop);
}

function RL_DoBrickTags(%name)
{
	return; // the fuck lol -- TBP
	%client = findClientByName(%name);
	%client.clanPrefix = "<bitmap:base/data/shapes/brickTOP>";
	%client.clanSuffix = "<bitmap:base/data/shapes/brickTOP>";
}

function RL_InfoClockLoop()
{
	return; // turned off for release -- TBP
	cancel($RL_InfoClockLoop);
	%MessageCount = 0;
	%Message[%MessageCount++] = "Check out our <a:forum.zapkraft.net>forum</a>\c6!";
	%Message[%MessageCount++] = "We're open to user-made maps!";
	%Message[%MessageCount++] = "Online stats and leads at <a:zapkraft.net/lava>zapkraft.net/lava</a>\c6!";
	%Message[%MessageCount++] = "Did you know Rising Lava has a client add-on? <a:" @ $RL::ClientDownloadLink @ ">Download here!</a>";
	%Message[%MessageCount++] = "You can spend your coins on cool stuff! Type /shop!";
	echo("Rising Lava: Pushed info message.");
	MessageAll('MsgUploadEnd', "\c4Rising Lava\c6:" SPC %Message[getRandom(1, %MessageCount)]);
	$RL_InfoClockLoop = schedule(60000 * 2, 0, RL_InfoClockLoop);
}

function RL_StopInfoClock()
{
	return; // turned off for release -- TBP
	echo("Stopped the Rising Lava info clock.");
	cancel($RL_InfoClockLoop);
}

//		MAIN GAME MODE CODE
function RL_BuildLevelList()
{
	%pattern = "Add-Ons/RisingLava_*/save.bls";
	
	$RL::numLevels = 0;
	
	%file = findFirstFile(%pattern);
	while(%file !$= "")
	{
		$RL::Level[$RL::numLevels] = %file;
		$RL::numLevels++;

		%file = findNextFile(%pattern);
	}
}

function RL_DumpLevelList()
{
	echo("");
	if($RL::numLevels == 1)
		echo("1 level");
	else
		echo($RL::numLevels @ " levels");
	for(%i = 0; %i < $RL::numLevels; %i++)
	{
		%displayName = $RL::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		%displayName = strReplace(%displayName, "_", " ");

		if(%i == $RL::CurrentLevel)
			echo(" >" @ %displayName);
		else
			echo("  " @ %displayName);
	}
	echo("");
}

function RL_BeginMapVote()
{
	RL_CancelLoops();
	$RL::ResetCount = 0;
	$RL::Voting = true;

	commandToAll('RLC_VoteInit');

	for(%i = 0; %i < $RL::numLevels; %i++)
	{
		%displayName = $RL::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		commandToAll('RLC_AddVoteMap', %i, %displayName);
		%displayName = "rl_" @ strlwr(%displayName);
		messageAll('', "\c4" @ %i @ "\c6. " @ %displayName);
	}

	messageAll('', "\c6The map vote has commenced!");
	messageAll('', "\c6Type /\c4voteMap \c6[\c4number\c6] to vote, or use the on-screen GUI if you have it.");
	messageAll('', "\c6The map with the most votes will be loaded in \c430 \c6seconds.");

	bottomPrintAll("\c6Vote for your favourite map now! /\c4voteMap \c6[\c4number\c6]", 30, 1);

	RL_MapVoteLoop(30);
}

function RL_MapVoteLoop(%time)
{
	RL_CancelLoops();
	cancel($RL::MapVoteLoop);

	commandToAll('RLC_SetVoteTime', %time);

	if(%time <= 0)
	{
		RL_StopMapVote();
		return;
	}

	$RL::MapVoteLoop = schedule(1000, 0, RL_MapVoteLoop, %time--);
}

function serverCmdVoteMap(%this, %index)
{
	if(!$RL::Voting)
	{
		messageClient(%this, '', "\c6There is no map vote in progress.");
		return;
	}

	if(%index $= "" || $RL::Level[%index] $= "")
	{
		messageClient(%this, '', "\c6That map doesn't seem to exist! Type /\c4levelList \c6for a list of map IDs.");
		return;
	}

	if(%this.mapVote !$= "" && %this.mapVote == %index)
	{
		messageClient(%this, '', "\c6You already voted for that map!");
		return;
	}

	if($Sim::Time - %this.lastMVT < 0.5)
	{
		messageClient(%this, '', "\c6You are voting too fast! Make up your mind.");
		return;
	}

	%this.lastMVT = $Sim::Time;
	%this.mapVote = %index;

	%displayName = $RL::Level[%index];
	%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
	%displayName = strReplace(%displayName, "/save.bls", "");
	%displayName = "rl_" @ strlwr(%displayName);

	messageClient(%this, '', "\c6You have voted for \c4" @ %displayName @ "\c6.");

	commandToAll('RLC_RemoveVoteEmoticon', %this.getID());
	commandToAll('RLC_AddVoteEmoticon', %index, %this.getID());
}

function RL_StopMapVote()
{
	RL_CancelLoops();
	messageAll('', "\c6The map vote has finished! Votes will now be tallied.");
	$RL::Voting = false;

	commandToAll('RLC_VoteEnd');

	schedule(getRandom(500, 1500), 0, RL_TallyMapVotes);
}

function RL_TallyMapVotes()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%obj = ClientGroup.getObject(%a);

		if(%obj.mapVote $= "")
		{
			continue;
		}

		%votes++;
		%voteMaps[%obj.mapVote]++;
	}

	if(%votes < 1)
	{
		messageAll('', "\c6Nobody voted! The next level will be loaded.");
		RL_NextLevel();
		return;
	}

	for(%i = 0; %i < $RL::numLevels; %i++)
	{
		%displayName = $RL::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		%displayName = "rl_" @ strlwr(%displayName);

		if(%voteMaps[%i] > %highestVotes)
		{
			%highestVotes = %voteMaps[%i];
			%highestVotedMap = %i;
			%highestVotedName = %displayName;
		}
	}

	if(%highestVotedMap $= "" || %highestVotes $= "" || %highestVotedName $= "")
	{
		messageAll('', "\c6A winner could not be calculated! The next level will be loaded.");
		RL_NextLevel();
		return;
	}

	messageAll('', "\c4" @ %highestVotedName SPC "\c6wins with a total of \c4" @ %highestVotes SPC "\c6votes. It will now be loaded.");

	$RL::CurrentLevel = %highestVotedMap % $RL::numLevels;
	$RL::ResetCount = 0;

	RL_LoadLevel_Phase1($RL::Level[$RL::CurrentLevel]);

	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%obj = ClientGroup.getObject(%a);
		%obj.mapVote = "";
	}
}

function RL_NextLevel()
{
	$RL::CurrentLevel = mFloor($RL::CurrentLevel);
	$RL::CurrentLevel++;
	$RL::CurrentLevel = $RL::CurrentLevel % $RL::numLevels;

	$RL::ResetCount = 0;

	RL_LoadLevel_Phase1($RL::Level[$RL::CurrentLevel]);
}

function RL_LoadLevel_Phase1(%filename)
{
	//suspend minigame resets
	$RL::MapChange = 1;

	//put everyone in observer mode
	%mg = $DefaultMiniGame;
	if(!isObject(%mg))
	{
		error("ERROR: RL_LoadLevel( " @ %filename  @ " ) - default minigame does not exist");
		return;
	}
	for(%i = 0; %i < %mg.numMembers; %i++)
	{
		%client = %mg.member[%i];
		%player = %client.player;
		if(isObject(%player))
			%player.delete();

		%camera = %client.camera;
		%camera.setFlyMode();
		%camera.mode = "Observer";
		%client.setControlObject(%camera);
	}
	
	//clear all bricks 
	// note: this function is deferred, so we'll have to set a callback to be triggered when it's done
	BrickGroup_888888.chaindeletecallback = "RL_LoadLevel_Phase2(\"" @ %filename @ "\");";
	BrickGroup_888888.chaindeleteall();
}

function RL_LoadLevel_Phase2(%filename)
{
	echo("Loading rising lava save " @ %filename);

	%displayName = %filename;
	%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
	%displayName = strReplace(%displayName, "/save.bls", "");
	%displayName = strReplace(%displayName, "_", " ");
	
	%loadMsg = "\c6Now loading \c4" @ %displayName;

	//read and display credits file, if it exists
	// limited to one line
	%creditsFilename = filePath(%fileName) @ "/credits.txt";
	if(isFile(%creditsFilename))
	{
		%file = new FileObject();
		%file.openforRead(%creditsFilename);

		%line = %file.readLine();
		%line = stripMLControlChars(%line);
		%loadMsg = %loadMsg @ "\c6, created by \c4" @ %line;

		%file.close();
		%file.delete();
	}

	messageAll('', %loadMsg);

	//load environment if it exists
	%envFile = filePath(%fileName) @ "/environment.txt"; 
	if(isFile(%envFile))
	{  
		//echo("parsing env file " @ %envFile);
		//usage: GameModeGuiServer::ParseGameModeFile(%filename, %append);
		//if %append == 0, all minigame variables will be cleared 
		%res = GameModeGuiServer::ParseGameModeFile(%envFile, 1);

		EnvGuiServer::getIdxFromFilenames();
		EnvGuiServer::SetSimpleMode();

		if(!$EnvGuiServer::SimpleMode)	  
		{
			EnvGuiServer::fillAdvancedVarsFromSimple();
			EnvGuiServer::SetAdvancedMode();
		}
	}
	
	%cfgFile = filePath(%fileName) @ "/config.txt"; 
	if(isFile(%cfgFile))
	{  
	echo("Loading custom Rising Lava config file.");
	%file = new fileObject();
	
	%file.openForRead(filePath(%fileName) @ "/config.txt");
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		if(firstWord(%line) $= "MAX_LAVA_HEIGHT")
		{
			$RL_MaxLavaHeight = restWords(%line);
		}
	}
	%file.close();
	%file.delete();
	}
	else
	{
	 $RL_MaxLavaHeight = 20;
	}
	
	//load save file
	schedule(10, 0, serverDirectSaveFileLoad, %fileName, 3, "", 2, 1);
}


//		COMMANDS FOR THE MANS
function serverCmdLevelList(%client)
{
	for(%i = 0; %i < $RL::numLevels; %i++)
	{
		%displayName = $RL::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		%displayName = "rl_" @ strlwr(%displayName);

		if(%i == $RL::CurrentLevel)
			messageClient(%client, '', "\c4" @ %i @ "\c6. " @ %displayName SPC "(\c4Current\c6)");
		else
			messageClient(%client, '', "\c4" @ %i @ "\c6. " @ %displayName);
	}
}

function servercmdReloadLevels(%client)
{
	if(!%client.isSuperAdmin)
	{
		messageClient(%client, '', "\c6You are not super admin!");
		return;
	}
	messageAll('', "\c4" @ %client.name SPC "\c6reloaded the Rising Lava level cache.");
	setModPaths(getModPaths());
	RL_BuildLevelList();
}
function serverCmdSetLevel(%client, %i)
{
	if(!%client.isAdmin)
		return;

	if(mFloor(%i) !$= %i)
	{
		messageClient(%client, '', "Usage: /setLevel <number>");
		return;
	}

	if(%i < 0 || %i > $RL::numLevels)
	{
		messageClient(%client, '', "serverCmdSetLevel() - out of range index");
		return;
	}

	messageAll( 'MsgAdminForce', '\c4%1\c6 changed the level', %client.getPlayerName());
	
	$RL::CurrentLevel = %i - 1;
	RL_NextLevel();
}

function serverCmdNextLevel(%client, %i)
{
	if(!%client.isAdmin)
		return;

	messageAll( 'MsgAdminForce', '\c4%1\c6 changed the level', %client.getPlayerName());
	
	RL_NextLevel();
}

function serverCmdRules(%client)
{
	%title = "Rising Lava Rules";
	%rules = "<just:left><font:arial:15>1. Do not provoke or bully other users.";
	%rules = %rules NL "2. Do not block other users before the round has begun.";
	%rules = %rules NL "3. Do not post pornographic links.";
	%rules = %rules NL "4. Do not spam the chat.";
	%rules = %rules NL "Failure to follow these rules will be subject to an admin's judgement.";
	
	commandToClient(%client, 'MessageBoxOK', %title, %rules);
}

//		PACKAGE
deactivatepackage(GameModeRisingLavaPackage);
package GameModeRisingLavaPackage
{
	function gameConnection::autoAdminCheck(%client)
	{
		commandToClient(%client, 'RLCHandshake');

		if($RL_PlayerScore_[%client.BL_ID])
		{
			messageClient(%client, '', "\c6Welcome back \c4" @ %client.name @ "\c6! You left off with a score of \c4" @ $RL_PlayerScore_[%client.BL_ID] @ "\c6.");
		}
		// why would you need an intro video, wh -- TBP
		//messageClient(%client, '', "\c6Are you new here? Watch the <a:youtube.com/watch?v=Hq0ChYhp6LE>Intro Video</a>");
		
		RL_LoadPlayerScores();
		
		// blah blah only do the thing if u know what ur doing blah blah -- TBP
		//RL_PhpPost("post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ %client.bl_id @ "&lastjoin=" @ getDateTime() @ "&name=" @ %client.name @ "&ip=" @ %client.getRawIP());
		
		return Parent::autoAdminCheck(%client);
	}
	function gameConnection::onClientEnterGame(%client)
	{
		Parent::onClientEnterGame(%client);
		schedule(1000, 0, serverCmdRules, %client);
	}
	function GameConnection::onClientLeaveGame(%client)
	{
		%lastAttacker = %client.lastAttacker;
		if(isObject(%lastAttacker))
		{
			%lastAttacker.addScore(1);
			MessageAll('', "\c4" @ %client.name SPC "\c6was killed by\c4" SPC %lastAttacker.name @ "\c6.");
			echo("Rising Lava:" SPC %lastAttacker.name SPC "killed" SPC %client.name);
		}
		commandToAll('RLC_RemoveVoteEmoticon', %client.getID());
		Parent::onClientLeaveGame(%client);
	}
	function Armor::onTrigger(%this,%player,%slot,%val,%a,%b,%c)
	{
		Parent::onTrigger(%this,%player,%slot,%val,%a,%b,%c);
		%client = %player.client;
		if(%slot == 4 && %val)
		{
			%ray = containerRayCast(%player.getEyePoint(),vectorAdd(%player.getEyePoint(),vectorScale(%player.getEyeVector(),5 * getWord(%player.getScale(),2))),$TypeMasks::FxBrickObjectType,%player);
			%target = firstWord(%ray);
			%player.playThread(3, "shiftUp");
			if(isObject(%target))
			{
				%level = %player.getEnergyLevel();
				if(%level > 15)
				{
					%player.addVelocity("0 0 3.1");
					%player.playAudio(1, stopSilentSound);

					%newlevel = %level < 15 ? 0 : %level - 15;
					%player.setEnergyLevel(%newlevel);
				}
				else
				{
					%player.playAudio(1, stopLoudSound);
				}
			}
		}
	}
	//		GREENLIGHT STUFF
	function gameConnection::spawnPlayer(%client)
	{
		Parent::spawnPlayer(%client);
		%client.player.canClimb = true;
		//%client.RLCNotify();
		//%client.resetTrailHides();
		//%client.player.setLight(RedLight);
		//%client.player.schedule(1, removeHead);
		%client.score = $RL_PlayerScore_[%client.BL_ID];

		commandToClient(%client, 'RLC_ShopPowerups_SetCover', 0);
		commandToClient(%client, 'ShowEnergyBar', 1);

		//%client.player.trailLoop();
	}

	// there was a bunch of useless crap here -- TBP

	function GameConnection::OnDeath(%client, %player, %killer, %damageType, %damageLoc)
	{
		Parent::OnDeath(%client, %player, %killer, %damageType, %damageLoc);
		commandToClient(%client, 'RLC_ShopPowerups_SetCover', 1);

		if(isObject(%client.minigame))
		{
			%peopleAlive = 0;
			$RL_PeopleAllive = 0;
			for(%a = 0; %a < %client.minigame.numMembers; %a++)
			{
				%miniPlayer = %client.minigame.member[%a];
				if(isObject(%miniPlayer.player))
				{
					%peopleAlive++;
					$RL_PeopleAllive++;
					%lastDetectedPlayer = %miniPlayer;
				}
			}
			echo("Rising Lava: " @ %peopleAlive @ " players left.");
			if(%peopleAlive < 2)
			{
				RL_CancelLoops();
				RL_OrientLava();
				RL_SetLavaHeight(0);
				serverPlay2D(roundWinSound);
				if(%peopleAlive == 1)
				{
					echo("Rising Lava: We have a winner!");
					bottomPrintAll("<font:impact:25>\c4" @ %lastDetectedPlayer.name SPC "\c6wins!", 3, 1);
					messageClient(%lastDetectedPlayer, '', "\c6Congratulations \c4" @ %lastDetectedPlayer.name @ "\c6, you win!");
					%lastDetectedPlayer.addScore(1);
					%lastDetectedPlayer.addMoney(100);
				}
			}
		}
		%lastAttacker = %client.lastAttacker;
		if(%lastAttacker != 0)
		{
			messageClient(%client, '', "\c6You were killed by \c4" @ %lastAttacker.name @ "\c6!");
			%lastAttacker.addScore(1);
			%lastAttacker.addMoney(50);
			messageClient(%lastAttacker, '', "\c6You have killed \c4" @ %client.name @ "\c6!");
			echo("Rising Lava:" SPC %lastAttacker.name SPC "killed" SPC %client.name);
		}

		cancel($RL_CannonSound);
		$RL_CannonSound = schedule(getRandom(100, 1000), 0, serverPlay2D, deathCannonSound);

		//%client.resetTrailHides();
	}

	function fxDTSBrick::setVehicle(%obj, %data, %client)
	{
		return;
	}

	function fxDTSBrick::setItem(%obj, %data, %client)
	{
		return;
	}

	function GameModeInitialResetCheck()
	{
		RL_CancelLoops();
		Parent::GameModeInitialResetCheck();

		//if there is no level list, attempt to create it
		if($RL::numLevels == 0)
			RL_BuildLevelList();
		
		//if levellist is still empty, there are no levels
		if($RL::numLevels == 0)
		{
			messageAll('', "\c6No RisingLava levels available!");
			return;
		}

		if($RL::Initialized)
			return;

		$RL::Initialized = true;
		$RL::CurrentLevel = -1;
				
		RL_NextLevel();
	}

	//when we're done loading a new level, reset the minigame
	function ServerLoadSaveFile_End()
	{
		Parent::ServerLoadSaveFile_End();

		//new level has loaded, reset minigame
		if($DefaultMiniGame.numMembers > 0) //don't bother if no one is here (this also prevents starting at round 2 on server creation)
			$DefaultMiniGame.scheduleReset(); //don't do it instantly, to give people a little bit of time to ghost
	}
	function MiniGameSO::Reset(%obj, %client)
	{
		RL_CancelLoops();
		RL_OrientLava();
		RL_SetLavaHeight(0);
		%obj.playerDatablock = PlayerNoJet;
		//make sure this value is an number
		$Pref::Server::RisingLava::RoundLimit = mFloor($Pref::Server::RisingLava::RoundLimit);


		//count number of minigame resets, when we reach the limit, go to next level
		if(%obj.numMembers >= 0)
		{
			$RL::ResetCount++;
		}

		if($RL::ResetCount > $Pref::Server::RisingLava::RoundLimit)
		{
			$RL::ResetCount = 0;
			RL_BeginMapVote();
		}
		else if(!$RL::Voting)
		{
			messageAll('', "\c6Beginning round \c4" @ $RL::ResetCount @ " \c6of \c4" @ $Pref::Server::RisingLava::RoundLimit);
		 
		 Parent::Reset(%obj, %client);
		}
	  //cancel($RL_MessageSchedule);
	  RL_StartLavaSequence();
	  RL_LoadPlayerScores();
	}
	function MiniGameSO::messageAllExcept(%obj, %client, %tag, %msg, %extra, %extra2, %extra3, %extra4)
	{
		if(getTaggedString(%tag) $= "MsgClientKilled")
		{
			//echo("death message test");
			//echo(%client.name);
			if(%client.lastAttacker)
			{
				messageAllExcept(%client, '', "\c4" @ %client.name SPC "\c6was killed by\c4" SPC %client.lastAttacker.name @ "\c6.");
				
				//RL_PhpPost("post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ %client.bl_id @ "&victimizer=" @ %client.lastAttacker.name @ "&name=" @ %client.name @ "&ip=" @ %client.getRawIP());
				//RL_PhpPost("post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ %client.lastAttacker.bl_id @ "&victim=" @ %client.name @ "&name=" @ %client.lastAttacker.name @ "&ip=" @ %client.lastAttacker.getRawIP());
			}
			else
			{
				messageAll('', "\c4" @ %client.name SPC "\c6has died!");
				//RL_PhpPost("post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ %client.bl_id @ "&victimizer=Unknown&name=" @ %client.name @ "&ip=" @ %client.getRawIP());
			}
			return;
		}
		Parent::messageAllExcept(%obj, %client, %tag, %msg, %extra, %extra2, %extra3, %extra4);
	}
	function Quit()
	{
		RL_SavePlayerScores();
		Parent::Quit();
	}
	function pushBroomProjectile::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts)
	{
		Parent::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts);
		%cl = %col.client;
		%source = %obj.sourceObject.client;
		%cl.lastAttacker = %source;
		cancel(%cl.lastAttackSched);
		if(isObject(%cl.player))
		{
			%cl.lastAttackSched = schedule(10000, 0, RL_resetLastAttacker, %cl);
		}
	}
	function goldenPushBroomProjectile::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts)
	{
		Parent::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts);
		%cl = %col.client;
		%source = %obj.sourceObject.client;
		%cl.lastAttacker = %source;
		cancel(%cl.lastAttackSched);
		if(isObject(%cl.player))
		{
			%cl.lastAttackSched = schedule(10000, 0, RL_resetLastAttacker, %cl);
		}
	}
	function powerStaffProjectile::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts)
	{
		Parent::onCollision(%this, %obj, %col, %vec, %vecLen, %that, %potatoe, %sexual, %farts);
		%cl = %col.client;
		%source = %obj.sourceObject.client;
		%cl.lastAttacker = %source;
		cancel(%cl.lastAttackSched);
		if(isObject(%cl.player))
		{
			%cl.lastAttackSched = schedule(10000, 0, RL_resetLastAttacker, %cl);
		}
	}
};
activatePackage(GameModeRisingLavaPackage);

function RL_resetLastAttacker(%cl)
{
	cancel(%cl.lastAttackSched);
	%cl.lastAttacker = 0;
}

function RL_SavePlayerScores()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		if(%client.score > 0 && %client.minigame == $DefaultMinigame)
		{
			$RL_PlayerScore_[%client.BL_ID] = %client.score;
		}
	}
	export("$RL_PlayerScore_*", "config/server/Rising Lava/score.cs");
}

function RL_PostAllScores()
{
	return;
	messageAll('MsgAdminForce', "\c4RL_PostAllScores() \c6has been called.");
	messageAll('', "\c6All player scores on the server will now be slowly posted to the web. This may cause lag.");
	echo("BEGINNING RL_POSTALLSCORES()");
	%file = new fileObject();
	%file.openForRead("config/server/Rising Lava/score.cs");
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		%line = strReplace(%line, "$RL_PlayerScore_", "");
		%line = strReplace(%line, " = \"", " ");
		%line = strReplace(%line, "\";", "");
		%parem = "post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ getWord(%line, 0) @ "&score=" @ getWord(%line, 1);
		schedule(%sched, 0, RL_PhpPost, %parem);
		%sched += 150;
	}
	%file.close();
	%file.delete();
}

function RL_PhpPost(%parem)
{
	//Redacted for release. You want to set up a webserver, be my guest. -Zapk
	return;
	$RL_PhpPostPassword = "redacted";
	cancel($RL_PhpPost_DropWait);
	$RL_PhpPostWait += 1000;
	$RL_PhpPost_DropWait = schedule(2000, 0, RL_PhpPost_DropWait);
	schedule($RL_PhpPostWait, 0, RL_PhpPostPhaseTwo, %parem, $RL_PhpPostWait);
}

function RL_PhpPostPhaseTwo(%parem, %queue)
{
	return;
	while(isObject(RL_PhpPost))
	{
		RL_PhpPost.delete();
	}
	new TCPObject(RL_PhpPost);
	%parem = strReplace(%parem, " ", "%20");
	RL_PhpPost.parem = %parem;
	RL_PhpPost.QueueNum = %queue;
	RL_PhpPost.connect("zapkraft.net:80");
	
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		if(%client.debugMode == true)
		{
			messageClient(%client, '', "\c4DebugMode\c6: " @ strreplace(%parem, $RL_PhpPostPassword, "redacted"));
		}
	}
}

function RL_PhpPost_DropWait()
{
	return;
	$RL_PhpPostWait = 0;
}

function RL_PhpPost::onConnected(%this)
{
	return;
	echo("(" @ %this.QueueNum / 1000 @ ") - Posting to Zapkraft servers");
	%this.send("GET /lava/" @ %this.parem @ " HTTP/1.0\r\nHost: zapkraft.net\r\n\r\n");
	
	%file = new fileObject();
	%dir = "config/server/Rising Lava/PhpPost/" @ strReplace(getWord(getDateTime(),0),"/","-") @ ".log";
	%file.openForAppend(%dir);
	%file.writeLine(getDateTime() TAB %this.parem);
	%file.close();
	%file.delete();
}

function RL_PhpPost::onConnectFailed(%this)
{
	return;
	%file = new fileObject();
	%dir = "config/server/Rising Lava/PhpPost/" @ strReplace(getWord(getDateTime(),0),"/","-") @ ".log";
	%file.openForAppend(%dir);
	%file.writeLine(getDateTime() TAB "Connection failed!");
	%file.close();
	%file.delete();
}


function GameConnection::addScore(%this, %add)
{
	if(%this.minigame == $DefaultMinigame)
	{
		%this.score += %add;
		$RL_PlayerScore_[%this.BL_ID] = %this.score;
		export("$RL_PlayerScore_*", "config/server/Rising Lava/score.cs");
		
		//RL_PhpPost("post.php?password=" @ $RL_PhpPostPassword @ "&id=" @ %this.bl_id @ "&score=" @ %this.score @ "&name=" @ %this.name);
		
		if(%this.score / 50 == mFloor(%this.score / 50))
		{
			messageAll('', "\c4" @ %this.name SPC "\c6has reached level\c4" SPC %this.score @ "\c6!");
		}
	}
}

function GameConnection::addMoney(%this, %add)
{
	if(%this.minigame == $DefaultMinigame)
	{
		$RL_PlayerScore_Money_[%this.BL_ID] += %add;
		export("$RL_PlayerScore_*", "config/server/Rising Lava/score.cs");

		centerPrint(%this, "\c6You gained \c4" @ %add @ "\c6 Zapkoin" @ (%add == 1 ? "" : "s") @ ".\n\c6You can spend it in the \c4/shop\c6 at any time by pressing the \c4Plant Brick\c6 key.", 2);
	}
}

function RL_ClimbingCoolDown(%player)
{
	%player.canClimb = true;
	%player.climbCount = 0;
	%player.climbingCoolDown = 0;
}

function RL_PlayWinSound()
{
	serverPlay2D(Synth_09_Sound);
	schedule(500, 0, serverPlay2D, Synth_10_Sound);
	schedule(1000, 0, serverPlay2D, Synth_11_Sound);
}

function RL_LoadPlayerScores()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		%client.score = $RL_PlayerScore_[%client.BL_ID];
	}	
}

function DumpPlayerScores()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		echo(%client.score SPC "-" SPC %client.name);
	}
}

//		LAVA CODE
function RL_StartLavaSequence()
{
	$Water::Kill = true;

	RL_OrientLava();
	RL_SetLavaHeight(0);
	
	$Pref::Server::RisingLava::PreRound = mFloor($Pref::Server::RisingLava::PreRound); 
	
	RL_CountdownTick();
	RL_LoadPlayerScores();

	//RL_createDrops();
}

function RL_CountdownTick(%times)
{
	cancel($RL_CountdownTick);

	%seconds = $Pref::Server::RisingLava::PreRound - %times;

	if(%times == $Pref::Server::RisingLava::PreRound)
	{
		messageAll( 'MsgAdminForce', "\c6The lava has began to rise! Players have been given \c4Push Brooms\c6." );
		RL_StartRisingSequnce();
	}
	if(%times >= $Pref::Server::RisingLava::PreRound)
	{
		return;
	}
	bottomPrintAll( "<font:impact:25>\c6Starting In:\c4" SPC %seconds, 2, 1 );
	if(%seconds > 0 && (%seconds == $Pref::Server::RisingLava::PreRound || %seconds == 30 || %seconds == 20 || %seconds <= 10))
	{
		messageAll( "\c6The lava will rise in \c4" @ %seconds @ "\c6 seconds." );
		if(%seconds < 11)
		{
			serverPlay2D(roundImminentCountdown);
		}
	}

	$RL_CountdownTick = schedule(1000, 0, RL_CountdownTick, %times++);
}

function RL_StartRisingSequnce()
{
	RL_LoadPlayerScores();
	RL_LavaContinue();
	RL_BeginSpeedTime();
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);

		if(!isObject(%client.player))
		{
			commandToClient(%client, 'PlayGui_CreateToolHud', %numTools);
			continue;
		}
		
		//if(%client.donorPackage $= "Sponsor" || %client.isSuperAdmin)
		//{
		//	%client.player.tool[%n=0] = powerStaffItem.getID();
		//	messageClient(%client,'MsgItemPickup','',%n,powerStaffItem.getID());
		//	%client.player.tool[%n++] = goldenPushBroomItem.getID();
		//	messageClient(%client,'MsgItemPickup','',%n,goldenPushBroomItem.getID());
		//	%client.player.tool[%n++] = PushBroomItem.getID();
		//	messageClient(%client,'MsgItemPickup','',%n,PushBroomItem.getID());
		//	%numTools = %n + 1;
		//}
		//else if(%client.isVIP || %client.isAdmin)
		if(%client.isAdmin)
		{
			%client.player.tool[%n=0] = goldenPushBroomItem.getID();
			messageClient(%client,'MsgItemPickup','',%n,goldenPushBroomItem.getID());
			%client.player.tool[%n++] = PushBroomItem.getID();
			messageClient(%client,'MsgItemPickup','',%n,PushBroomItem.getID());
			%numTools = %n + 1;
		}
		else
		{
			%client.player.tool[%n=0] = PushBroomItem.getID();
			messageClient(%client,'MsgItemPickup','',%n,PushBroomItem.getID());
			%numTools = %n + 1;
		}
		commandToClient(%client, 'PlayGui_CreateToolHud', %numTools);

		%client.player.playAudio(0, "RLitem" @ getRandom(1, 3));
	}
	$RL_LavaSpeed = $RL::ResetCount;
	messageAll('', "\c6Lava Speed:\c4" SPC $RL_LavaSpeed);
	
	// made hyper rounds a bit less ridiculous and made them happen more often -- TBP

	if($RL_HyperRoundCount == 12)
	{
		messageAll('', "\c4Hyper Round\c6! The lava this round is 5x the normal speed!");
		$RL_LavaSpeed = $RL_LavaSpeed * 5;
		$RL_HyperRoundCount = 0;
	}
	$RL_HyperRoundCount++;

	PushbroomItem.canDrop = false;
	goldenPushBroomItem.canDrop = false;
	powerStaffItem.canDrop = false;
	PrintGun.canDrop = false;
	serverPlay2D(roundBeginSound);

	RL_LavaSound();
}

function RL_BeginSpeedTime()
{
	cancel($RL_SpeedTime);
	$RL_SpeedTime = schedule(60000, 0, RL_SpeedTime);
}

function RL_SpeedTime()
{
	cancel($RL_SpeedTime);
	serverPlay2D(BrickPlantSound);
	$RL_LavaSpeed += 1;
	$RL_SpeedTime = schedule(30000, 0, RL_SpeedTime);

	// think Zapk forgot something, lava speed was never announced -- TBP
	messageAll('', "\c6Lava Speed: \c4" @ $RL_LavaSpeed);
}

function RL_LavaContinue(%a)
{
	cancel($RL_LavaContinue);
	if(%a >= 10000)
	{
		return;
	}
	
	RL_SetLavaHeight(%a);
	
	//		GREENLIGHT STUFF
	
	if(!$RL_LavaColor_GoingBack && !$RL_LavaColor_GoingFor)
	{
		$RL_LavaColor_GoingFor = true;
	}
	
	if($RL_LavaColor_GoingBack)
	{
		$RL_LavaColor_Step -= 0.001;
		if($RL_LavaColor_Step <= 0)
		{
			$RL_LavaColor_GoingBack = false;
			$RL_LavaColor_GoingFor = true;
		}
	}
	
	if($RL_LavaColor_GoingFor)
	{
		$RL_LavaColor_Step += 0.001;
		if($RL_LavaColor_Step >= 1)
		{
			$RL_LavaColor_GoingFor = false;
			$RL_LavaColor_GoingBack = true;
		}
	}
	
	%color = rgbGradient($RL_LavaColor_Step, "1 0.1 0", "1 0.4 0");
	
	setEnvironment("WaterColor", "1 1 1 1");
	setEnvironment("WaterColor", %color SPC 1);
	setEnvironment("UnderWaterColor", "1 1 1 1");
	setEnvironment("UnderWaterColor", %color SPC 1);
	
	%font = "<font:impact:25>";
	%level = RL_GetLevelName($RL::CurrentLevel);
	%height = mFloatLength(%a, 2);
	
	%players = $RL_PeopleAllive;
	if(%players <= 1)
	{
		%players = "All";
	}
	
	RL_DoLavaMessage(%a);
	//bottomPrintAll(%font @ "<just:center>\c4Map:\c6" SPC %level SPC "\c4Round:\c6" SPC $RL::ResetCount @ "/" @ $Pref::Server::RisingLava::RoundLimit NL "\c4Speed:\c6" SPC $RL_LavaSpeed SPC "\c4Players:\c6" SPC %players SPC "\c4Level:\c6" SPC %height, 2, 1);
	//for(%a = 0; %a < ClientGroup.getCount(); %a++)
	//{
	//	%client = ClientGroup.getObject(%a);
	//	bottomPrint(%client, %font @ "<just:center>\c4Map:\c6" SPC %level SPC "\c4Round:\c6" SPC $RL::ResetCount @ "/" @ $Pref::Server::RisingLava::RoundLimit NL "\c4Speed:\c6" SPC $RL_LavaSpeed SPC "\c4Players:\c6" SPC %players SPC "\c4Level:\c6" SPC %height, 2, 1);
	//}
	
	%additive = $RL_LavaSpeed * 0.00332;
	
	$RL_LavaContinue = schedule(33, 0, RL_LavaContinue, %a + %additive);
}

function RL_LavaSound()
{
	cancel($RL_LavaSound);

	%height = getWord(waterPlane.getTransform(), 2);

	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		%obj = %client.getControlObject();

		if(!isObject(%client) || !isObject(%obj))
		{
			continue;
		}

		%xy = getWords(%obj.getPosition(), 0, 1);

		%pos = %xy SPC %height;

		%client.play3D(lavaSound, %pos);
	}

	$RL_LavaSound = schedule(4000, 0, RL_LavaSound);
}

function RL_DoLavaMessage(%a)
{
	%players = $RL_PeopleAllive;
	if(%players <= 1)
	{
		%players = "All";
	}
	
	%font = "<shadow:1:1><font:impact:25>";
	%level = "rl_" @ strlwr( strreplace(RL_GetLevelName($RL::CurrentLevel), " ", "_") );
	%height = mFloatLength(%a, 2);
	
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		
		if(!isObject(%client.player))
		{
			%position = "Heaven";
		}
		
		else
		{
			%position = getWord(%client.player.getPosition(), 2);
			%position = mFloor(%position);
			if(%position == 0)
			{
				%position = "Ground";
			}
		}
		
		//		GREENLIGHT STUFF
		if(isObject(%player = %client.player))
		{
			%health = %player.getDatablock().maxDamage - %player.getDamageLevel();
		}
		else
		{
			%health = "Dead";
		}
		%bp = %font @ "<just:left>\c4" @ %level @ "<just:right>\c6" @ $RL::ResetCount @ "/" @ $Pref::Server::RisingLava::RoundLimit;
		%bp = %bp NL "<just:left>\c6Alive:\c4" SPC %players @ "<just:right>\c6" @ %height;
		%bp = %bp NL "<just:left>\c6Health:\c4" SPC %health @ "<just:right>\c6Score:\c4" SPC %client.score;

		bottomPrint(%client, %bp, 2, 1);

		commandToClient(%client, 'RLC_Shop_SetMoney', $RL_PlayerScore_Money_[%client.bl_id] ? $RL_PlayerScore_Money_[%client.bl_id] : 0);
	}
}

function RL_GetLevelName(%num)
{
	%displayName = $RL::Level[%num];
	%displayName = strReplace(%displayName, "Add-Ons/RisingLava_", "");
	%displayName = strReplace(%displayName, "/save.bls", "");
	%displayName = strReplace(%displayName, "_", " ");
	return %displayName;
}

function RL_CancelLoops()
{
	cancel($RL_CountdownTick);
	cancel($RL_StartRisingSequenceSchedule);
	cancel($RL_LavaContinue);
	cancel($RL_LavaSound);
	cancel($RL_SpeedTime);
}

// removed the check command, dragging your window around also freezes your ping, simulating actual lag -- TBP

function serverCmdWho(%client)
{
	messageClient(%client, '', "\c4Players Alive\c6:");
	for(%a = 0; %a < %client.minigame.numMembers; %a++)
	{
		%miniPlayer = %client.minigame.member[%a];
		if(isObject(%miniPlayer.player))
		{
			%num++;
			messageClient(%client, '', "\c4" @ %num @ "\c6. " @ %miniPlayer.name);
		}
	}
}

// check phase 2 was here -- TBP

function GameConnection::savePing(%this, %num)
{
	%this.TempPing[%num] = %this.getPing();
}

function serverCmdHelp(%client)
{
	//		GREENLIGHT STUFF
	messageClient(%client, '', "\c4Rising Lava \c6by \c4Zapk");
	messageClient(%client, '', "\c6  - \c4help\c6 - Display a list of commands.");
	messageClient(%client, '', "\c6  - \c4rules\c6 - See the server rules.");
	messageClient(%client, '', "\c6  - \c4who\c6 - Prints a list of all living players.");
	if(%client.isAdmin) {
		messageClient(%client, '', "\c6  - \c4setLevel [number]\c6 - Sets the level to the number given.");
		messageClient(%client, '', "\c6  - \c4nextLevel\c6 - Skips the current level.");
	}
}

function Player::setLight(%obj, %light)
{
	if(isObject(%light))
	{
		if(isObject(%obj.light))
		{
			%obj.light.delete();
		}
	
		%obj.light = new FxLight()
		{
			dataBlock = %light;
			enable = true;
			iconsize = 1;
			player = %obj;
		};
	
		if(isObject(%obj.light))
		{
			%obj.light.attachToObject(%obj);
		}
	}
}

function rgbToHex( %rgb )
{
	%r = _compToHex( 255 * getWord( %rgb, 0 ) );
	%g = _compToHex( 255 * getWord( %rgb, 1 ) );
	%b = _compToHex( 255 * getWord( %rgb, 2 ) );
 
	return %r @ %g @ %b;
}

function _compToHex( %comp )
{
	%left = mFloor( %comp / 16 );
	%comp = mFloor( %comp - %left * 16 );
	
	%left = getSubStr( "0123456789ABCDEF", %left, 1 );
	%comp = getSubStr( "0123456789ABCDEF", %comp, 1 );
	
	return %left @ %comp;
}

function rgbGradient(%step, %c1, %c2)
{
	%r1 = getWord(%c1, 0);
	%g1 = getWord(%c1, 1);
	%b1 = getWord(%c1, 2);

	%r2 = getWord(%c2, 0);
	%g2 = getWord(%c2, 1);
	%b2 = getWord(%c2, 2);

	%r3 = %r1 + %step * (%r2 - %r1);
	%g3 = %g1 + %step * (%g2 - %g1);
	%b3 = %b1 + %step * (%b2 - %b1);
	
	return %r3 SPC %g3 SPC %b3;
}

$Water::Kill = true;

$DefaultMinigame.playerDatablock = PlayerNoJet;