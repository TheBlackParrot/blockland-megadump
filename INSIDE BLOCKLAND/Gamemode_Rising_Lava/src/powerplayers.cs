$RL::ClientDownloadLink = "repository.zapkraft.net/download/Client_Rising_Lava";

$RLC::Server::Version = "1.2.1";

function serverCmdRLCHandshake(%this, %version)
{
	if(%this.hasGUI) return;

	if( strreplace(%version, ".", "") < strreplace($RLC::Server::Version, ".", "") )
	{
		messageClient(%this, '', "\c6The server's version number is higher than yours. This means your client is outdated!\nTo update your client, please restart Blockland or re-download the client <a:" @ $RL::ClientDownloadLink @ ">here</a>\c6.");
		commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Client", "The server's version number is higher than yours. This means your client is outdated!\n\nTo update your client, please restart Blockland or re-download the client <a:" @ $RL::ClientDownloadLink @ ">here</a>.");
	}

	%this.hasGUI = true;
	%this.guiVer = %version;
}

function serverCmdRLCRequestMoney(%this)
{
	commandToClient(%this, 'RLC_Shop_SetMoney', $RL_PlayerScore_Money_[%this.bl_id] ? $RL_PlayerScore_Money_[%this.bl_id] : 0);

	%this.resetTrailHides(isObject(%this.player));
}

function GameConnection::RLCNotify(%this, %force)
{
	if(%this.hasGUI || (%this.RLCNotified && !%force)) return;

	%this.RLCNotified = true;
	commandToClient(%this, 'MessageBoxOK', "Client_Rising_Lava", "Hey there, you don't seem to have our client!\n\nThe client allows you to use the shop with ease, and gives you access to cool new features.\n\n<a:" @ $RL::ClientDownloadLink @ ">Client_Rising_Lava.zip</a>");
}

function serverCmdShop(%this)
{
	%this.RLCNotify(true);

	if(!%this.hasGUI)
	{
		messageClient(%this, '', "\c6Although you don't have the client, you can still use the commands.");

		messageClient(%this, '', "\c6Commands: /powerup, /trail, /settrail");
	}

	commandToClient(%this, 'RLC_Shop_Open');
}

function serverCmdPowerup(%this, %request)
{
	%pus[%puc=0] = "Swiftness";
	%pus[%puc++] = "Regaining";
	%pus[%puc++] = "HighJump";
	%pus[%puc++] = "Strength";

	%pu["Swiftness"] = 850;
	%pu["Regaining"] = 700;
	%pu["HighJump"] = 620;
	%pu["Strength"] = 500;

	if(%request $= "Climbing")
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "Climbing is now default for everyone again.");
		else
			messageClient(%this, '', "\c6Climbing is now default for everyone again.");

		return;
	}

	if(%request $= "" && !%this.hasGUI)
	{
		messageClient(%this, '', "\c6Current Powerups:");

		for(%a = 0; %a <= %puc; %a++)
		{
			messageClient(%this, '', "\c4" @ %pus[%a] SPC "\c6- " @ %pu[%pus[%a]] SPC "Zapkoins");
		}
		messageClient(%this, '', "\c6You currently have \c4" @ ($RL_PlayerScore_Money_[%this.bl_id] ? $RL_PlayerScore_Money_[%this.bl_id] : 0) SPC "\c6Zapkoins.");

		messageClient(%this, '', "\c6The shop is much better with our <a:" @ $RL::ClientDownloadLink @ ">client</a>\c6.");

		return;
	}
	else if(%request $= "")
	{
		commandToClient(%this, 'RLC_Shop_Open');

		return;
	}

	if(%pu[%request] $= "")
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "Error: <font:Arial Bold Italic:14>NO_SUCH_POWERUP<font:Arial:14>\n\nThe server did not understand the powerup request.");
		else
			messageClient(%this, '', "\c6No such powerup.");

		return;
	}

	%player = %this.player;

	if(!isObject(%player))
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You don't seem to have spawned yet!");
		else
			messageClient(%this, '', "\c6You have no player.");

		return;
	}

	if(%player.hasPowerup[%request])
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You already have that powerup!");
		else
			messageClient(%this, '', "\c6Powerup already owned.");

		return;
	}

	if($RL_PlayerScore_Money_[%this.bl_id] < %pu[%request])
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You don't have enough money for that powerup!");
		else
			messageClient(%this, '', "\c6Not enough money!");

		return;
	}

	for(%a = 0; %a <= %puc; %a++)
	{
		if(%pus[%a] $= %request)
		{
			%request = %pus[%a];
		}
	}

	%this.addMoney(-%pu[%request]);
	%player.hasPowerup[%request] = true;

	commandToClient(%this, 'RLC_ShopPowerups_SetCover', 1, %request);

	%player.setPowerupDatablock();

	if(%request $= "Regaining")
	{
		%player.regainingLoop();
	}

	%player.playAudio(0, "RLitem" @ getRandom(1, 3));

	if(%this.hasGUI)
		commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You have purchased the" SPC %request SPC "powerup for <font:Arial Bold Italic:14>" @ %pu[%request] SPC "<font:Arial:14>Zapkoins.");
	else
		messageClient(%this, '', "\c6You have purchased the\c4" SPC %request SPC "\c6powerup for " @ %pu[%request] SPC "Zapkoins.");
}

function GameConnection::resetTrailHides(%this)
{
	%pus[%puc=0] = "Rainbow";
	%pus[%puc++] = "Flame";
	%pus[%puc++] = "LaserBlue";
	%pus[%puc++] = "LaserGreen";
	%pus[%puc++] = "LaserRed";
	%pus[%puc++] = "LaserGold";

	%pus[%puc++] = "None";

	//Clear all buy buttons
	commandToClient(%this, 'RLC_ShopTrails_SetCover', 0, "", 0);
	//Hide all set buttons
	commandToClient(%this, 'RLC_ShopTrails_SetCover', 1, "", 1);

	$RL_PlayerScore_HasTrail_[%this.bl_id, "LaserGold"] = %this.isVIP || %this.isAdmin;
	for(%a = 0; %a <= %puc; %a++)
	{
		if($RL_PlayerScore_HasTrail_[%this.bl_id, %pus[%a]] || %pus[%a] $= "None")
		{
			//Hide bought buttons
			commandToClient(%this, 'RLC_ShopTrails_SetCover', 1, %pus[%a], 0);
			//Unhide bought set buttons
			if(%pus[%a] !$= $RL_PlayerScore_CurrentTrail_[%this.bl_id])
				commandToClient(%this, 'RLC_ShopTrails_SetCover', 0, %pus[%a], 1);
		}
	}
}

function serverCmdTrail(%this, %request)
{
	%pus[%puc=0] = "Rainbow";
	%pus[%puc++] = "Flame";
	%pus[%puc++] = "LaserBlue";
	%pus[%puc++] = "LaserGreen";
	%pus[%puc++] = "LaserRed";
	//%pus[%puc++] = "LaserGold";

	%pu["Rainbow"] = 3000;
	%pu["Flame"] = 2600;
	%pu["LaserBlue"] = 2500;
	%pu["LaserGreen"] = 2500;
	%pu["LaserRed"] = 2500;

	if(%request $= "" && !%this.hasGUI)
	{
		messageClient(%this, '', "\c6Current Trails:");

		for(%a = 0; %a <= %puc; %a++)
		{
			messageClient(%this, '', "\c4" @ %pus[%a] SPC "\c6- " @ %pu[%pus[%a]] SPC "Zapkoins");
		}

		messageClient(%this, '', "\c6You currently have \c4" @ ($RL_PlayerScore_Money_[%this.bl_id] ? $RL_PlayerScore_Money_[%this.bl_id] : 0) @ "\c6 Zapkoins.");

		messageClient(%this, '', "\c6The shop is much better with our <a:" @ $RL::ClientDownloadLink @ ">client</a>\c6.");

		return;
	}
	else if(%request $= "")
	{
		commandToClient(%this, 'RLC_Shop_Open');

		return;
	}

	if(%pu[%request] $= "")
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "Error: <font:Arial Bold Italic:14>NO_SUCH_TRAIL<font:Arial:14>\n\nThe server did not understand the trail request.");
		else
			messageClient(%this, '', "\c6No such trail.");

		return;
	}

	echo($RL_PlayerScore_HasTrail_[%this.bl_id, %request] SPC %this.bl_id SPC %request);
	if($RL_PlayerScore_HasTrail_[%this.bl_id, %request])
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You already have that trail!");
		else
			messageClient(%this, '', "\c6Trail already owned.");

		return;
	}

	if($RL_PlayerScore_Money_[%this.bl_id] < %pu[%request])
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You don't have enough money for that trail!");
		else
			messageClient(%this, '', "\c6Not enough money!");

		return;
	}

	for(%a = 0; %a <= %puc; %a++)
	{
		if(%pus[%a] $= %request)
		{
			%request = %pus[%a];
		}
	}

	$RL_PlayerScore_HasTrail_[%this.bl_id, %request] = true;
	%this.addMoney(-%pu[%request]);

	commandToClient(%this, 'RLC_ShopTrails_SetCover', 1, %request, 0);
	commandToClient(%this, 'RLC_ShopTrails_SetCover', 0, %request, 1);

	if(%this.hasGUI)
		commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You have purchased the" SPC %request SPC "trail for <font:Arial Bold Italic:14>" @ %pu[%request] SPC "<font:Arial:14>Zapkoins.");
	else
		messageClient(%this, '', "\c6You have purchased the\c4" SPC %request SPC "\c6trail for " @ %pu[%request] SPC "Zapkoins.");
}

function serverCmdSetTrail(%this, %request)
{
	if(%request $= "" && !%this.hasGUI)
	{
		messageClient(%this, '', "\c6No trail specified.");

		return;
	}
	else if(%request $= "")
	{
		commandToClient(%this, 'RLC_Shop_Open');

		return;
	}

	if(%request $= "LaserGold")
	{
		$RL_PlayerScore_HasTrail_[%this.bl_id, %request] = %this.isVIP || %this.isAdmin;
	}

	if(!$RL_PlayerScore_HasTrail_[%this.bl_id, %request] && %request !$= "None")
	{
		if(%this.hasGUI)
			commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You either do not own that trail or it does not exist.");
		else
			messageClient(%this, '', "\c6You either do not own that trail or it does not exist.");

		return;
	}

	%player = %this.player;

	if($RL_PlayerScore_CurrentTrail_[%this.bl_id] $= %request)
	{
		return;
	}

	if($RL_PlayerScore_CurrentTrail_[%this.bl_id] !$= "")
	{
		commandToClient(%this, 'RLC_ShopTrails_SetCover', 0, $RL_PlayerScore_CurrentTrail_[%this.bl_id], 1);
	}

	$RL_PlayerScore_CurrentTrail_[%this.bl_id] = %request;

	if(isObject(%player))
	{
		cancel(%player.trailLoop);
		%player.trailLoop();
		%player.playAudio(0, "RLitem" @ getRandom(1, 3));
	}

	commandToClient(%this, 'RLC_ShopTrails_SetCover', 1, %request, 1);

	if(%this.hasGUI)
		commandToClient(%this, 'RLC_MessageBoxOK', "Rising Lava Shop", "You have set your trail to" SPC %request @ ".");
	else
		messageClient(%this, '', "\c6You have set your trail to\c4" SPC %request @ "\c6.");
}

function Player::trailLoop(%this)
{
	cancel(%this.trailLoop);

	%trail = $RL_PlayerScore_CurrentTrail_[%this.client.bl_id];

	%obj = "Trail" @ %trail @ "Image";

	if(isObject(%obj))
	{
		%this.mountImage(%obj, 3);
	}
	else
	{
		%this.unmountImage(3);
	}

	if(%trail $= "" || %trail $= "None")
	{
		return;
	}

	%this.trailLoop = %this.schedule(2000, trailLoop);
}

function Player::regainingLoop(%this)
{
	cancel(%this.regainingLoop);

	%damage = %this.getDamageLevel();

	if(%damage > 10)
	{
		%this.setDamageLevel(%damage - 10);
	}
	else
	{
		%this.setDamageLevel(0);
	}

	%this.regainingLoop = %this.schedule(1000, regainingLoop);
}

function Player::setPowerupDatablock(%this)
{
	%player[0] = "HighJump";
	%player[1] = "Swiftness";
	%player[2] = "Strength";

	for(%a = 0; %a < 3; %a++)
	{
		if(%this.hasPowerup[%player[%a]])
		{
			%string = %string @ %player[%a];
		}
	}

	%string = "PowerPlayer" @ %string;

	if(isObject(%string))
	{
		%this.setDatablock(%string);
	}
}

datablock PlayerData(PowerPlayerHighJump : PlayerNoJet)
{
	jumpForce = 1100;
};

datablock PlayerData(PowerPlayerHighJumpSwiftness : PowerPlayerHighJump)
{
	maxForwardSpeed = 9;
	maxBackwardSpeed = 6;
	maxSideSpeed = 8;
};

datablock PlayerData(PowerPlayerHighJumpSwiftnessStrength : PowerPlayerHighJump)
{
	maxForwardSpeed = 9;
	maxBackwardSpeed = 6;
	maxSideSpeed = 8;

	maxDamage = 150;
};

datablock PlayerData(PowerPlayerHighJumpStrength : PowerPlayerHighJump)
{
	maxDamage = 150;
};


datablock PlayerData(PowerPlayerSwiftness : PlayerNoJet)
{
	maxForwardSpeed = 9;
	maxBackwardSpeed = 6;
	maxSideSpeed = 8;
};

datablock PlayerData(PowerPlayerSwiftnessStrength : PowerPlayerSwiftness)
{
	maxDamage = 150;
};

datablock PlayerData(PowerPlayerStrength : PlayerNoJet)
{
	maxDamage = 150;
};

//trails

datablock ParticleData(TrailRainbowParticle)
{
	textureName          = "Add-Ons/GameMode_Rising_Lava/res/shapes/rainbow";
	dragCoefficient      = 0;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0;
	windCoefficient      = 0;
	constantAcceleration = 3;
	lifetimeMS           = 1000;
	lifetimeVarianceMS   = 0;

	spinSpeed     = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	useInvAlpha   = false;

	colors[0]	= "1 1 1 1";
	colors[1]	= "1 1 1 1";

	sizes[0]	= 0.6;
	sizes[1]	= 0.6;

	times[0]	= 1.0;
	times[1]	= 0.2;
};

datablock ParticleEmitterData(TrailRainbowEmitter)
{
	particles = TrailRainbowParticle;

	ejectionPeriodMS = 10;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	ejectionOffset   = 0.00;
	velocityVariance = 0.0;
	thetaMin         = 0;
	thetaMax         = 0;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance  = false;
};

datablock ShapeBaseImageData(TrailRainbowImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;
	mountPoint = 7;
	offset = "0 -0.3 0.8";
	eyeOffset = "0 -500 -500";
	
	stateName[0]        = "Activate";
	stateEmitter[0]     = TrailRainbowEmitter;
	stateEmitterTime[0] = 999999;
};

//flame

datablock ParticleData(TrailFlameParticle : TrailRainbowParticle)
{
	textureName = "Add-Ons/GameMode_Rising_Lava/res/shapes/flame";
};

datablock ParticleEmitterData(TrailFlameEmitter : TrailRainbowEmitter)
{
	particles = TrailFlameParticle;
};

datablock ShapeBaseImageData(TrailFlameImage : TrailRainbowImage)
{
	stateEmitter[0] = TrailFlameEmitter;
};

//laserblue

datablock ParticleData(TrailLaserBlueParticle : TrailRainbowParticle)
{
	textureName = "Add-Ons/GameMode_Rising_Lava/res/shapes/laserblue";
};

datablock ParticleEmitterData(TrailLaserBlueEmitter : TrailRainbowEmitter)
{
	particles = TrailLaserBlueParticle;
};

datablock ShapeBaseImageData(TrailLaserBlueImage : TrailRainbowImage)
{
	stateEmitter[0] = TrailLaserBlueEmitter;
};

//lasergreen

datablock ParticleData(TrailLaserGreenParticle : TrailRainbowParticle)
{
	textureName = "Add-Ons/GameMode_Rising_Lava/res/shapes/lasergreen";
};

datablock ParticleEmitterData(TrailLaserGreenEmitter : TrailRainbowEmitter)
{
	particles = TrailLaserGreenParticle;
};

datablock ShapeBaseImageData(TrailLaserGreenImage : TrailRainbowImage)
{
	stateEmitter[0] = TrailLaserGreenEmitter;
};

//laserred

datablock ParticleData(TrailLaserRedParticle : TrailRainbowParticle)
{
	textureName = "Add-Ons/GameMode_Rising_Lava/res/shapes/laserred";
};

datablock ParticleEmitterData(TrailLaserRedEmitter : TrailRainbowEmitter)
{
	particles = TrailLaserRedParticle;
};

datablock ShapeBaseImageData(TrailLaserRedImage : TrailRainbowImage)
{
	stateEmitter[0] = TrailLaserRedEmitter;
};

//lasergold

datablock ParticleData(TrailLaserGoldParticle : TrailRainbowParticle)
{
	textureName = "Add-Ons/GameMode_Rising_Lava/res/shapes/lasergold";
};

datablock ParticleEmitterData(TrailLaserGoldEmitter : TrailRainbowEmitter)
{
	particles = TrailLaserGoldParticle;
};

datablock ShapeBaseImageData(TrailLaserGoldImage : TrailRainbowImage)
{
	stateEmitter[0] = TrailLaserGoldEmitter;
};