datablock PlayerData(PlayerPatientZero : PlayerNoJet)
{
	maxForwardSpeed = 10;
	maxBackwardSpeed = 10;
	maxSideSpeed = 10;
	
	maxForwardCrouchSpeed = 5;
	maxBackwardCrouchSpeed = 5;
	maxSideCrouchSpeed = 5;
	
	maxDamage = 250;
	
	uiName = "Patient Zero Player";
};

datablock triggerData(DiseaseTriggerData)
{
	tickPeriodMS = 100;
};

datablock ParticleData(diseaseSpores)
{
	dragCoefficient = 10;
	gravityCoefficient = -0.15;
	inheritedVelFactor = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 700;
	lifetimeVarianceMS = 300;
	textureName = "base/data/particles/cloud";
	
	spinSpeed = 50.0;
	spinRandomMin = -500.0;
	spinRandomMax = 500.0;
	
	colors[0] = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[1] = 92/255 SPC 17/255 SPC 56/255 SPC 0.3;
	colors[2] = 35/255 SPC 122/255 SPC 18/255 SPC 0.4;
	sizes[0] = 1.0;
	sizes[1] = 0.5;
	sizes[2] = 0.6;
	
	useInvAlpha = true;
};

datablock ParticleEmitterData(diseaseSporeEmitter)
{
	lifeTimeMS = 50;
	
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 10;
	velocityVariance = 1.0;
	ejectionOffset = 0.0;
	thetaMin = 0;
	thetaMax = 95;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	particles = diseaseSpores;
	
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(diseaseSporeExplosion)
{
	lifeTimeMS = 400;
	
	emitter[0] = diseaseSporeEmitter;
	particleEmitter = diseaseSporeEmitter;
	particleDensity = 30;
	particleRadius = 1.0;
	
	faceViewer = true;
	explosionScale = "1 1 1";
	
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.0 0.0 0.0";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(diseaseSpore)
{
	directDamage = 0;
	impactImpulse = 1300;
	verticalImpulse = 1300;
	explosion = diseaseSporeExplosion;
	
	muzzleVelocity = 50;
	velInheritFactor = 1;
	
	armingDelay = 0;
	lifetime = 100;
	fadeDelay = 70;
	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = false;
	gravityMod = 0.0;
	
	hasLight = false;
	lightRadius = 3.0;
	lightColor = "0 0 0.5";
};

%pattern = "Add-Ons/GameMode_Disease/sound/*.wav";

%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_disease/sound/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "/", "_");
	
	echo("Created disease sound Disease_Sound_" @ %soundName);
	eval("datablock AudioProfile(Disease_Sound_" @ %soundName @ ") { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");
	
	%file = findNextFile(%pattern);
}

//HATS

datablock ShapeBaseImageData(Disease_Hat_Bandage)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Bandage.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 10 10";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Builder)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Builder.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0.02 0.3";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Cigarette)
{
	cost = 50;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Cigarette.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 10 10";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Swag)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Dark.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Headphones)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Headphones.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "-0.03 0 0.24";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Jason)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Jason.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "-1 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Cowboy)
{
	cost = 60;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Western.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.8";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_PaperBag)
{
	cost = 65;
	shapeFile = "Add-Ons/GameMode_Disease/hats/PaperBag.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Beret)
{
	cost = 65;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Beret.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Nails)
{
	cost = 55;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Nails.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Shades)
{
	cost = 70;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Shades.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Pirate)
{
	cost = 70;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Pirate.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Bandit)
{
	cost = 65;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Bandit.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 -0.4 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};

datablock ShapeBaseImageData(Disease_Hat_Doctor)
{
	cost = 65;
	shapeFile = "Add-Ons/GameMode_Disease/hats/Doctor.dts";
	emap = true;
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0.4";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	colorShiftColor = "0.392 0.196 0.0 1.000";
};