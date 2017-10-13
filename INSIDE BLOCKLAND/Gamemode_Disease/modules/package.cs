%package = "DiseasePackage";

if(isPackage(%package))
{
	deactivatePackage(%package);
}

package DiseasePackage
{
	function Player::ActivateStuff(%this)
	{	
		Parent::ActivateStuff(%this);
		
		if(!%this.isInfected)
		{
			return;
		}

		%target = ContainerRayCast(%eye = %this.getEyePoint(), vectorAdd(%eye, vectorScale(%this.getEyeVector(),2)), $TypeMasks::PlayerObjectType, %this);
		
		if(%target)
		{
			%target = %target.getID();
			
			if(%target.getClassName() !$= "Player")
			{
				return;
			}
			
			%client = %this.client;
			%targetClient = %target.client;
			
			if(!isObject(%targetClient))
			{
				return;
			}
			
			if(%target.isInfected)
			{
				centerPrint(%client, "\c3" @ %targetClient.name @ " \c0is already infected with the disease.", 2);
			}
			else
			{
				%target.infect(%client);
			}
		}
	}
	
	function MinigameSO::scheduleReset(%this, %time)
	{
		if($Disease::Resetting)
		{
			return;
		}
		$Disease::Resetting = true;
		if(!%time)
		{
			%time = 10000;
		}
		Parent::scheduleReset(%this, %time);
		Disease_onRoundEnd();
	}
	
	function MinigameSO::reset(%this, %client)
	{
		Parent::reset(%this, %client);
		%this.PlayerDatablock = PlayerNoJet;
		$Disease::Resetting = false;
		$Disease::isStarting = true;
		schedule(300, 0, Disease_removeHelicopter);
		schedule(600, 0, Disease_onStart);
	}
	
	function GameConnection::spawnPlayer(%this)
	{
		Parent::spawnPlayer(%this);
		%this.score = $Disease_PlayerScore_[%this.BL_ID];
		
		%item = Disease_getRandomMeleeItem().getID();
		%this.player.tool[0] = %item;
		%this.player.setShapeNameDistance(32);
		mountHat(%this);
		messageClient(%this, 'MsgItemPickup', '', 0, %item);
	}
	
	function GameConnection::applyBodyColors(%this,%override)
	{
		Parent::applyBodyColors(%this);
		if(%override)
		{
			return;
		}
		mountHat(%this);
	}
	
	function FlyingVehicle::onActivate(%this, %obj)
	{
		return;
	}
	
	function FlyingVehicle::mountObject(%this, %obj, %slot)
	{
		if(%this.getName() $= "helicopter" && %obj.getClassName() $= "Player")
		{
			return Disease_enterHelicopter(%obj);
		}
		Parent::mountObject(%this, %obj, %slot);
	}
	
	function serverCmdUseTool(%this, %val)
	{
		if(!isObject(%player = %this.player))
		{
			return;
		}
		if(%player.isInfected)
		{
			centerPrint(%this, "\c0You cannot use items when you are infected.", 2);
			return;
		}
		Parent::serverCmdUseTool(%this, %val);
	}
	
	function serverCmdDropTool(%this, %val)
	{
		centerPrint(%this, "\c0You cannot drop your items.", 2);
		return;
		//Parent::serverCmdUseTool(%this, %val);
	}
	
	function minigameCanDamage(%obj, %obj2)
	{
		if(%obj.getClassName() $= "GameConnection")
		{
			%obj = %obj.player;
		}
		if(%obj2.getClassName() $= "GameConnection")
		{
			%obj2 = %obj2.player;
		}
		
		if(%obj.isInfected)
		{
			return false;
		}
		
		if(%obj.isInfected == %obj2.isInfected)
		{
			return false;
		}
		
		return Parent::minigameCanDamage(%obj, %obj2);
	}
	
	function GameConnection::onDeath(%this, %killerPlayer, %killer, %damageType, %a)
	{	
		%obj = %this.player;
		
		cancel(%obj.cough);
		if(%obj.isInfected)
		{
			commandToClient(%this, 'SetVignette', 0, 0);
		}
		
		Parent::onDeath(%this, %killerPlayer, %killer, %damageType, %a);
		if(isObject(%killer) && !%obj.isInfected)
		{
			%killer.addScore(2, "Killing infected");
		}
		
		Disease_checkForEnd();
	}
	
	function GameConnection::onClientEnterGame(%client)
	{
		Parent::onClientEnterGame(%client);
		schedule(10000, 0, serverCmdRules, %client);
	}
	
	function GameConnection::onClientLeaveGame(%this)
	{
		Parent::onClientLeaveGame(%this);
		Disease_checkForEnd();
	}
	
	function serverCmdSuicide(%this)
	{
		if($Disease::Resetting)
		{
			return;
		}
		Parent::serverCmdSuicide(%this);
	}
	
	function DiseaseTriggerData::onLeaveTrigger(%this, %trigger, %obj)
	{
		if(!isObject(%client = %obj.client) || $Disease::Resetting)
		{
			return;
		}
		schedule(1000, 0, Disease_LeaveBounds, %obj, %client);
	}
	
	function serverCmdTeamMessageSent(%this, %msg)
	{
		messageClient(%this, '', "\c0Please use the normal chat.");
		return;
	}
	
	//Thanks Greek2Me!
	function serverCmdMessageBoxCancel(%client)
	{
		Parent::serverCmdMessageBoxCancel(%client);

		AFKK_clientActive(%client);
	}

	function serverCmdMessageSent(%client,%msg)
	{
		Parent::serverCmdMessageSent(%client,%msg);

		AFKK_clientActive(%client);
	}
	
	function serverCmdStartTalking(%client)
	{
		Parent::serverCmdStartTalking(%client);

		AFKK_clientActive(%client);
	}

	function serverCmdStopTalking(%client)
	{
		Parent::serverCmdStopTalking(%client);

		AFKK_clientActive(%client);
	}
};

activatePackage(%package);