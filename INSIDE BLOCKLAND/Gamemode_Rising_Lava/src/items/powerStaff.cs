datablock AudioProfile(powerStaffHitSound)
{
	filename	 = "Add-Ons/GameMode_Rising_Lava/res/sounds/powerStaffHit.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(powerStaffSwingSound)
{
	filename	 = "Add-Ons/GameMode_Rising_Lava/res/sounds/powerStaffSwing.wav";
	description = AudioClosestLooping3d;
	preload = true;
};

datablock ParticleData(powerStaffSparkParticle)
{
	dragCoefficient		= 4;
	gravityCoefficient	= 1;
	inheritedVelFactor	= 0.2;
	constantAcceleration = 0.0;
	lifetimeMS			  = 400;
	lifetimeVarianceMS	= 300;
	textureName			 = "base/data/particles/thinRing";
	
	useInvAlpha = false;
	spinSpeed		= 150.0;
	spinRandomMin		= -150.0;
	spinRandomMax		= 150.0;

	colors[0]	 = (223/255) SPC (213/255) SPC (32/255) SPC 0.0;
	colors[1]	 = (255/255) SPC (255/255) SPC (255/255) SPC 0.5;
	colors[2]	 = (255/255) SPC (215/255) SPC (0/255) SPC 0.0;
	sizes[0]		= 0.5;
	sizes[1]		= 1.0;
	sizes[2]		= 0.5;

	times[0]	= 0.1;
	times[1] = 0.5;
	times[2] = 1.0;

	useInvAlpha = true;
};

datablock ParticleEmitterData(powerStaffSparkEmitter)
{
	lifeTimeMS = 10;

	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 5;
	velocityVariance = 3.0;
	ejectionOffset	= 1.50;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel  = 0;
	phiVariance		= 360;
	overrideAdvance = false;
	particles = powerStaffSparkParticle;

	uiName = "Power Staff Chunk";
};

datablock ParticleData(powerStaffExplosionParticle)
{
	dragCoefficient		= 10;
	gravityCoefficient	= -0.15;
	inheritedVelFactor	= 0.2;
	constantAcceleration = 0.0;
	lifetimeMS			  = 800;
	lifetimeVarianceMS	= 500;
	textureName			 = "base/data/particles/dot";

	spinSpeed		= 50.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;

	colors[0]	 = (255/255) SPC (215/255) SPC (0/255) SPC 0.0;
	colors[1]	 = (255/255) SPC (255/255) SPC (255/255) SPC 0.6;
	colors[2]	 = (223/255) SPC (213/255) SPC (32/255) SPC 0.0;
	sizes[0]		= 0.5;
	sizes[1]		= 0.1;

	useInvAlpha = true;
};

datablock ParticleEmitterData(powerStaffExplosionEmitter)
{
	lifeTimeMS = 50;

	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 10;
	velocityVariance = 1.0;
	ejectionOffset	= 0.0;
	thetaMin			= 0;
	thetaMax			= 95;
	phiReferenceVel  = 0;
	phiVariance		= 360;
	overrideAdvance = false;
	particles = powerStaffExplosionParticle;

	uiName = "Power Staff Dust";
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(powerStaffExplosion)
{
	lifeTimeMS = 400;
	
	emitter[0] = powerStaffSparkEmitter;
	particleEmitter = powerStaffExplosionEmitter;
	particleDensity = 30;
	particleRadius = 1.0;
	
	faceViewer	  = true;
	explosionScale = "1 1 1";

	soundProfile = powerStaffHitSound;

	
	shakeCamera = true;
	cameraShakeFalloff = false;
	camShakeFreq = "2.0 3.0 1.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 2.5;
	camShakeRadius = 0.0001;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.0 0.0 0.0";
	lightEndColor = "0 0 0";
};

datablock ProjectileData(powerStaffProjectile)
{
	directDamage		  = 0;
	impactImpulse		 = 1300;
	verticalImpulse	  = 1300;
	explosion			  = powerStaffExplosion;

	muzzleVelocity		= 50;
	velInheritFactor	 = 1;

	armingDelay			= 0;
	lifetime				= 100;
	fadeDelay			  = 70;
	bounceElasticity	 = 0;
	bounceFriction		= 0;
	isBallistic			= false;
	gravityMod = 0.0;

	hasLight	 = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

	uiName = "Power Staff";
};

datablock ItemData(powerStaffItem)
{
	category = "Tools";  // Mission editor category
	className = "Weapon"; // For inventory system
	
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/powerStaff.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Power Staff";
	iconName = "Add-Ons/GameMode_Rising_Lava/res/shapes/icon_powerStaff";
	doColorShift = false;
	
	image = powerStaffImage;
	canDrop = true;
};

datablock ShapeBaseImageData(powerStaffImage)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/powerStaff.dts";
	emap = true;
	
	mountPoint = 0;
	offset = "0 0 0";
	
	correctMuzzleVector = false;

	className = "WeaponImage";

	item = powerStaffItem;
	ammo = " ";
	projectile = powerStaffProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = true;

	doColorShift = false;
	
	stateName[0]							= "Activate";
	stateTimeoutValue[0]				 = 0.2;
	stateTransitionOnTimeout[0]		 = "Ready";

	stateName[1]							= "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]			= true;

	stateName[2]			= "PreFire";
	stateScript[2]						= "onPreFire";
	stateAllowImageChange[2]		  = true;
	stateTimeoutValue[2]				= 0.01;
	stateTransitionOnTimeout[2]	  = "Fire";

	stateName[3]						  = "Fire";
	stateTransitionOnTimeout[3]	  = "Fire";
	stateTimeoutValue[3]				= 0.2;
	stateFire[3]						  = true;
	stateAllowImageChange[3]		  = true;
	stateSequence[3]					 = "Fire";
	stateScript[3]						= "onFire";
	stateWaitForTimeout[3]			= true;
	stateSequence[3]				= "Fire";
	stateTransitionOnTriggerUp[3]	= "StopFire";
	stateSound[3] = powerStaffSwingSound;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";
	stateSound[4] = powerStaffSwingSound;

	stateName[5]						  = "StopFire";
	stateTransitionOnTimeout[5]	  = "Ready";
	stateTimeoutValue[5]				= 0.2;
	stateAllowImageChange[5]		  = true;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]					 = "StopFire";
	stateScript[5]					  = "onStopFire";
};

function powerStaffImage::onFire(%this, %obj, %slot)
{
	Parent::onFire(%this, %obj, %slot);
	%obj.playthread(2, rotCW);
}

function powerStaffImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}

function powerStaffImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}
