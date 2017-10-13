$Disease::Version = "1.0";

echo("\c4Loading disease modules.");

%pattern = "Add-Ons/GameMode_Disease/modules/*.cs";

%file = findFirstFile(%pattern);
while(%file !$= "")
{
	if(fileExt(%file) $= ".cs")
	{
		exec(%file);
	}
	%file = findNextFile(%pattern);
}

echo("\c4All disease modules loaded.");

function serverCmdDisease_RequestVersion(%this)
{
	commandToClient(%this, 'Disease_VersionResponse', $Disease::Version);
}

if(isFile("config/server/Disease/score.cs"))
{
	exec("config/server/Disease/score.cs");
}

function Disease_ReloadModules()
{
	echo("\c4Reloading disease modules.");
	
	%pattern = "Add-Ons/GameMode_Disease/modules/*.cs";

	%file = findFirstFile(%pattern);
	while(%file !$= "")
	{
		if(strstr(%file, "datablock") < 0)
		{
			if(fileExt(%file) $= ".cs")
			{
				exec(%file);
			}
		}
		else
		{
			echo("\c4Skipping datablock file.");
		}
		%file = findNextFile(%pattern);
	}
	
	echo("\c4All disease modules reloaded.");
}

//	Manual modules
exec("Add-Ons/GameMode_Disease/footsteps/server.cs");
exec("Add-Ons/GameMode_Disease/blood/server.cs");

// 3rd party addons --TBP
exec("Add-Ons/GameMode_Disease/addons/Sound_Blockland/server.cs");
exec("Add-Ons/GameMode_Disease/addons/Emote_Critical/server.cs");
exec("Add-Ons/GameMode_Disease/addons/Vehicle_CivilianHeliPack/server.cs");
exec("Add-Ons/GameMode_Disease/addons/Weapon_Package_Tier1/server.cs");
exec("Add-Ons/GameMode_Disease/addons/Weapon_Melee_Extended/server.cs");