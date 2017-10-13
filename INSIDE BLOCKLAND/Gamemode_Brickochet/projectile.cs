//explosion
datablock AudioProfile(combo1)
{
	filename = "./sounds/combo1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(combo2:combo1) { filename = "./sounds/combo2.wav"; };
datablock AudioProfile(combo3:combo1) { filename = "./sounds/combo3.wav"; };
datablock AudioProfile(combo4:combo1) { filename = "./sounds/combo4.wav"; };
datablock AudioProfile(combo5:combo1) { filename = "./sounds/combo5.wav"; };
datablock AudioProfile(takeProjExplode:combo1) { filename = "./sounds/explode.wav"; };
datablock AudioProfile(takeProjHit:combo1) { filename = "./sounds/hit.wav"; };
datablock AudioProfile(takeShrapnelHit:combo1) { filename = "./sounds/shrapnel_hit.wav"; };
datablock AudioProfile(takeProjFire:combo1) { filename = "./sounds/fire.wav"; };
datablock AudioProfile(takeProjHitBomb:combo1) { filename = "./sounds/bomb.wav"; };
datablock AudioProfile(takeProjHitSteal:combo1) { filename = "./sounds/steal.wav"; };
datablock AudioProfile(laserhum:combo1) {
	filename = "./sounds/laser_hum.wav";
	description = AudioClosestLooping3d;
};

datablock ParticleData(takeGameProjExplosionParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.0;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 700;
	lifetimeVarianceMS   = 300;
	textureName          = "base/data/particles/dot";
	spinSpeed      = 0;
	spinRandomMin     = 0;
	spinRandomMax     = 0;
	colors[0]     = "1 1 1 0.1";
	colors[1]     = "1 1 1 0.5";
	colors[2]     = "1 1 1 1";
	sizes[0]      = 2;
	sizes[1]      = 2;
	sizes[2]      = 2;

	times[0] = 0;
	times[1] = 0.1;
	times[2] = 0.2;

	useInvAlpha = false;
};

datablock ParticleEmitterData(takeGameProjExplosionEmitter)
{
	lifeTimeMS = 500;

	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 15;
	velocityVariance = 8.0;
	ejectionOffset   = 1.25;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "takeGameProjExplosionParticle";

	uiName = "takeGameProj Explosion";
};
datablock ExplosionData(takeGameProjExplosion)
{
	//explosionShape = "";
	lifeTimeMS = 500;

	emitter[0] = takeGameProjExplosionEmitter;

	soundProfile = takeProjExplode;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "2.0 2.0 2.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	// Dynamic light
	lightStartRadius = 3;
	lightEndRadius = 1;
	lightStartColor = "00.0 0.6 0.9";
	lightEndColor = "0 0 0";
};

if(!isObject(BrickochetShrapnelSet)) {
	$Take::ShrapnelSet = new SimSet(BrickochetShrapnelSet);
}

//bullet trail effects
function createDynamicTGProjectiles() {
	$Take::initDatablocks = 1;
	for(%i=1;%i<64;%i++) {
		datablock ParticleData(TempTGDatablock1) {
			dragCoefficient      = 3;
			gravityCoefficient   = 0.0;
			inheritedVelFactor   = 0.0;
			constantAcceleration = 0.0;
			lifetimeMS           = 15000;
			lifetimeVarianceMS   = 0;
			textureName          = "base/data/particles/dot";
			spinSpeed      = 0;
			spinRandomMin     = 0;
			spinRandomMax     = 0;
			sizes[0]      = 1.3;
			sizes[1]      = 1.3;
			sizes[2]      = 1.3;
			sizes[3]      = 1.3;

			times[0] = 0;
			times[1] = 0;
			times[2] = 0;
			times[3] = 0;

			useInvAlpha = false;

			colors[0] = getWords(getColorIDTable(%i),0,2) SPC "1";
			colors[1] = getWords(getColorIDTable(%i),0,2) SPC "1";
			colors[2] = getWords(getColorIDTable(%i),0,2) SPC "1";
			colors[3] = getWords(getColorIDTable(%i),0,2) SPC "1";
		};
		TempTGDatablock1.setName("takeGameProjTrailParticle" @ %i);

		datablock ParticleEmitterData(TempTGDatablock2) {
			ejectionPeriodMS = 2;
			periodVarianceMS = 0;
			ejectionVelocity = 0.0;
			velocityVariance = 0.0;
			ejectionOffset   = 0.0;
			thetaMin         = 0;
			thetaMax         = 90;
			phiReferenceVel  = 0;
			phiVariance      = 360;
			overrideAdvance = false;
			particles = "takeGameProjTrailParticle" @ %i;

			uiName = "takeGameProj" @ %i SPC "Trail";
		};
		TempTGDatablock2.setName("takeGameProjTrailEmitter" @ %i);

		datablock ProjectileData(TempTGDatablock3) {
			projectileShapeName = "base/data/shapes/empty.dts";
			directDamage        = 0;
			directDamageType    = $DamageType::Default;
			radiusDamageType    = $DamageType::Default;
			className = "takeGameProjProjectileClass";


			impactImpulse       = 0;
			verticalImpulse     = 0;
			explosion           = "";
			bloodExplosion        = "";
			particleEmitter = "takeGameProjTrailEmitter" @ %i;
			explodeOnPlayerImpact = false;
			explodeOnDeath        = true;

			brickExplosionRadius = 0;
			brickExplosionImpact = 0;             //destroy a brick if we hit it directly?
			brickExplosionForce  = 0;             
			brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
			brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

			sound = laserhum;

			muzzleVelocity      = 200;
			velInheritFactor    = 1.0;

			armingDelay         = 30000;
			lifetime            = 30000;
			fadeDelay           = 1023;
			bounceElasticity    = 0.99;
			bounceFriction      = 0.00;
			isBallistic         = true;
			gravityMod          = 0.0;

			hasLight    = false;
			lightRadius = 3.0;
			lightColor  = "0 0 0.5";

			uiName = "takeGameProj" @ %i;
		};
		TempTGDatablock3.setName("takeGameProjProjectile" @ %i);

		datablock ProjectileData(TempTGDatablock4) {
			projectileShapeName = "base/data/shapes/empty.dts";
			directDamage        = 0;
			directDamageType    = $DamageType::Default;
			radiusDamageType    = $DamageType::Default;
			className = "takeGameProjShrapnelClass";


			impactImpulse       = 0;
			verticalImpulse     = 0;
			explosion           = "";
			bloodExplosion        = "";
			particleEmitter = "takeGameProjTrailEmitter" @ %i;
			explodeOnPlayerImpact = false;
			explodeOnDeath        = true;

			brickExplosionRadius = 0;
			brickExplosionImpact = 0;             //destroy a brick if we hit it directly?
			brickExplosionForce  = 0;             
			brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
			brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

			sound = "";

			muzzleVelocity      = 200;
			velInheritFactor    = 1.0;

			armingDelay         = 10000;
			lifetime            = 10000;
			fadeDelay           = 1023;
			bounceElasticity    = 0.99;
			bounceFriction      = 0.00;
			isBallistic         = true;
			gravityMod          = 1.0;

			hasLight    = false;
			lightRadius = 3.0;
			lightColor  = "0 0 0.5";

			uiName = "takeGameProjShrapnel" @ %i;
		};
		TempTGDatablock4.setName("takeGameProjShrapnel" @ %i);
	}
}
if(!$Take::initDatablocks) {
	createDynamicTGProjectiles();
}

package takeGameProjPackage {
	function Projectile::onAdd(%this,%obj) {
		if(%obj.getDatablock().classname $= "takeGameProjProjectileClass") {
			%obj.checkPosition();
			%obj.client.projectile = %obj;
			%obj.setName("Laser" @ %obj.getID());
		}
		if(%obj.getDatablock().classname $= "takeGameProjShrapnelClass") {
			%row = new ScriptObject(BrickochetShrapnelLink) {
				projectile = %obj;
				owner = %obj.client;
				createdAt = getSimTime();
			};
			BrickochetShrapnelSet.add(%row);
			%obj.setLink = %row;
		}
		return parent::onAdd(%this,%obj);
	}
};
activatePackage(takeGameProjPackage);

function fxDTSBrick::onLaserHit(%this,%obj) {
	if(%this.getType() & $TypeMasks::FXBrickObjectType) {
		if(!isObject(%obj.client)) {
			return;
		}
		if(!isObject(%obj.client.minigame)) {
			return;
		}
		%client = %obj.client;


		if(%this.shapeFxID) {
			//%this.exploded = 1;
			%obj.combo++;
			if(%obj.combo >= 5) {
				%sound = "combo" @ %obj.combo-4;
				if(!isObject(%sound)) {
					%sound = "combo5";
				}
				%client.playSound(%sound);
			}
			%client.play2D(takeProjHitBomb);
			%this.playSound(takeProjHitBomb);
			
			//%this.onBombExplode(%obj);
			//return;

			%amount = getRandom(14,24);
			for(%i=0;%i<%amount;%i++) {
				%this.spawnProjectile(getRandom(-85,85) SPC getRandom(-85,85) SPC getRandom(-85,85),takeGameProjShrapnel @ %client.color,"0 0 0",0.5,%client);
			}

			%this.disappear(2);
			%this.schedule(1000,delete);
			return;
		}

		if(%this.takenBy != %client) {
			%obj.combo++;
			%obj.client.score += %obj.combo;
			if(%this.takenBy !$= "") {
				%obj.client.score += 20;
			}

			if(%obj.combo >= 5) {
				%sound = "combo" @ %obj.combo-4;
				if(!isObject(%sound)) {
					%sound = "combo5";
				}
				%client.playSound(%sound);
			}

			// couldn't determine correct class name?
			switch$(%obj.scale) {
				case "1 1 1":
					%client.play2D(takeProjHit);
					%this.playSound(takeProjHit);
				case "0.5 0.5 0.5":
					%client.play2D(takeShrapnelHit);
					%this.playSound(takeShrapnelHit);
			}

			if(%this.takenBy !$= "" && %this.takenBy != -1) {
				%this.takenBy.amountHas--;
				%this.takenBy.play2D(takeProjHitSteal);
			}
			%client.amountHas++;
		}

		%this.takenBy = %client;
		%this.setColor(%client.color);
	}
}

// -- decided this was no fun. used "shrapnel" lasers instead --
//function fxDTSBrick::onBombExplode(%this,%obj) {
//	%real = getRealTime();
//	echo(%this);
//	initContainerRadiusSearch(%this.getPosition(),getRandom(20,32),$TypeMasks::FXBrickObjectType);
//	while((%targetObject = containerSearchNext()) != 0 && isObject(%targetObject)) {
//		if(%targetObject == %this || %targetObject.exploded) {
//			continue;
//		}
//		%targetObject.onLaserHit(%obj,1);
//		if(%this.shapeFxID) {
//			%this.delete();
//		}
//		if(getRealTime() - %real > 10000) {
//			talk("EMERGENCY END");
//			return;
//		}
//	}
//}

function takeGameProjProjectileClass::onCollision(%this,%obj,%col,%fade,%pos,%normal) {
	if(%col.getClassName() $= "fxDTSBrick") {
		%col.onLaserHit(%obj);
	}
}

function takeGameProjShrapnelClass::onCollision(%this,%obj,%col,%fade,%pos,%normal) {
	if(%col.getClassName() $= "fxDTSBrick") {
		%col.onLaserHit(%obj);
	}
	%obj.delete();
}

function takeGameProjProjectileClass::onExplode(%this,%obj) {
	%client = %obj.client;

	if(%obj.markedForExplosion) {
		%client.play2D(takeProjExplode);
		if(%obj.combo > %client.highestCombo) {
			%client.highestCombo = %obj.combo;
		}
		if(%obj.combo >= 20) {
			%color = "<color:" @ RGBToHex(getColorIDTable(%client.color)) @ ">";
			$DefaultMinigame.messageAll('',%color @ %client.name SPC "\c6obtained a\c3 x" @ %obj.combo SPC "combo!");
		}
		cancel(%obj.positionLoop);

		Sky.flashColor(%client,1);
	
		%client.saveTakeGame();
		return;
	}
}

function Sky::flashColor(%this,%client,%step) {
	%maxsteps = mCeil(mCeil(30/$DefaultMinigame.numMembers)/1.5);
	cancel(%this.flashSched);
	if(%step < %maxsteps) {
		%this.flashSched = %this.schedule(50,flashColor,%client,%step+1);
	}

	if(%step == %maxsteps) {
		$EnvGuiServer::SkyColor = "0 0 0 1";
		%this.skyColor = $EnvGuiServer::SkyColor;
		%this.sendUpdate();
		return;
	}

	switch(%step % 2) {
		case 1:
			$EnvGuiServer::SkyColor = getWords(getColorIDTable(%client.color),0,2) SPC "1";
			%this.skyColor = $EnvGuiServer::SkyColor;
			%this.sendUpdate();
		case 0:
			$EnvGuiServer::SkyColor = "1 1 1 1";
			%this.skyColor = $EnvGuiServer::SkyColor;
			%this.sendUpdate();
	}
}

function Projectile::checkPosition(%this) {
	cancel(%this.positionLoop);
	%this.positionLoop = %this.schedule(200,checkPosition);

	%position = %this.getPosition();
	%x = getWord(%position,0);
	%y = getWord(%position,1);
	%z = getWord(%position,2);

	if(%x < 0 || %x > $Pref::Take::PlayAreaSize || %y < 0 || %y > $Pref::Take::PlayAreaSize || %z < 0 || %z > $Pref::Take::PlayAreaHeight) {
		if(%z > $Pref::Take::PlayAreaHeight && getWord(%this.getVelocity(),2) < 0) {
			return;
		}
		%this.markedForExplosion = 1;
		%this.explode();

		// fixing issues with weird explosion placement
		// -- commenting out for now because dedicated servers are being a pain in the ass
		//
		//%explosion = new Explosion() {
		//	dataBlock = takeGameProjExplosion;
		//	position = %position;
		//};
		//
		//MissionCleanup.add(%explosion);
	}
}

function Projectile::removeFromShrapnelList(%this) {
	// i'm assuming TGE won't allow this to be perfect
	for(%i=0;%i<BrickochetShrapnelSet.getCount();%i++) {
		%row = BrickochetShrapnelSet.getObject(%i);
		if(isObject(%row.projectile)) {
			if(%row.projectile == %this) {
				%row.delete();
			}
		} else {
			%row.delete();
		}
	}
}

package BrickochetProjectilePackage {
	function Projectile::delete(%this) {
		if(%this.getDatablock().classname $= "takeGameProjShrapnelClass") {
			%this.removeFromShrapnelList();
		}
		parent::delete(%this);
	}

	function Projectile::explode(%this) {
		if(%this.getDatablock().classname !$= "takeGameProjProjectileClass") {
			parent::explode(%this);
		}
		if(%this.markedForExplosion) {
			parent::explode(%this);
			return;
		}
		if(%this.getDatablock().classname $= "takeGameProjShrapnelClass") {
			%this.removeFromShrapnelList();
		}
	}
};
activatePackage(BrickochetProjectilePackage);