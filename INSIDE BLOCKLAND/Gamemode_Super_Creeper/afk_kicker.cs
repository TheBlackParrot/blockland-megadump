// The following is just code I nicked from Greek2Me's AFK Kicker.
// I added this so players who need it won't have to crowbar it into the gamemode, since it's kind of a necissity.

// +------------------+
// | AFK Kicker       |
// +------------------+
// | Greek2me (11902) |
// +------------------+

if(isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook)
		exec("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs");

	RTB_registerPref("AFK-Kicker","Creep Mod","$Pref::AFKK::enabled","bool","Gamemode_Super_Creeper",1,0,0);
	RTB_registerPref("AFK-Kicker Time","Creep Mod","$Pref::AFKK::afkTime","int 1 300","Gamemode_Super_Creeper",5,0,0);
	RTB_registerPref("Kick Only When Full","Creep Mod","$Pref::AFKK::fullOnly","bool","Gamemode_Super_Creeper",1,0,0);
	$Pref::AFKK::minigameOnly = 0;
	RTB_registerPref("Admin Bypass Level","Creep Mod","$Pref::AFKK::bypassLevel","list None 0 SuperAdmin 1 Admin 2","Gamemode_Super_Creeper",1,0,0);
}
else
{
	$Pref::AFKK::enabled = 1;
	$Pref::AFKK::afkTime = 5;
	$Pref::AFKK::fullOnly = 1;
	$Pref::AFKK::minigameOnly = 0;
	$Pref::AFKK::bypassLevel = 1;
}

function AFKK_Tick()
{
	cancel($AFKK::tickSchedule);
	$AFKK::tickSchedule = schedule(60000,0,AFKK_Tick);

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
		if($Pref::AFKK::bypassLevel == 2 && %cl.isAdmin)
			continue;
		if($Pref::AFKK::bypassLevel == 1 && %cl.isSuperAdmin)
			continue;

		//Are they playing in a minigame?
		if($Pref::AFKK::minigameOnly && !isObject(getMinigameFromObject(%cl)))
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
		if(!$Pref::AFKK::enabled)
			continue;

		//Is the server full?
		if($Pref::AFKK::fullOnly && %playerCount < $Pref::Server::MaxPlayers)
			continue;

		//They're AFK, so give them the boot.
		if(%cl.afkTick >= $Pref::AFKK::afkTime + 1 && %cl.afkWarned)
		{
			messageAll('',"\c3CONSOLE \c2kicked \c3" @ %cl.getPlayerName() @ "\c2(ID:" SPC %cl.getBLID() @ ") for being AFK.");
			%cl.delete("You were kicked from the server for being <a:www.urbandictionary.com/define.php?term=Afk>AFK</a>.\nYou may rejoin at any time.");
		}
		else if(%cl.afkTick >= $Pref::AFKK::afkTime) //notify client that they're about to be kicked
		{
			commandToClient(%cl,'MessageBoxOKCancel',"ALERT","You will be automatically kicked from the server in <sPush><font:Arial Bold:14>1 MINUTE<sPop> for being AFK unless you click \"OK\" below.\n(Clicking \"Cancel\" is also okay if you want to be a smartass about it)",'notAFK');
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

package AFKK
{
	function serverCmdMessageBoxCancel(%client)
	{
		parent::serverCmdMessageBoxCancel(%client);

		AFKK_clientActive(%client);
	}

	function serverCmdMessageSent(%client,%msg)
	{
		parent::serverCmdMessageSent(%client,%msg);

		AFKK_clientActive(%client);
	}

	function serverCmdTeamMessageSent(%client,%msg)
	{
		parent::serverCmdTeamMessageSent(%client,%msg);

		AFKK_clientActive(%client);
	}

	function serverCmdStartTalking(%client)
	{
		parent::serverCmdStartTalking(%client);

		AFKK_clientActive(%client);
	}

	function serverCmdStopTalking(%client)
	{
		parent::serverCmdStopTalking(%client);

		AFKK_clientActive(%client);
	}
};
activatePackage(AFKK);

//start the schedule loop
AFKK_Tick();