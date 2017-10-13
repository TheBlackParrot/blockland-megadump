exec("./env.cs");

if(isFunction(RTB_registerPref)) {
//if(isFile("Add-Ons/System_ReturnToBlockland/server.cs")) { -- why the file?
	RTB_registerPref("Cycle Maps Automatically","Creep Mod","Creep::UseRounds","bool","GameMode_Super_Creeper",1,0,0,0);
	RTB_registerPref("Disable Shop","Creep Mod","Creep::ShopAllowed","bool","GameMode_Super_Creeper",1,0,0,0);
	RTB_registerPref("Disable Bombs","Creep Mod","Creep::NoBombs","bool","GameMode_Super_Creeper",0,0,0,0);
	RTB_registerPref("CreepKill+ Cost Factor","Creep Mod","Creep::KillUpgradeCostFactor","int 1 99999","GameMode_Super_Creeper",500,0,0,0);
} else {
	$Creep::KillUpgradeCostFactor = 500;
	$Creep::UseRounds = 1;
}

$Creep::Init = 1;

function SC_Reload()
{
	exec("Add-Ons/GameMode_Super_Creeper/main.cs");
	messageAll('', "\c6The \c4Super Creeper \c6code has been reloaded by the host.");
}

//		MAIN GAME MODE CODE
function SC_BuildLevelList()
{
	%pattern = "Add-Ons/SuperCreeper_*/save.bls";
	
	$SuperCreeper::numLevels = 0;
	
	%file = findFirstFile(%pattern);
	while(%file !$= "")
	{
		$SuperCreeper::Level[$SuperCreeper::numLevels] = %file;
		$SuperCreeper::numLevels++;

		%file = findNextFile(%pattern);
	}
}

function SC_DumpLevelList()
{
	echo("");
	if($SuperCreeper::numLevels == 1)
		echo("1 level");
	else
		echo($SuperCreeper::numLevels @ " levels");
	for(%i = 0; %i < $SuperCreeper::numLevels; %i++)
	{
		%displayName = $SuperCreeper::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/SuperCreeper_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		%displayName = strReplace(%displayName, "_", " ");

		if(%i == $SuperCreeper::CurrentLevel)
			echo(" >" @ %displayName);
		else
			echo("  " @ %displayName);
	}
	echo("");
}

function SC_NextLevel()
{
	$SuperCreeper::CurrentLevel = mFloor($SuperCreeper::CurrentLevel);
	$SuperCreeper::CurrentLevel++;
	$SuperCreeper::CurrentLevel = $SuperCreeper::CurrentLevel % $SuperCreeper::numLevels;

	$SuperCreeper::ResetCount = 0;

	SC_LoadLevel_Phase1($SuperCreeper::Level[$SuperCreeper::CurrentLevel]);
}

function SC_LoadLevel_Phase1(%filename)
{
	//$DefaultMinigame = Slayer.Minigames.getHostMinigame(); // hotfix

	%mg = $DefaultMiniGame;
	if(!isObject(%mg))
	{
		error("ERROR: SC_LoadLevel( " @ %filename  @ " ) - default minigame not found");
		return;
	}

	%mg.loading = true;

	for(%i = 0; %i < %mg.numMembers; %i++)
	{
		%client = %mg.member[%i];
		%player = %client.player;

		%camera = %client.camera;
		%camera.setFlyMode();
		%camera.mode = "Observer";
		%client.setControlObject(%camera);

		if(isObject(%player))
			%player.delete();
	}
	
	//clear all bricks 
	if(isObject(CreepGroup)) {
		CreepGroup.ClearSpawns();
	}
	
	// note: this function is deferred, so we'll have to set a callback to be triggered when it's done
	BrickGroup_888888.chaindeletecallback = "SC_LoadLevel_Phase2(\"" @ %filename @ "\");";
	BrickGroup_888888.chaindeleteall();
}

function SC_LoadLevel_Phase2(%filename)
{
	echo("Loading Super Creeper save " @ %filename);

	//%displayName = %filename;
	//%displayName = strReplace(%displayName, "Add-Ons/SuperCreeper_", "");
	//%displayName = strReplace(%displayName, "/save.bls", "");
	//%displayName = strReplace(%displayName, "_", " ");

	// fileBase, my dude
	%displayName = strReplace(fileBase(%displayName), "_", " ");
	
	%loadMsg = "\c6Now loading \c4" @ %displayName;

	//read and display credits file, if it exists
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
	
	// load super creeper config
	// %configFilename = filePath(%fileName) @ "/config.cs";
	// exec(%configFilename);

	// let server owners dictate what they want, fuck off -- TBP
	
	// show map config messages
	if($Creep::TimeLimit > 0) {
		messageAll('', "\c6- The time limit for this map is \c4" @ $Creep::TimeLimit @ "\c6 minutes.");
	} else {
		messageAll('', "\c6- There is no time limit for this map.");
	}
	
	if($Creep::UseShop) {
		messageAll('', "\c6- The shop is \c4On \c6for this map.");
	} else {
		messageAll('', "\c6- The shop is \c4Off \c6for this map.");
	}
	
	//load environment if it exists
	%envFile = filePath(%fileName) @ "/environment.txt"; 
	if(isFile(%envFile))
	{  
		loadEnvironment(%envFile);
	}
	
	//let the gamemode start
	if($Creep::Init == 1) {
		$Creep::Init = 0;
	}
	
	//load save file
	schedule(10, 0, serverDirectSaveFileLoad, %fileName, 3, "", 2, 1);
}


//		COMMANDS FOR THE MANS
function serverCmdLevelList(%client)
{
	if($Sim::Time - %client.lastUsed["levellist"] < 5) {
		return;
	}
	%client.lastUsed["levellist"] = $Sim::Time;

	for(%i = 0; %i < $SuperCreeper::numLevels; %i++)
	{
		%displayName = $SuperCreeper::Level[%i];
		%displayName = strReplace(%displayName, "Add-Ons/SuperCreeper_", "");
		%displayName = strReplace(%displayName, "/save.bls", "");
		%displayName = strReplace(%displayName, "_", " ");

		if(%i == $SuperCreeper::CurrentLevel)
			messageClient(%client, '', "\c4" @ %i @ "\c6. " @ %displayName SPC "(\c4Selected\c6)");
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
	messageAll('', "\c4" @ %client.name SPC "\c6reloaded the Super Creeper level cache.");
	setModPaths(getModPaths());
	SC_BuildLevelList();
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

	if(%i < 0 || %i > $SuperCreeper::numLevels)
	{
		messageClient(%client, '', "serverCmdSetLevel() - out of range index");
		return;
	}

	messageAll( 'MsgAdminForce', '\c4%1\c6 changed the level', %client.getPlayerName());
	
	$SuperCreeper::CurrentLevel = %i - 1;
	SC_NextLevel();
}
function serverCmdNextLevel(%client, %i)
{
	if(!%client.isAdmin)
		return;

	messageAll( 'MsgAdminForce', '\c4%1\c6 changed the level', %client.getPlayerName());
	
	SC_NextLevel();
}

function serverCmdHelp(%client) {
	MessageClient(%client,'',"\c2-Super Creeper Commands-");
	MessageClient(%client,'',"\c2/help\c3 - shows this message.");
	MessageClient(%client,'',"\c2/rules\c3 - shows the rules.");
	MessageClient(%client,'',"\c2/upgrade\c3 - Allows you to buy items for use in certain gamemodes.");
	MessageClient(%client,'',"\c2/levellist\c3 - Shows all the maps available.");
	
	if(%client.isAdmin || %client.isSuperAdmin) {
		MessageClient(%client,'',"\c2/nextlevel\c3 - go to the next map, skipping all other rounds of this one.");
		MessageClient(%client,'',"\c2/reloadlevels\c3 - loads new levels from Add-Ons without restarting the server.");
		MessageClient(%client,'',"\c2/setlevel\c3 - loads a level. Use a levelID.");
		MessageClient(%client,'',"\c2/forcekill\c3 - get rid of people who are minorly breaking the rules.");
	}
}

function serverCmdRules(%client) {
	MessageClient(%client,'',"\c2== \c6Rules ==");
	MessageClient(%client,'',"\c41.\c6 Do not floathack.");
	MessageClient(%client,'',"\c42.\c6 Do not deliberately hold up the game.");
	MessageClient(%client,'',"\c43.\c6 Do not block other players in non-competitive modes.");
}

function serverCmdUpgrade(%client, %buy) {
	if(%client.points $= "") {
		return;
	}
	
	if(!$Creep::ShopAllowed) {
		MessageClient(%client,'',"\c2The shop is disabled.");
		return;
	}

	%nextCreepKillCost = %client.creepKillLvl*$Creep::KillUpgradeCostFactor;
	%nextCreepBombCost = %client.creepBombLvl*($Creep::KillUpgradeCostFactor*1.5);
	
	if(%client.creepKillLvl == 4) {
		%nextCreepKillCost = "MAXED";
	}
	
	if(%client.creepBombLvl == 2) {
		%nextCreepBombCost = "MAXED";
	}
	
	if(%buy $= "") {
		MessageClient(%client,'',"\c2-UPGRADES-");
		//MessageClient(%client,'',"\c3The shop is work in progress. Don't get too attached to items you buy.");
		MessageClient(%client,'',"\c3Use \c2/upgrade [name] \c3to buy:");
		MessageClient(%client,'',"\c2CreepKill \c3- Upgrade your CreepKill item so you can destroy creeper more quickly. (\c2" @ %nextCreepKillCost @ "\c3)");
		
		if(!$Creep::NoBombs) {
			MessageClient(%client,'',"\c2CreepBomb \c3- Anti-Creeper grenades. (\c2" @ %nextCreepBombCost @ "\c3)");
		}
		
		return;
	}
	
	if(%buy $= "CreepKill") {
		if(%nextCreepKillCost $= "MAXED") {
			MessageClient(%client,'',"\c2Your CreepKill is maxed out!");
			return;
		}
		
		if(%client.points >= %nextCreepKillCost) {
			%client.points -= %nextCreepKillCost;
			%client.creepKillLvl++;
			
			%client.UpdateSuperCreeperScore();
			giveNewCreepKill(%client, %client.creepKillLvl + 1);
			
			MessageClient(%client,'',"\c2Your CreepKill item has been upgraded!");
		} else {
			MessageClient(%client,'',"\c2You need more points. Try to win some games.");
		}
	}
	
	if(%buy $= "CreepBomb" && !$Creep::NoBombs) {
		if(%nextCreepBombCost $= "MAXED") {
			MessageClient(%client,'',"\c2Your CreepBomb is maxed out!");
			return;
		}
		
		if(%client.points >= %nextCreepBombCost) {
			%client.points -= %nextCreepBombCost;
			%client.creepBombLvl++;
			
			%client.UpdateSuperCreeperScore();
			giveNewCreepBomb(%client, %client.creepBombLvl + 1);
			
			MessageClient(%client,'',"\c2Your CreepBomb item has been upgraded!");
		} else {
			MessageClient(%client,'',"\c2You need more points. Try to win some games.");
		}
	}
}

function giveNewCreepKill(%client,%level) {
	%player = %client.player;
	if(!isObject(%player)) return;
	
	// remove old CreepKill
	switch(%level) {
		case 2:
			%player.removeItem(CreepKillWeakGunItem);
		case 3:
			%player.removeItem(CreepKillGunItem);
		case 4:
			%player.removeItem(CreepKillStrongGunItem);
	}
	
	// give new CreepKill
	switch(%level) {
		case 2:
			%player.addItem(nameToID(CreepKillGunItem),%client);
		case 3:
			%player.addItem(nameToID(CreepKillStrongGunItem),%client);
		case 4:
			%player.addItem(nameToID(CreepKillVStrongGunItem),%client);
	}
}

function giveNewCreepBomb(%client,%level) {
	%player = %client.player;
	if(!isObject(%player)) return;
	
	// remove old CreepBomb (if it's already been used the command will handle it)
	if(%level == 2) {
		%player.removeItem(CreepBombItem);
	}
	
	// give new CreepBomb
	switch(%level) {
		case 1:
			%player.addItem(nameToID(CreepBombItem),%client);
		case 2:
			%player.addItem(nameToID(CreepBomb2Item),%client);
	}
}

function serverCmdForceKill(%client, %victim_search) {
	// totally rewritten this command -- TBP

	if(!%client.isAdmin) {
		return;
	}

	%victim = findClientByName(%victim_search);
	if(!isObject(%victim)) {
		%victim = findClientByBL_ID(%victim_search);
		if(!isObject(%victim)) {
			messageClient(%client, '', "Could not find anyone using \"" @ %victim_search @ "\"");
			return;
		}
	}

	if(isObject(%victim.player)) {
		%victim.player.kill();
		messageClient(%victim, '', "You were forcekilled by an admin.");
	}
}

function MinigameSO::giveCreepBombs(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%player = %client.player;

		if(%client.creepBombLvl > 0 && !$Creep::NoBombs && isObject(%client.player)) {
			if(%client.creepBombLvl == 1) {
				%player.addItem(nameToID(CreepBombItem),%client);
			} else {
				%player.addItem(nameToID(CreepBomb2Item),%client);
			}
		}
	}

	messageAll('', "\c4Creep Bombs \c6have been given out!");
}

function MinigameSO::giveCreepKills(%this) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%player = %client.player;

		if(isObject(%client.player)) {
			if($Creep::UseShop) {
				switch(%client.creepKillLvl) {
					case 1:
						%player.addItem(nameToID(CreepKillWeakGunItem),%client);
					case 2:
						%player.addItem(nameToID(CreepKillGunItem),%client);
					case 3:
						%player.addItem(nameToID(CreepKillStrongGunItem),%client);
					case 4:
						%player.addItem(nameToID(CreepKillVStrongGunItem),%client);
					default:
						%player.addItem(nameToID(CreepKillWeakGunItem),%client);
				}
			} else { // if not just give everyone the bog standard
				%player.addItem(nameToID(CreepKillGunItem),%client);
			}
		}
	}

	messageAll('', "\c4Creep Kills \c6have been given out!");	
}

//	PACKAGE
deactivatepackage(GameModeSuperCreeperPackage);
package GameModeSuperCreeperPackage
{
	function fxDTSBrick::setVehicle(%obj, %data, %client) { return; }
	function fxDTSBrick::setItem(%obj, %data, %client) { return; }

	//this is called when save loading finishes
	function GameModeInitialResetCheck()
	{
		Parent::GameModeInitialCheck();

		//if there is no level list, attempt to create it
		if($SuperCreeper::numLevels == 0)
			SC_BuildLevelList();
		
		//if levellist is still empty, there are no levels
		if($SuperCreeper::numLevels == 0)
		{
			echo("\c6No Super Creeper levels available!");
			return;
		}

		if($SuperCreeper::Initialized)
			return;

		$SuperCreeper::Initialized = true;
		$SuperCreeper::CurrentLevel = -1;
				
		SC_NextLevel();
	}

	//when we're done loading a new level, reset the minigame
	function ServerLoadSaveFile_End()
	{
		$DefaultMinigame.loading = false;

		Parent::ServerLoadSaveFile_End();
		
		//new level has loaded, reset minigame
		if($DefaultMiniGame.numMembers > 0) //don't bother if no one is here (this also prevents starting at round 2 on server creation)
			$DefaultMiniGame.scheduleReset(); //don't do it instantly, to give people a little bit of time to ghost
	}
	function MiniGameSO::Reset(%obj, %client)
	{
		if(%this.loading) {
			return;
		}

		if(!$Creep::Init) {
			SC_CancelPostGameSchedules();
			
			// clean up the previous round's mess
			if($Creep::active) {
				stopCreeper();
			}
			
			ClearCreeper();
			
			//make sure this value is an number
			$Creep::RoundLimit = mFloor($Creep::RoundLimit);

			//count number of minigame resets, when we reach the limit, go to next level
			if(%obj.numMembers >= 0)
			{
				$SuperCreeper::ResetCount++;
			}
			
			
			if($Creep::UseRounds) {
				if($SuperCreeper::ResetCount > $Creep::RoundLimit)
				{
					$SuperCreeper::ResetCount = 0;
					SC_NextLevel();
				}
				else
				{
					messageAll('', "\c6Beginning round \c4" @ $SuperCreeper::ResetCount @ " \c6of \c4" @ $Creep::RoundLimit);
					
					Parent::Reset(%obj, %client);
				}
			} else {
				messageAll('', "\c6Beginning round \c4" @ $SuperCreeper::ResetCount);
				
				Parent::Reset(%obj, %client);
			}
			
			// get round type
			$Creep::RoundType = getSpecialRound();
			
			if($Creep::RoundType != 0) {
				switch($Creep::RoundType) {
					case 1:
						MessageAll('MsgAdminForce',"\c0NIGHTMARE ROUND\c6: The Creeper will go directly for players - and have a lot more health!");
					case 2:
						MessageAll('MsgAdminForce',"\c2INSTA-GIB ROUND\c6: The Creeper will grow a lot faster - but won't take much to kill!");
					case 3:
						MessageAll('MsgAdminForce',"\c3IMMUNITY ROUND\c6: The Creeper cannot be stopped - only slowed down!");
				}
			}

			for(%i=0;%i<ClientGroup.getCount();%i++) {
				%c = ClientGroup.getObject(%i);
				%c.saveData();
			}
			
			schedule(33, 0, SC_PostGameSchedules);
		}
	}
	
	function GameConnection::spawnPlayer(%client) {
		Parent::spawnPlayer(%client);
		%client.updateSuperCreeperScore();
		
		%player = %client.player;
		
		// CREEPER VARIABLES // 
		%client.kills = 0;
		%player.nochase = 0;

		// ironically enough, none of the default maps for this used any creeper gamemode other than 0, so i just took out lives
		// TODO: strip the game of anything not relating to gamemode 0
		
		// LOCATION //
		// players spawn on bricks named "spawn"
		%pos = getRandomPosOnBrick(getRandomSpawnBrick());
		%player.settransform(%pos);
		%player.setvelocity("0 0 0");
		return;
	}

	function MinigameSO::checkLastManStanding(%this) {
		if(%this.loading) {
			return;
		}

		if($SuperCreeper::Init $= "") {
			SC_NextLevel();
			$SuperCreeper::Init = true;
			return;
		}

		for(%i=0;%i<%this.numMembers;%i++) {
			%client = %this.member[%i];
			if(isObject(%client.player)) {
				return;
			}
		}

		%this.endRound();

		return; // we need to do our own thing
	}
	
	// this USED TO BE a slayer function, this is NOT a default function, i just left it here because i'm lazy -- TBP
	function MinigameSO::endRound(%this,%winner,%resetTime) {
		// clean up the previous round's mess
		if($Creep::active) {
			stopCreeper();
		}
		
		ClearCreeper();
		
		SC_CancelPostGameSchedules();

		if(%this.loading) {
			return;
		}
		
		// is anyone still alive?
		%count = 0;
		for(%i=0; %i < %this.numMembers; %i++)
		{
			%cl = %this.member[%i];
			if(isObject(%cl.player))
			{
				// give the living players their reward
				// also, singular they please T_T -- TBP
				messageAll('', "\c6- \c4" @ %cl.name SPC "\c6SURVIVED with" SPC %cl.kills SPC "kills. They receive\c4" SPC %cl.kills*5 SPC "\c6points.");
				%cl.points += ($Creep::RoundType > 0 ? %cl.kills*15 : %cl.kills*5);

				%count++;
			} else {
				messageAll('', "\c6- \c4" @ %cl.name SPC "\c6died with" SPC %cl.kills SPC "kills. They receive\c4" SPC mFloor(%cl.kills*0.5) SPC "\c6points.");
				%cl.points += mFloor(%cl.kills*0.5);
			}
			
			%cl.updateSuperCreeperScore();
		}

		if(!%count) {
			messageAll('', "<color:9b65d8>The Creeper \c6wins this round!");
		}

		messageAll('', "\c6Resetting in 7 seconds...");
		cancel(%this.resetSched);
		%this.resetSched = %this.schedule(7000, reset);
	}
	
	//function Slayer_MinigameSO::victoryCheck_Lives(%this) -- removed, we can do this in endRound without slayer
	
	//function Slayer_MinigameSO::victoryCheck_Creeper(%this) -- this checked to see how big the creeper got, wasn't used in gamemode 0

	function GameConnection::AutoAdmincheck(%this)
	{
		%r = parent::AutoAdmincheck(%this);
		%this.loadData();
		%this.updateSuperCreeperScore();
		return %r;
	}
};
activatePackage(GameModeSuperCreeperPackage);

function getRandomPosOnBrick(%brick)
{
	%box = %brick.getworldbox();
	%x = getrandom(0, (mabs(getword(%box, 0) - getword(%box, 3)) - 1.5) * 500) / 1000 * (getrandom(0, 1) ? -1 : 1);
	%y = getrandom(0, (mabs(getword(%box, 1) - getword(%box, 4)) - 1.5) * 500) / 1000 * (getrandom(0, 1) ? -1 : 1);
	%z = mabs(getword(%box, 2) - getword(%box, 5)) / 2;
	return vectoradd(%brick.getworldboxcenter(), %x SPC %y SPC %z);
}

function getRandomSpawnBrick()
{
	%brick = BrickGroup_888888.NTObject_["spawn", getRandom(0, BrickGroup_888888.NTObjectCount_["spawn"]-1)];
	return %brick;
}

function SC_PostGameSchedules()
{
	%time = $Creep::StartTime;
	
	$SC::PostGameSchedule[0] = schedule(1, 0, messageAll, '', "\c6The Creeper spores have started to pollenate. You have approximately \c4" @ %time SPC "\c6seconds to prepare!");
	$SC::PostGameSchedule[1] = schedule(%time*1000, 0, startCreeper);
	$SC::PostGameSchedule[2] = schedule((%time*1000)+1000, 0, messageAll, '', "\c6The Creeper is spreading! Run for your lives!");
	$SC::PostGameSchedule[3] = $DefaultMinigame.schedule($Creep::TimeLimit * 60000, endRound);
	$SC::PostGameSchedule[4] = $DefaultMinigame.schedule((%time*1000)+1000 + 60000, giveCreepBombs);
	$SC::PostGameSchedule[5] = $DefaultMinigame.schedule((%time*1000)+1000 + 7000, giveCreepKills);
	$SC::GameScheds = 6;

	for(%i=1;%i<$Creep::TimeLimit;%i++) {
		$SC::PostGameSchedule[$SC::GameScheds] = schedule(%i*60000, 0, messageAll, '', "\c3" @ $Creep::TimeLimit - %i SPC "minutes \c5remaining");
		$SC::GameScheds++;
	}

	if(!$DefaultMinigame.loading) {
		$DefaultMinigame.respawnAll();
	}
}

function SC_CancelPostGameSchedules() {
	for(%a = 0; %a < $SC::GameScheds; %a++) {
		cancel($SC::PostGameSchedule[%a]);
	}
}

// import the addItem code
function Player::addItem(%player,%image,%client)
{
   for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
   {
	  %tool = %player.tool[%i];
	  if(%tool == 0)
	  {
		 %player.tool[%i] = %image;
		 %player.weaponCount++;
		 messageClient(%client,'MsgItemPickup','',%i,%image);
		 break;
	  }
   }
}

function Player::removeItem(%this,%item)
{
   if(!isObject(%this) || !isObject(%item.getID()))
	  return;
   for(%i=0;%i<%this.getDatablock().maxTools;%i++)
   {
	  if(!isObject(%this.tool[%i])) continue;
	  // ^ this line of code not being in the mod i ripped it from was the reason that mod caused console errors
	  
	  %tool=%this.tool[%i].getID();
	  if(%tool==%item.getID())
	  {
		 %this.tool[%i]=0;
		 messageClient(%this.client,'MsgItemPickup','',%i,0);
		 if(%this.currTool==%i)
		 {
			%this.updateArm(0);
			%this.unMountImage(0);
		 }
	  }
   }
}

// player database
// this ~~is~~ was the server code that interfaces with zebase.

// please do not EVER use a mass load of script objects just to save your damn level and score values -- TBP

function GameConnection::UpdateSuperCreeperScore(%this) {
	%this.score = %this.points;
}

function getSpecialRound() {
	%rand = getRandom(0, 49);
	
	if(%rand < 40) {
		return 0;
	} else {
		// special round!
		// 1 = nightmare, 2 = instagib, 3 = unstoppable creeper
		if(%rand < 43) {
			return 1;
		}
		else if(%rand < 46) {
			return 2;
		}
	}

	return 0;
}