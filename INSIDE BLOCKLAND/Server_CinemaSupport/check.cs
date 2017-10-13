$AWS::Major = 0;
$AWS::Minor = 2;
$AWS::Revision = 1;

package AwsClientCheck {
	function Gameconnection::startLoad(%this) {
		commandToClient(%this, 'AWS_Version');
		%this.versionSchedule = %this.schedule(1000, getAws);
		
		Parent::startLoad(%this);
	}

	function Gameconnection::getAws(%this) {
		messageClient(%this, '', "\c6It appears you do not have BL-Browser or have an outdated version, <a:forum.blockland.us/index.php?topic=305792.0>get it here.</a>");
	}

	function serverCmdAWS_Version(%this, %major, %minor, %revision) {
		messageClient(%this, '', "\c6Detected AWS version \c4" @ %major @ "." @ %minor @ "." @ %revision);

		%this.AWSMajor = %major;
		%this.AWSMinor = %minor;
		%this.AWSRevision = %revision;

		if(%major >= $AWS::Major && %minor >= $AWS::Minor && %revision >= $AWS::Revision) {
			cancel(%this.versionSchedule);
		}
	}
};
activatePackage("AwsClientCheck");