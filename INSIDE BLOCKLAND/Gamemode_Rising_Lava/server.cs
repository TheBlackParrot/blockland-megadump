if($GameModeArg !$= "Add-Ons/GameMode_Rising_Lava/gamemode.txt" )
{
	error("Error: GameMode_Rising_Lava cannot be used in custom games");
	return;
}

if($Pref::Server::RisingLava::RoundLimit $= "")
{
	$Pref::Server::RisingLava::RoundLimit = 3;
}

if($Pref::Server::RisingLava::PreRound $= "")
{
	$Pref::Server::RisingLava::PreRound = 45;
}

$RL::Initialized = false;

exec("./src/sounds.cs");

exec("./src/items/pushbroom.cs");
exec("./src/items/goldenPushBroom.cs");

exec("./src/main.cs");

if(isFile("config/server/Rising Lava/score.cs"))
{
	exec("config/server/Rising Lava/score.cs");
}

RL_StartInfoClock();