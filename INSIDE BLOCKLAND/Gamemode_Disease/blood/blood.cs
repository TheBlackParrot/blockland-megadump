$SMMBlood::DripTimeOnBlood = 5;
$SMMBlood::CheckpointCount = 30;

function BloodDripProjectile::onCollision(%this, %obj, %col, %pos, %fade, %normal) {
	if (%col.getType() & $TypeMasks::FxBrickObjectType && !%obj.isPaint) {
		// spawnDecal(bloodDecal @ getRandom(1, 2), vectorAdd(%pos, vectorScale(%normal, 0.01)), %normal);
	}

	if (%col.getType() & $TypeMasks::PlayerObjectType && !%obj.isPaint) {
		%col.startDrippingBlood($SMMBlood::DripTimeOnBlood);
	}
	%obj.explode();
}

function bloodDripProjectile::onExplode(%this, %obj, %pos)
{
	ServerPlay3D(bloodDripSound @ getRandom(1, 4), %pos);
}

function createBloodDripProjectile(%position, %size, %paint) {
	%obj = new Projectile() {
		dataBlock = BloodDripProjectile;

		initialPosition = %position;
		initialVelocity = "0 0 -2";
		isPaint = %paint;
	};

	MissionCleanup.add(%obj);

	if (%size !$= "") {
		%obj.setScale(%size SPC %size SPC %size);
	}

	return %obj;
}

function Player::startDrippingBlood(%this, %duration) {
	%duration = mClampF(%duration, 0, 60);
	%remaining = %this.dripBloodEndTime - $Sim::Time;

	if (%duration == 0 || (%this.dripBloodEndTime !$= "" && %duration < %remaining)) {
		return;
	}

	%this.dripBloodEndTime = $Sim::Time + %duration;

	if (!isEventPending(%this.dripBloodSchedule)) {
		%this.dripBloodSchedule = %this.schedule(getRandom(300, 800), dripBloodSchedule);
	}
}

function Player::stopDrippingBlood(%this) {
	%this.dripBloodEndTime = "";
	cancel(%this.dripBloodSchedule);
}

function Player::dripBloodSchedule(%this) {
	cancel(%this.dripBloodSchedule);

	if ($Sim::Time >= %this.dripBloodEndTime) {
		return;
	}

	%this.doDripBlood(true);
	%this.dripBloodSchedule = %this.schedule(getRandom(300, 800), dripBloodSchedule);
}

function Player::doDripBlood(%this, %force, %start, %end) {
	if (!%force && $Sim::Time - %this.lastBloodDrip <= 0.2) {
		return false;
	}
	if (%start $= "") {
		%start = vectorAdd(%this.position, "0 0 0.1");
	}
	if (%end $= "") {
		%end = vectorSub(%this.position, "0 0 0.1");
	}
		
	%ray = containerRayCast(%start, %end, $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType);

	%this.lastBloodDrip = $Sim::Time;
	%decal = spawnDecalFromRayCast(BloodDecal @ getRandom(1, 2), %ray, 0.4 + getRandom() * 0.85);
	if (isObject(%decal)) {
		%decal.isBlood = true;
	}

	return true;
	
	%this.lastBloodDrip = $Sim::Time;
	
	%x = getRandom() * 6 - 3;
	%y = getRandom() * 6 - 3;
	%z = 0 - (20 + getRandom() * 40);

	return true;
}

function Player::doSplatterBlood(%this, %amount, %pos) {
	if (%pos $= "") {
		%pos = %this.getHackPosition();
	}

	if (%amount $= "") {
		%amount = getRandom(15, 30);
	}

	%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
	%spread = 0.25 + getRandom() * 0.25;

	for (%i = 0; %i < %amount; %i++) {
		%cross = vectorScale(vectorSpread("0 0 -1", %spread), 6);
		%stop = vectorAdd(%pos, %cross);

		%ray = containerRayCast(%pos, %stop, %masks);
		%scale = 0.4 + getRandom() * 0.85;
		%decal = spawnDecalFromRaycast(BloodDecal @ getRandom(1, 2), %ray, %scale);
		if(getWord(%ray, 1))
		{
			%decal.isBlood = true;
			serverPlay3d(bloodSpillSound, getWords(%ray, 1, 3));
			createBloodExplosion(getWords(%ray, 1, 3), vectorNormalize(%this.getVelocity()), %scale SPC %scale SPC %scale);
			if(vectorDot("0 0 -1", %decal.normal) >= 0.5 && !isEventPending(%decal.ceilingBloodSchedule)) {
				if(getRandom(0, 3) == 3) 
				{
					%decal.ceilingBloodSchedule = schedule(getRandom(16, 500), 0, ceilingBloodLoop, %decal, getWords(%ray, 1, 3));
				}
			}
		}
	}
}

function ceilingBloodLoop(%this, %pos, %paint) {
	cancel(%this.ceilingBloodSchedule);
	if(!isObject(%this))
	{
		return;
	}

	if(!%this.driptime) {
		%this.driptime = 500;
	}

	%this.driptime = %this.driptime + 10;
	if(%this.driptime > 3000) {
		return;
	}

	if(%pos $= "") {
		return;
	}

	createBloodDripProjectile(%pos, "", %paint);
	%this.ceilingBloodSchedule = schedule(%this.driptime, 0, ceilingBloodLoop, %this, %pos, %paint);
}

function createBloodExplosion(%position, %velocity, %scale) {
	%datablock = bloodExplosionProjectile @ getRandom(1, 2);
	%obj = new Projectile() {
		dataBlock = %datablock;

		initialPosition = %position;
		initialVelocity = %velocity;
	};

	MissionCleanup.add(%obj);

	%obj.setScale(%scale);
	%obj.explode();
}

datablock staticShapeData(BloodDecal1) {
	shapeFile = "./shapes/decals/blood1.dts";

	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};

datablock staticShapeData(BloodDecal2) {
	shapeFile = "./shapes/decals/blood2.dts";

	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};

datablock ParticleData(bloodParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.2;
	gravityCoefficient	= 0.2;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 40.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "./shapes/decals/blood2.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 0.4;
	sizes[1]	= 2;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile1)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion;
	particleEmitter     = bloodEmitter;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};



datablock ParticleData(bloodParticle2)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.1;
	gravityCoefficient	= 0.3;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 300;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 20.0;
	spinRandomMin		= -10.0;
	spinRandomMax		= 10.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "./shapes/decals/blood3.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 1;
	sizes[1]	= 0;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter2)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle2;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion2)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter2;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile2)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion2;
	particleEmitter     = bloodEmitter2;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};

datablock ParticleData(bloodDripParticle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.1;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 200;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 20.0;
	spinRandomMin		= -10.0;
	spinRandomMax		= 10.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0.8";
	sizes[0]	= 0.1;
	sizes[1]	= 0;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodDripEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodDripParticle;

	useEmitterColors = true;
	uiName = "";
};

datablock ProjectileData(bloodDripProjectile)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion2;
	particleEmitter     = bloodDripEmitter;

	muzzleVelocity      = 60;
	velInheritFactor    = 1;

	armingDelay         = 3000;
	lifetime            = 3000;
	fadeDelay           = 2000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};