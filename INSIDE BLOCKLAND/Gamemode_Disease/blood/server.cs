datablock AudioDescription( AudioSilent3d : AudioClose3d )
{
	maxDistance = 15;
};

datablock AudioProfile(BloodSpillSound) {
	fileName = "./sounds/physics/blood_Spill.wav";
	description = AudioSilent3D;
	preload = true;
};
datablock AudioProfile(BloodDripSound1) {
	fileName = "./sounds/physics/blood_drip1.wav";
	description = AudioSilent3D;
	preload = true;
};
datablock AudioProfile(BloodDripSound2) {
	fileName = "./sounds/physics/blood_drip2.wav";
	description = AudioSilent3D;
	preload = true;
};
datablock AudioProfile(BloodDripSound3) {
	fileName = "./sounds/physics/blood_drip3.wav";
	description = AudioSilent3D;
	preload = true;
};
datablock AudioProfile(BloodDripSound4) {
	fileName = "./sounds/physics/blood_drip4.wav";
	description = AudioSilent3D;
	preload = true;
};

//exec("./rtbprefs.cs");
$SMMBlood::SprayOnDamage = 1;
$SMMBlood::DripOnDamage = 1;
$SMMBlood::SprayOnDeath = 1;
$SMMBlood::CorpseBlood = 1;
//exec("./Support_AmmoSystem.cs");
exec("./decals.cs");
exec("./blood.cs");
exec("./damage.cs");
//exec("./paint.cs");
//exec("./cleanspray.cs");

function vectorSpread(%vector, %spread) {
	%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
	%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
	%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;

	%mat = matrixCreateFromEuler(%x SPC %y SPC %z);
	return vectorNormalize(matrixMulVector(%mat, %vector));
}