// The Sounds
datablock AudioProfile(GrowCreeper1)
{
	filename = "./grow1.wav";
	description = AudioClosest3d;
	preload = false;
};
datablock AudioProfile(GrowCreeper2)
{
	filename = "./grow2.wav";
	description = AudioClosest3d;
	preload = false;
};
datablock AudioProfile(GrowCreeper3)
{
	filename = "./grow3.wav";
	description = AudioClosest3d;
	preload = false;
};

// The CreepKills
datablock ExplosionData(CreepKillExplosion : blinkPaintExplosion)
{
	lifeTimeMS = 150;

	particleEmitter = "";//creepKillExplosionEmitter;
	particleDensity = 12;
	particleRadius = 0.1;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 2;
	lightStartColor = "0.6 0.6 0.9";
	lightEndColor = "0 0 0";
};

// STANDARD //
datablock ProjectileData(CreepKillProjectile : stablePaintProjectile)
{
	directDamage        = 0;
	impactImpulse       = 1300;
	verticalImpulse     = 1300;
	explosion           = CreepKillExplosion;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 275;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	
	uiName = "Creep Kill";
	
	creepDamage = 1;
};

datablock ItemData(CreepKillGunItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./spraycan.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;

	uiName = "CreepKill Meta";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.5 0.4 1 1.000";

	image = CreepKillGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(CreepKillGunImage : stableSprayCanImage)
{
	shapeFile = "./spraycan.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CreepKillGunItem;
	ammo = " ";
	//projectile = CreepKillProjectile;
	//projectileType = Projectile;
	melee = false;

	doColorShift = true;
	colorShiftColor = CreepKillGunItem.colorShiftColor;
	statetimeoutvalue[2] = 0.05;
	//stateemitter[2] = "";
	//stateemittertime[2] = "0";
	statesound[0] = "";
	//statesound[2] = "sprayFireSound";
};

// WEAK //
datablock ProjectileData(CreepKillWeakProjectile : stablePaintProjectile)
{
	directDamage        = 0;
	impactImpulse       = 1300;
	verticalImpulse     = 1300;
	explosion           = CreepKillExplosion;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 275;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	
	uiName = "Creep Kill Weak";
	
	creepDamage = 0.5;
};

datablock ItemData(CreepKillWeakGunItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./spraycan.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;

	uiName = "CreepKill Standard";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.3 0.4 1 1.000";

	image = CreepKillWeakGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(CreepKillWeakGunImage : stableSprayCanImage)
{
	shapeFile = "./spraycan.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CreepKillWeakGunItem;
	ammo = " ";
	//projectile = CreepKillWeakProjectile;
	//projectileType = Projectile;
	melee = false;

	doColorShift = true;
	colorShiftColor = CreepKillWeakGunItem.colorShiftColor;
	statetimeoutvalue[2] = 0.05;
	//stateemitter[2] = "";
	//stateemittertime[2] = "0";
	statesound[0] = "";
	//statesound[2] = "sprayFireSound";
};

// STRONG //
datablock ProjectileData(CreepKillStrongProjectile : stablePaintProjectile)
{
	directDamage        = 0;
	impactImpulse       = 1300;
	verticalImpulse     = 1300;
	explosion           = CreepKillExplosion;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 275;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	
	uiName = "Creep Kill Strong";
	
	creepDamage = 2;
};

datablock ItemData(CreepKillStrongGunItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./spraycan.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;

	uiName = "CreepKill Super";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.7 0.4 1 1.000";

	image = CreepKillStrongGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(CreepKillStrongGunImage : stableSprayCanImage)
{
	shapeFile = "./spraycan.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CreepKillStrongGunItem;
	ammo = " ";
	//projectile = CreepKillStrongProjectile;
	//projectileType = Projectile;
	melee = false;

	doColorShift = true;
	colorShiftColor = CreepKillStrongGunItem.colorShiftColor;
	statetimeoutvalue[2] = 0.035;
	//stateemitter[2] = "";
	//stateemittertime[2] = "0";
	statesound[0] = "";
	//statesound[2] = "sprayFireSound";
};

// VERY STRONG //
datablock ProjectileData(CreepKillVStrongProjectile : stablePaintProjectile)
{
	directDamage        = 0;
	impactImpulse       = 1300;
	verticalImpulse     = 1300;
	explosion           = CreepKillExplosion;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 275;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	
	uiName = "Creep Kill V. Strong";
	
	creepDamage = 4;
};

datablock ItemData(CreepKillVStrongGunItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./spraycan.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;

	uiName = "CreepKill Mega";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "1 0.4 1 1.000";

	image = CreepKillVStrongGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(CreepKillVStrongGunImage : stableSprayCanImage)
{
	shapeFile = "./spraycan.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CreepKillVStrongGunItem;
	ammo = " ";
	//projectile = CreepKillVStrongProjectile;
	//projectileType = Projectile;
	melee = false;

	doColorShift = true;
	colorShiftColor = CreepKillVStrongGunItem.colorShiftColor;
	statetimeoutvalue[2] = 0.02;
	//stateemitter[2] = "";
	//stateemittertime[2] = "0";
	statesound[0] = "";
	//statesound[2] = "sprayFireSound";
};

// MOWER //
datablock ProjectileData(CreepKillMowerProjectile : stablePaintProjectile)
{
	directDamage        = 0;
	impactImpulse       = 1300;
	verticalImpulse     = 1300;
	explosion           = CreepKillExplosion;

	muzzleVelocity      = 40;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 275;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	
	uiName = "Creepkill Insta";
	
	creepDamage = 6;
};

datablock ItemData(CreepKillMowerGunItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/printGun.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;

	uiName = "CreepMow";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1.000";

	image = CreepKillMowerGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(CreepKillMowerGunImage : stableSprayCanImage)
{
	shapeFile = "base/data/shapes/printGun.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CreepKillMowerGunItem;
	ammo = " ";
	//projectile = CreepKillMowerProjectile;
	//projectileType = Projectile;
	melee = false;

	doColorShift = true;
	colorShiftColor = CreepKillMowerGunItem.colorShiftColor;
	statetimeoutvalue[2] = 0.01;
	//stateemitter[2] = "";
	//stateemittertime[2] = "0";
	statesound[0] = "";
	//statesound[2] = "sprayFireSound";
};

// SMALL BOMB //
// SORT OF copy-pasted from spear.cs

datablock AudioProfile(CreepBombExplosionSound)
{
	filename    = "./spearHit.wav";
	description = AudioClose3d;
	preload = false;
};

datablock AudioProfile(CreepBombFireSound)
{
	filename    = "./spearFire.wav";
	description = AudioClose3d;
	preload = true;
};

//CreepBomb trail
datablock ParticleData(CreepBombTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 600;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/ring";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.75 0.75 0.75 0.3";
	colors[1]	= "0.75 0.75 0.75 0.2";
	colors[2]	= "1 1 1 0.0";
	sizes[0]	= 0.15;
	sizes[1]	= 0.35;
	sizes[2]	= 0.05;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(CreepBombTrailEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = CreepBombTrailParticle;

	useEmitterColors = true;
	uiName = "CreepBomb Trail";
};


//effects
datablock ParticleData(CreepBombExplosionParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 900;
	lifetimeVarianceMS	= 300;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]	= "0.3 0.3 0.2 0.9";
	colors[1]	= "0.2 0.2 0.2 0.0";
	sizes[0]	= 4.0;
	sizes[1]	= 7.0;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(CreepBombExplosionEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	lifeTimeMS	   = 21;
	ejectionVelocity = 8;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CreepBombExplosionParticle";

	uiName = "CreepBomb Smoke";
	emitterNode = TenthEmitterNode;
};

datablock ParticleData(CreepBombExplosionParticle2)
{
	dragCoefficient		= 0.1;
	windCoefficient		= 0.0;
	gravityCoefficient	= 2.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1000;
	lifetimeVarianceMS	= 500;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/chunk";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]	= "0.0 0.0 0.0 1.0";
	colors[1]	= "0.0 0.0 0.0 0.0";
	sizes[0]	= 0.5;
	sizes[1]	= 0.5;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(CreepBombExplosionEmitter2)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	lifetimeMS       = 7;
	ejectionVelocity = 15;
	velocityVariance = 5.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CreepBombExplosionParticle2";

	useEmitterColors = true;
	uiName = "CreepBomb Chunk";
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(CreepBombExplosion)
{
	//explosionShape = "";
	lifeTimeMS = 150;

	soundProfile = CreepBombExplosionSound;

	emitter[0] = CreepBombExplosionEmitter;
	emitter[1] = CreepBombExplosionEmitter2;
	//particleDensity = 30;
	//particleRadius = 1.0;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "7.0 8.0 7.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 15.0;

	// Dynamic light
	lightStartRadius = 4;
	lightEndRadius = 3;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	//impulse
	impulseRadius = 3.5;
	impulseForce = 2000;

	//radius damage
	radiusDamage        = 40;
	damageRadius        = 3.5;
};

//projectile
datablock ProjectileData(CreepBombProjectile)
{
	projectileShapeName = "./sprayCan.dts";
	directDamage        = 0;
	impactImpulse	   = 1000;
	verticalImpulse	   = 1000;
	explosion           = CreepBombExplosion;
	particleEmitter     = CreepBombTrailEmitter;

	brickExplosionRadius = 0;
	brickExplosionImpact = false; //destroy a brick if we hit it directly?
	brickExplosionForce  = 20;
	brickExplosionMaxVolume = 200;
	brickExplosionMaxVolumeFloating = 200;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 20000;
	fadeDelay           = 19500;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = true;
	gravityMod = 0.50;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "CreepBomb";
	
	creepDamage = 30;
};


//////////
// item //
//////////
datablock ItemData(CreepBombItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./sprayCan.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "CreepBomb";
	iconName = " ";
	doColorShift = true;
	colorShiftColor = "1.000 1.000 0.000 1.000";

	 // Dynamic properties defined by the scripts
	image = CreepBombImage;
	canDrop = true;
};

//function CreepBomb::onUse(%this,%user)
//{
//	//mount the image in the right hand slot
//	%user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(CreepBombImage)
{
	// Basic Item properties
	shapeFile = "./sprayCan.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";
	//eyeOffset = "0.1 0.2 -0.55";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = CreepBombItem;
	ammo = " ";
	projectile = CreepBombProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";
	doColorShift = true;
	colorShiftColor = "1.000 1.000 0.000 1.000";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]                    = "Charge";
	stateTransitionOnTimeout[2]	= "Armed";
	stateTimeoutValue[2]            = 0.01;
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateScript[2]                  = "onCharge";
	stateAllowImageChange[2]        = false;
	
	stateName[3]			= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= 0.3;
	stateWaitForTimeout[3]		= true;
	stateScript[3]			= "onAbortCharge";
	stateAllowImageChange[3]	= false;

	stateName[4]			= "Armed";
	stateTransitionOnTriggerUp[4]	= "Fire";
	stateAllowImageChange[4]	= false;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
	stateSound[5]				= CreepBombFireSound;
};

function CreepBombImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, CreepBombReady);
}

function CreepBombImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function CreepBombImage::onFire(%this, %obj, %slot)
{
	%obj.playthread(2, CreepBombThrow);
	Parent::onFire(%this, %obj, %slot);
	 
	 Player::removeItem(%obj,CreepBombItem);
}

// Creep Bomb Large
//projectile
datablock ProjectileData(CreepBomb2Projectile)
{
	projectileShapeName = "./sprayCan.dts";
	directDamage        = 0;
	impactImpulse	   = 1000;
	verticalImpulse	   = 1000;
	explosion           = CreepBombExplosion;
	particleEmitter     = CreepBombTrailEmitter;

	brickExplosionRadius = 0;
	brickExplosionImpact = false; //destroy a brick if we hit it directly?
	brickExplosionForce  = 20;
	brickExplosionMaxVolume = 200;
	brickExplosionMaxVolumeFloating = 200;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 20000;
	fadeDelay           = 19500;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = true;
	gravityMod = 0.50;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "CreepBomb Meta";
	
	creepDamage = 50;
};


//////////
// item //
//////////
datablock ItemData(CreepBomb2Item)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./sprayCan.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "CreepBomb Meta";
	iconName = " ";
	doColorShift = true;
	colorShiftColor = "1.000 0.500 0.010 1.000";

	 // Dynamic properties defined by the scripts
	image = CreepBomb2Image;
	canDrop = true;
};

//function CreepBomb::onUse(%this,%user)
//{
//	//mount the image in the right hand slot
//	%user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(CreepBomb2Image)
{
	// Basic Item properties
	shapeFile = "./sprayCan.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";
	//eyeOffset = "0.1 0.2 -0.55";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = CreepBomb2Item;
	ammo = " ";
	projectile = CreepBomb2Projectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";
	doColorShift = true;
	colorShiftColor = "1.000 0.500 0.010 1.000";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]                    = "Charge";
	stateTransitionOnTimeout[2]	= "Armed";
	stateTimeoutValue[2]            = 0.01;
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateScript[2]                  = "onCharge";
	stateAllowImageChange[2]        = false;
	
	stateName[3]			= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= 0.3;
	stateWaitForTimeout[3]		= true;
	stateScript[3]			= "onAbortCharge";
	stateAllowImageChange[3]	= false;

	stateName[4]			= "Armed";
	stateTransitionOnTriggerUp[4]	= "Fire";
	stateAllowImageChange[4]	= false;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
	stateSound[5]				= CreepBombFireSound;
};

function CreepBomb2Image::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, CreepBombReady);
}

function CreepBomb2Image::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function CreepBomb2Image::onFire(%this, %obj, %slot)
{
	%obj.playthread(2, CreepBombThrow);
	Parent::onFire(%this, %obj, %slot);
	 
	 Player::removeItem(%obj,CreepBomb2Item);
}

// Creeper Spawn
datablock fxDTSBrickData (brick1x1CreepSpawnData : brick1x1Data){
	category = "Special";
	subCategory = "Creeper";
	uiName = "Creeper Spawn";
	iconName = "";
};

function creeperFire(%image, %player, %slot) {
	%eye = vectorScale(%player.getEyeVector(), 8);
	%pos = %player.getEyePoint();
	%mask = $TypeMasks::FxBrickObjectType;
	%hit = firstWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %player));
		
	if(!isObject(%hit)) {
		return;
	}

	%image.onHitObject(%player, %slot, %hit);
}

function CreepKillGunImage::onFire(%this, %player, %slot) { creeperFire(%this, %player, %slot); }
function CreepKillWeakGunImage::onFire(%this, %player, %slot) { creeperFire(%this, %player, %slot); }
function CreepKillStrongGunImage::onFire(%this, %player, %slot) { creeperFire(%this, %player, %slot); }
function CreepKillVStrongGunImage::onFire(%this, %player, %slot) { creeperFire(%this, %player, %slot); }
function CreepKillMowerGunImage::onFire(%this, %player, %slot) { creeperFire(%this, %player, %slot); }

function CreepKillGunImage::onHitObject(%this, %player, %slot, %hitObj) { creeperHit(%this, %player.client, %hitObj); }
function CreepKillWeakGunImage::onHitObject(%this, %player, %slot, %hitObj) { creeperHit(%this, %player.client, %hitObj); }
function CreepKillStrongGunImage::onHitObject(%this, %player, %slot, %hitObj) { creeperHit(%this, %player.client, %hitObj); }
function CreepKillVStrongGunImage::onHitObject(%this, %player, %slot, %hitObj) { creeperHit(%this, %player.client, %hitObj); }
function CreepKillMowerGunImage::onHitObject(%this, %player, %slot, %hitObj) { creeperHit(%this, %player.client, %hitObj); }