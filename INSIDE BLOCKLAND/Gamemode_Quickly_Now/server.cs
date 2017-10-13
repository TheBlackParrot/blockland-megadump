exec("./sounds.cs");
exec("./cubes.cs");
exec("./shooting.cs");
exec("./system.cs");
exec("./commands.cs");

if($QuicklyNow::Init $= "") {
	$QuicklyNow::Init = true;

	schedule(100, 0, regenCubes, getRandom(64, 128));
	$DefaultMinigame.schedule(100, preGameLoop);
}

if(isFile("Add-Ons/Sky_TGE_Bluesky.zip")) {
	$EnvGuiServer::SkyFile = "Add-Ons/Sky_TGE_Bluesky/sky_bluesky.dml";
}