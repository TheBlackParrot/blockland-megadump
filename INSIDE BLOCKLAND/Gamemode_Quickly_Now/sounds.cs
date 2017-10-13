datablock AudioProfile(snap1)
{
	filename = "./sounds/snap/snap1.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(snap2 : snap1) { filename = "./sounds/snap/snap2.wav"; };
datablock AudioProfile(snap3 : snap1) { filename = "./sounds/snap/snap3.wav"; };
datablock AudioProfile(snap4 : snap1) { filename = "./sounds/snap/snap4.wav"; };
datablock AudioProfile(snap5 : snap1) { filename = "./sounds/snap/snap5.wav"; };
datablock AudioProfile(snap6 : snap1) { filename = "./sounds/snap/snap6.wav"; };
datablock AudioProfile(snap7 : snap1) { filename = "./sounds/snap/snap7.wav"; };
datablock AudioProfile(snap8 : snap1) { filename = "./sounds/snap/snap8.wav"; };
datablock AudioProfile(snap9 : snap1) { filename = "./sounds/snap/snap9.wav"; };

datablock AudioProfile(hit1)
{
	filename = "./sounds/hit/hit1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(hit2 : hit1) { filename = "./sounds/hit/hit2.wav"; };
datablock AudioProfile(hit3 : hit1) { filename = "./sounds/hit/hit3.wav"; };
datablock AudioProfile(hit4 : hit1) { filename = "./sounds/hit/hit4.wav"; };
datablock AudioProfile(hit5 : hit1) { filename = "./sounds/hit/hit5.wav"; };
datablock AudioProfile(hit6 : hit1) { filename = "./sounds/hit/hit6.wav"; };
datablock AudioProfile(hit7 : hit1) { filename = "./sounds/hit/hit7.wav"; };
datablock AudioProfile(hit8 : hit1) { filename = "./sounds/hit/hit8.wav"; };
datablock AudioProfile(hit9 : hit1) { filename = "./sounds/hit/hit9.wav"; };
datablock AudioProfile(hit10 : hit1) { filename = "./sounds/hit/hit10.wav"; };

datablock AudioProfile(tick)
{
	filename = "./sounds/tick.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(start)
{
	filename = "./sounds/start.wav";
	description = AudioClosest3d;
	preload = true;
};

$QuicklyNow::SnapSoundCount = 9;
$QuicklyNow::HitSoundCount = 10;

function initSounds() {
	if($QuicklyNow::InitSounds) {
		return;
	}

	$QuicklyNow::InitSounds = true;

	%pattern = "config/server/QuicklyNow/sounds/snap/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$QuicklyNow::SnapSoundCount++;

		eval("datablock AudioProfile(snap" @ $QuicklyNow::SnapSoundCount @ " : snap1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $QuicklyNow::SnapSoundCount SPC "snap sounds");

	%pattern = "config/server/QuicklyNow/sounds/hit/*.wav";
	for(%filename = findFirstFile(%pattern); isFile(%filename); %filename = findNextFile(%pattern)) {
		$QuicklyNow::HitSoundCount++;

		eval("datablock AudioProfile(hit" @ $QuicklyNow::HitSoundCount @ " : hit1) { filename = \"" @ %filename @ "\"; };");
	}
	dbg("Found" SPC $QuicklyNow::HitSoundCount SPC "hit sounds");
}
initSounds();