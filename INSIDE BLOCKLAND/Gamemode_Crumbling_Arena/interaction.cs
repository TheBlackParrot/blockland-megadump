function fxDTSBrick::fall(%this, %player) {
	%this.playSound(CA_popSound);
	%this.setColorFX(0);

	%this.disappear(-1);
	%this.schedule(33, delete);
}

function Player::getBrickLookedAt(%this) {
	%eye = vectorScale(%this.getEyeVector(), 100);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FXBrickObjectType;
	%hit = getWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this), 0);

	if(isObject(%hit)) {
		return %hit;
	}

	return -1;
}

deactivatePackage(CrumblingArenaInteractionPackage);
package CrumblingArenaInteractionPackage {
	function GameConnection::spawnPlayer(%this) {
		%r = parent::spawnPlayer(%this);

		%spawnPos = vectorRand($CrumblingArena::SpawnBox::Start, $CrumblingArena::SpawnBox::End);
		%this.player.setTransform(spawnPos);

		return %r;
	}

	function Armor::onEnterLiquid(%this, %obj, %coverage, %type) {
		if(!$CrumblingArena::Playing) {
			%spawnPos = vectorRand($CrumblingArena::SpawnBox::Start, $CrumblingArena::SpawnBox::End);
			%obj.setVelocity("0 0 0");
			%obj.setTransform(%spawnPos);

			return parent::onEnterLiquid(%this, %obj, %coverage, %type);
		}

		%obj.kill();

		return parent::onEnterLiquid(%this, %obj, %coverage, %type);
	}

	function fxDTSBrick::onPlayerTouch(%this, %player) {
		%r = parent::onPlayerTouch(%this, %player);
		if(!$CrumblingArena::Playing) {
			return %r;
		}

		if(%this.colorFxID != 3) {
			%this.playSound(CA_stepSound);
			%this.setColorFX(3);

			if(!$CrumblingArena::ExplodingBricks) {
				%this.schedule($CrumblingArena::Bricks::FallDelay, fall, %player);
			} else {
				%mask = $TypeMasks::FxBrickObjectType;
				initContainerRadiusSearch(%this.getPosition(), 1.5, %mask);
				while(%hit = containerSearchNext()) {
					if(isObject(%hit)) {
						%hit.setColorFX(3);
						%hit.schedule($CrumblingArena::Bricks::FallDelay, fall, %player);
					}
				}
			}
		}
		return %r;
	}

	// Script_ClickPush
	function Player::activateStuff(%player) {
		%v = Parent::activateStuff(%player);
		%client = %player.client;

		if($Sim::Time - %player.lastClick < 0.3) {
			return %v;
		}
		%player.lastClick = $Sim::Time;
		
		if(!$CrumblingArena::Playing) {
			return %v;
		}
		
		%target = containerRayCast(%player.getEyePoint(),vectorAdd(vectorScale(vectorNormalize(%player.getEyeVector()),2),%player.getEyePoint()),$TypeMasks::PlayerObjectType,%player);
		
		if(!isObject(%target) || %target == %player || %player.getObjectMount() == %target) {
			return %v;
		}
		
		%target.setVelocity(vectorAdd(%target.getVelocity(),vectorScale(%player.getEyeVector(),7)));
		
		return %v;
	}

	function Armor::onTrigger(%this, %player, %slot, %val) {
		if($CrumblingArena::Spleef) {
			if(%slot == 0 && %val) {
				if($Sim::Time - %player.lastClick < 0.1) {
					return parent::onTrigger(%this, %player, %slot, %val);
				}
				%player.lastClick = $Sim::Time;

				%looking = %player.getBrickLookedAt();
				if(isObject(%looking)) {
					%looking.playSound(CA_clickSound);
					%looking.fall();
				}
			}
		}

		return parent::onTrigger(%this, %player, %slot, %val);
	}

	function serverCmdDropTool(%client) {
		messageClient(%client, '', "\c6You cannot drop your tools here.");
		%client.play2D(errorSound);
	}
};
activatePackage(CrumblingArenaInteractionPackage);