package SMMBloodPackage {
	function MiniGameSO::reset(%this, %client) {
		Parent::reset(%this, %client);

		if (isObject(DecalGroup)) {
			DecalGroup.deleteAll();
		}

		if (isObject(DecalGroupNew)) {
			DecalGroupNew.deleteAll();
		}
	}
	
	function Armor::damage(%this, %obj, %src, %pos, %damage, %type) {
		if (getSimTime() - %obj.spawnTime < $Game::PlayerInvulnerabilityTime) {
			return Parent::damage(%this, %obj, %src, %pos, %damage, %type);
		}

		if (%pos $= "") {
			%pos = %obj.getHackPosition();
		}
		createBloodExplosion(%pos, %obj.getVelocity(), %obj.getScale());

		if (%obj.isCorpse) {
			if (%obj.corpseDamagelevel >= 400) {
				return;
			}
			if ($SMMBlood::CorpseBlood && !%obj.isWrapped) {
				%amt = mFloor(%damage * 0.1);
				%obj.doSplatterBlood(%amt, %pos);
			}

			%obj.corpseDamageLevel += %damage;
			return;
		}

		if (%obj.getDamageLevel() + %damage > %this.maxDamage) {
			%fatal = true;
		}

		Parent::damage(%this, %obj, %src, %pos, %damage, %type);
		if (%damage <= 0) {
			return;
		}

		if ($SMMBlood::DripOnDamage) {
			%time = %obj.getDamagePercent() * 10;
			%time = mClampF(%time, 0, 10);

			%obj.startDrippingBlood(%time);
		}

		if ($SMMBlood::SprayOnDamage && !%fatal) {
			%amt = mClampF(%damage * 0.1, 0, 15);
			%obj.doSplatterBlood(%amt, %pos);
		}
	}

	function Armor::onDisabled(%this, %obj, %disabled) {
		if ($SMMBlood::SprayOnDeath) {
			%obj.doSplatterBlood();
		}
		Parent::onDisabled(%this, %obj, %disabled);
	}
};

activatePackage("SMMBloodPackage");