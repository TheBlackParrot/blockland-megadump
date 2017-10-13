datablock AudioProfile(goldenPushBroomHitSound)
{
	filename	 = "Add-Ons/GameMode_Rising_Lava/res/sounds/goldenPushBroomHit.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(goldenPushBroomSwingSound)
{
	filename	 = "Add-Ons/GameMode_Rising_Lava/res/sounds/goldenPushBroomSwing.wav";
	description = AudioClosestLooping3d;
	preload = true;
};

datablock ParticleData(goldenPushBroomSparkParticle)
{
	dragCoefficient		= 4;
	gravityCoefficient	= 1;
	inheritedVelFactor	= 0.2;
	constantAcceleration = 0.0;
	lifetimeMS			  = 400;
	lifetimeVarianceMS	= 300;
	textureName			 = "base/data/particles/heart";
	
	useInvAlpha = false;
	spinSpeed		= 150.0;
	spinRandomMin		= -150.0;
	spinRandomMax		= 150.0;

	colors[0]	 = (255/255) SPC (0/255) SPC (0/255) SPC 0.0;
	colors[1]	 = (0/255) SPC (255/255) SPC (0/255) SPC 0.5;
	colors[2]	 = (0/255) SPC (0/255) SPC (255/255) SPC 0.0;
	sizes[0]		= 0.15;
	sizes[1]		= 0.15;
	sizes[2]		= 0.15;

	times[0]	= 0.1;
	times[1] = 0.5;
	times[2] = 1.0;

	useInvAlpha = true;
};

datablock ParticleEmitterData(goldenPushBroomSparkEmitter)
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
	particles = goldenPushBroomSparkParticle;

	uiName = "Golden Broom Chunk";
};

datablock ParticleData(goldenPushBroomExplosionParticle)
{
	dragCoefficient		= 10;
	gravityCoefficient	= -0.15;
	inheritedVelFactor	= 0.2;
	constantAcceleration = 0.0;
	lifetimeMS			  = 800;
	lifetimeVarianceMS	= 500;
	textureName			 = "base/data/particles/nut";

	spinSpeed		= 50.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;

	colors[0]	 = (255/255) SPC (215/255) SPC (0/255) SPC 0.0;
	colors[1]	  = "0.0 1.0 0.0 0.5";
	colors[2]	  = "0.0 0.0 1.0 0.0";
	sizes[0]		= 0.5;
	sizes[1]		= 1.0;

	useInvAlpha = true;
};

datablock ParticleEmitterData(goldenPushBroomExplosionEmitter)
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
	particles = goldenPushBroomExplosionParticle;

	uiName = "Golden Broom Dust";
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(goldenPushBroomExplosion)
{
	lifeTimeMS = 400;
	
	emitter[0] = goldenPushBroomSparkEmitter;
	particleEmitter = goldenPushBroomExplosionEmitter;
	particleDensity = 30;
	particleRadius = 1.0;
	
	faceViewer	  = true;
	explosionScale = "1 1 1";

	soundProfile = goldenPushBroomHitSound;

	
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

datablock ProjectileData(goldenPushBroomProjectile)
{
	directDamage		  = 0;
	impactImpulse		 = 1300;
	verticalImpulse	  = 1300;
	explosion			  = goldenPushBroomExplosion;

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

	uiName = "Golden Broom";
};

datablock ItemData(goldenPushBroomItem)
{
	category = "Tools";  // Mission editor category
	className = "Weapon"; // For inventory system
	
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/goldenPushBroom.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Golden Broom";
	iconName = "Add-Ons/GameMode_Rising_Lava/res/shapes/icon_pushBroom";
	doColorShift = true;
	colorShiftColor = (255/255) SPC (215/255) SPC (0/255) SPC (255/255);
	
	image = goldenPushBroomImage;
	canDrop = true;
};

datablock ShapeBaseImageData(goldenPushBroomImage)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/goldenPushBroom.dts";
	emap = true;
	
	mountPoint = 0;
	offset = "0 0 0";
	
	correctMuzzleVector = false;

	className = "WeaponImage";

	item = goldenPushBroomItem;
	ammo = " ";
	projectile = goldenPushBroomProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = true;

	doColorShift = true;
	colorShiftColor = goldenPushBroomItem.colorShiftColor;
	
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
	stateSound[3] = goldenPushBroomSwingSound;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";
	stateSound[4] = goldenPushBroomSwingSound;

	stateName[5]						  = "StopFire";
	stateTransitionOnTimeout[5]	  = "Ready";
	stateTimeoutValue[5]				= 0.2;
	stateAllowImageChange[5]		  = true;
	stateWaitForTimeout[5]		= true;
	stateSequence[5]					 = "StopFire";
	stateScript[5]					  = "onStopFire";
};

function goldenPushBroomImage::onFire(%this, %obj, %slot)
{
	Parent::onFire(%this, %obj, %slot);
	%obj.playthread(2, rotCW);
}

function goldenPushBroomImage::onStopFire(%this, %obj, %slot)
{	
	%obj.playthread(2, root);
}
