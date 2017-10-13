datablock AudioProfile(fall1)
{
	filename = "./sounds/fall/1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(warn1)
{
	filename = "./sounds/warn/1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(death1)
{
	filename = "./sounds/death/1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(join1)
{
	filename = "./sounds/join/1.wav";
	description = AudioClosest3d;
	preload = true;
};
$FallingPlatforms::FallSoundCount = 1;
$FallingPlatforms::WarnSoundCount = 1;
$FallingPlatforms::DeathSoundCount = 1;
$FallingPlatforms::JoinSoundCount = 1;

function initSounds() {
	if($FallingPlatforms::InitSounds) {
		return;
	}

	%pattern = "config/server/FallingPlatforms/sounds/fall/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$FallingPlatforms::FallSoundCount++;

		eval("datablock AudioProfile(fall" @ $FallingPlatforms::FallSoundCount @ " : fall1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $FallingPlatforms::FallSoundCount SPC "fall sounds");

	%pattern = "config/server/FallingPlatforms/sounds/warn/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$FallingPlatforms::WarnSoundCount++;

		eval("datablock AudioProfile(warn" @ $FallingPlatforms::WarnSoundCount @ " : fall1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $FallingPlatforms::WarnSoundCount SPC "warn sounds");

	%pattern = "config/server/FallingPlatforms/sounds/death/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$FallingPlatforms::DeathSoundCount++;

		eval("datablock AudioProfile(death" @ $FallingPlatforms::DeathSoundCount @ " : death1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $FallingPlatforms::DeathSoundCount SPC "death sounds");

	%pattern = "config/server/FallingPlatforms/sounds/join/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$FallingPlatforms::JoinSoundCount++;

		eval("datablock AudioProfile(join" @ $FallingPlatforms::JoinSoundCount @ " : join1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $FallingPlatforms::JoinSoundCount SPC "join sounds");
}
initSounds();

package FallingPlatformsSoundPackage {
	function player::playDeathCry(%this) {
		serverPlay3D(DeathSound, %this.getHackPosition());
	}
	function gameConnection::onDeath(%this) {
		serverPlay3D("death" @ getRandom(1, $FallingPlatforms::DeathSoundCount), %this.player.getHackPosition() SPC "0 0 1 0");
		return parent::onDeath(%this);
	}
};
activatePackage(FallingPlatformsSoundPackage);