datablock AudioProfile(CA_stepSound)
{
	filename = "./sounds/beep.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(CA_popSound : CA_stepSound) { filename = "./sounds/pop.wav"; };
datablock AudioProfile(CA_countdownSound : CA_stepSound) { filename = "./sounds/countdown.wav"; };
datablock AudioProfile(CA_goSound : CA_stepSound) { filename = "./sounds/go.wav"; };
datablock AudioProfile(CA_clickSound : CA_stepSound) { filename = "./sounds/click.wav"; };

datablock AudioProfile(death1)
{
	filename = "./sounds/death.wav";
	description = AudioClose3d;
	preload = true;
};

function initSounds() {
	if($CrumblingArena::InitSounds) {
		return;
	}
	$CrumblingArena::DeathSoundCount = 1;
	$CrumblingArena::InitSounds = true;

	%pattern = "config/server/CrumblingArena/sounds/death/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$CrumblingArena::DeathSoundCount++;

		eval("datablock AudioProfile(death" @ $CrumblingArena::DeathSoundCount @ " : death1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $CrumblingArena::DeathSoundCount SPC "death sounds", "sounds.cs", "initSounds");
}
initSounds();

deactivatePackage(CrumblingArenaSoundPackage);
package CrumblingArenaSoundPackage {
	function player::playDeathCry(%this) {
		serverPlay3D("death" @ getRandom(1, $CrumblingArena::DeathSoundCount), %this.getHackPosition() SPC "0 0 1 0");
		return;
	}
};
activatePackage(CrumblingArenaSoundPackage);