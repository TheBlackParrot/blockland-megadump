function Player::shoot(%this) {
	%eye = vectorScale(%this.getEyeVector(), 1000);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FXBrickObjectType;
	%hit = getWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this), 0);

	serverPlay3D("snap" @ getRandom(1, $QuicklyNow::SnapSoundCount), %this.getPosition());

	cancel(%this.client.afkSched);

	if(isObject(%hit) && $QuicklyNow::CanShoot) {
		cancel($QuicklyNow::EliminateAllNonHitSched);
		$QuicklyNow::EliminateAllNonHitSched = $DefaultMinigame.schedule(15000, eliminateAllNonHit);

		$EnvGuiServer::SkyColor = $EnvGuiServer::FogColor = getColorIDTable(%hit.colorID);
		Sky.skyColor = $EnvGuiServer::SkyColor;
		Sky.fogColor = $EnvGuiServer::FogColor;
		Sky.sendUpdate();

		%hit.delete();

		switch($QuicklyNow::Mode) {
			case 0:
				%this.client.dummyCamera.setTransform(%this.getHackPosition());
				%this.client.setControlObject(%this.client.dummyCamera);

				%this.client.didHit = true;

				%this.client.play2D("hit" @ getRandom(1, $QuicklyNow::HitSoundCount));

				if(BrickGroup_888888.getCount() <= 0) {
					cancel($QuicklyNow::EliminateAllNonHitSched);
					$DefaultMinigame.eliminateAllNonHit();

					$DefaultMinigame.gameStep4(); // pass off to system.cs
				}

			case 1:
				serverPlay2D("hit" @ getRandom(1, $QuicklyNow::HitSoundCount));

				if(BrickGroup_888888.getCount() <= 0) {
					cancel($QuicklyNow::EliminateAllNonHitSched);

					$DefaultMinigame.gameStep4(); // pass off to system.cs
				}
		}
	}
}

package QuicklyNowShootingPackage {
	function Player::activateStuff(%this) {
		%this.shoot();
		return parent::activateStuff(%this);
	}
};
activatePackage(QuicklyNowShootingPackage);