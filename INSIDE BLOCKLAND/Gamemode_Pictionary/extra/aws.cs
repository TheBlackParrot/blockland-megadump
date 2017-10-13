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
		messageClient(%this, '', "\c2It is NOT required for this server, it only runs the music system.");
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

package AwsSupport {
	function GameConnection::onClientEnterGame(%this) {
		switch$($AwsSupport::UrlType) {
			case "Youtube":
				commandToClient(%this, 'AWS_PlayYoutube', $AwsSupport::YoutubeID, atoi($Sim::Time - $AwsSupport::YoutubeStart));
				messageClient(%this, '', "\c6Playing YouTube video \c4" @ $AwsSupport::YoutubeID);

			default:
				commandToClient(%this, 'AWS_LoadUrl', $AwsSupport::Url);
				messageClient(%this, '', "\c6Loading url \c4" @ $AwsSupport::url);
		}

		Parent::onClientEnterGame(%this);
	}
};
activatePackage("AwsSupport");

//$Pictionary::MusicURL = "http://stream.theblackparrot.me/2";

//messageAll('', "\c6Loading url \c4" @ $Pictionary::MusicURL);
//commandToAll('AWS_LoadUrl', $AwsSupport::Url = $Pictionary::MusicURL);

//$AwsSupport::UrlType = "";