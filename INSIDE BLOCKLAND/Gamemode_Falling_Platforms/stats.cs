function GameConnection::statsLoop(%this) {
	cancel(%this.statsLoop);
	%this.statsLoop = %this.schedule(1000, statsLoop);

	%line1 = "<font:Arial Bold:20>\c4Time:\c6" SPC getTimeString(mFloor($Sim::Time - $FallingPlatforms::StartedAt));
	%line2l = "\c4Round:\c6" SPC $FallingPlatforms::Rounds;
	%line2c = "\c4Personal Best:\c6" SPC %this.roundRecord;
	%line2r = "\c4Score:\c6" SPC %this.totalScore;

	%this.bottomPrint(%line1 @ "<br>" @ %line2l @ "<just:center>" @ %line2c @ "<just:right>" @ %line2r @ " ", 3, 1);
}

package FallingPlatformsStatsPackage {
	function GameConnection::autoAdminCheck(%this) {
		%this.statsLoop();
		%this.loadData();
		%this.joinSound = "join" @ getRandom(1, $FallingPlatforms::JoinSoundCount);

		messageClient(%this, '', "<font:Arial Bold:30>\c5Yhis is still a work in progress. Please be aware of that. Thanks.");
		return parent::autoAdminCheck(%this);
	}
};
activatePackage(FallingPlatformsStatsPackage);