// Dedicated server helper script -- TheBlackParrot (18701)
// yw boink

function initBoulderDash() {
	if($BoulderDash::Init !$= "") {
		return;
	}
	$BoulderDash::Init = true;

	%pattern = "Add-Ons/Gamemode_Boulder_Dash/RequiredAddons/*.zip";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		%folder = fileBase(%filename);
		echo("Executing" SPC %folder SPC "for Boulder Dash...");
		
		exec("Add-Ons/Gamemode_Boulder_Dash/RequiredAddons/" @ %folder @ "/server.cs");
	}

	%pattern = "config/server/BoulderDash/music/*.ogg";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		%str = "musicData_" @ stripChars(fileBase(%filename), "()[]{}'?!@#$%^&*/\\\"<>,.:;|=+- ");
		eval("datablock AudioProfile(" @ %str @ ") {fileName = \"" @ %filename @ "\"; description = \"AudioMusicLooping3d\"; preload = 1; uiName = \"" @ strReplace(fileBase(%filename),"_"," ") @ "\";};");	
	}
}
initBoulderDash();

package BoulderDashPackage {
	function fxDTSBrick::onActivate(%this, %player, %client, %pos, %vec) {
		if(%this.getName() $= "_winWin") {
			messageAll('', "\c3" @ %client.name SPC "\c5reached the top in\c3" SPC getTimeString($Sim::Time - $BoulderDash::StartedAt) @ "\c5!");

			// would just parent %mg.reset instead of all this but lo and behold, this breaks events
			$BoulderDash::StartedAt = $Sim::Time;
			for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
				// fucking SIGH
				%brick = BrickGroup_888888.getObject(%i);
				if(%brick.getDatablock().getName() $= "brickVehicleSpawnData") {
					// MMMMMMMMmmmmmmmmm this too
					%brick.schedule(1000, respawnVehicle);
				}
			}
		}
		
		return parent::onActivate(%this, %player, %client, %pos, %vec);
	}

	function GameConnection::spawnPlayer(%this) {
		if($BoulderDash::StartedAt $= "") {
			$BoulderDash::StartedAt = $Sim::Time;
		}

		return parent::spawnPlayer(%this);
	}

	function onServerDestroyed() {
		deleteVariables("$BoulderDash::*");
		parent::onServerDestroyed();
	}
};
activatePackage(BoulderDashPackage);