exec("./commands.cs");
exec("./system.cs");

// this is now a dead link as of 10/12/17
$Pref::Server::BLBrowserRadio::URL = "theblackparrot.me";
$Pref::Server::BLBrowserRadio::File = "/bl-radio/index.php";
$Pref::Server::BLBrowserRadio::AWS_URL = "http://theblackparrot.me/bl-radio";

function getRadioList() {
	if(isObject(RadioListTCP)) {
		RadioListTCP.delete();
	}

	$Server::BLBrowserRadio::Count = 0;

	if($Pref::Server::BLBrowserRadio::URL $= "") {
		warn("No URL defined for the server radio, not running.");
		return;
	}
	if($Pref::Server::BLBrowserRadio::Port $= "") {
		warn("No port defined for the server radio, defaulting to 80.");
		$Pref::Server::BLBrowserRadio::Port = 80;
	}
	if($Pref::Server::BLBrowserRadio::File $= "") {
		warn("No file defined for the server radio, defaulting to /index.php.");
		$Pref::Server::BLBrowserRadio::File = "/index.php";
	} else {
		if(getSubStr($Pref::Server::BLBrowserRadio::File, 0, 1) !$= "/") {
			$Pref::Server::BLBrowserRadio::File = "/" @ $Pref::Server::BLBrowserRadio::File;
		}
	}

	%tcp = new TCPObject(RadioListTCP) {
		going = false;
	};
	%fullpath = $Pref::Server::BLBrowserRadio::URL @ ":" @ $Pref::Server::BLBrowserRadio::Port;
	echo("\c1Connecting to\c0" SPC %fullpath @ "\c1...");
	%tcp.connect(%fullpath);
}
if($Server::BLBrowserRadio::Init $= "") {
	$Server::BLBrowserRadio::Init = true;
	getRadioList();
}

function RadioListTCP::onConnected(%this) {
	%cloudflare_cache_bypass = "?cache_bypass=" @ urlEnc(sha1(getRandom(-999999, 999999)));

	echo("\c1Connected to\c0" SPC $Pref::Server::BLBrowserRadio::URL @ ":" @ $Pref::Server::BLBrowserRadio::Port SPC "\c1for the server radio...");
	
	echo("\c1Trying to obtain file\c0" SPC $Pref::Server::BLBrowserRadio::File @ "\c1...");
	%this.send("GET" SPC $Pref::Server::BLBrowserRadio::File @ %cloudflare_cache_bypass SPC "HTTP/1.0\r\nHost:" SPC $Pref::Server::BLBrowserRadio::URL @ "\r\nUser-Agent: Torque/1.0\r\n\r\n");
}
function RadioListTCP::onConnectFailed(%this) {
	echo("\c2Failed to connect to\c0" SPC $Pref::Server::BLBrowserRadio::URL @ ":" @ $Pref::Server::BLBrowserRadio::Port SPC "\c2for the server radio.");
}
function RadioListTCP::onConnectFailed(%this) {
	echo("\c2Could not resolve DNS for\c0" SPC $Pref::Server::BLBrowserRadio::URL @ ":" @ $Pref::Server::BLBrowserRadio::Port SPC "\c2for the server radio.");
}

function RadioListTCP::onLine(%this, %line) {
	switch$(trim(%line)) {
		case "START":
			if(!%this.going) {
				%this.going = true;
			}

		case "END":
			if(%this.going) {
				%this.going = false;
			}
			talk("Finished!");
			refreshRadioQueue();

		default:
			if(%this.going) {
				%idx = $Server::BLBrowserRadio::Count;

				$Server::BLBrowserRadio::Title[%idx] = getField(%line, 0);
				$Server::BLBrowserRadio::Artist[%idx] = getField(%line, 1);
				$Server::BLBrowserRadio::Duration[%idx] = getField(%line, 2);
				$Server::BLBrowserRadio::Path[%idx] = getField(%line, 3);

				echo("Added" SPC $Server::BLBrowserRadio::Artist[%idx] SPC "-" SPC $Server::BLBrowserRadio::Title[%idx] SPC "to the radio list.");

				$Server::BLBrowserRadio::Count++;
			}
	}
}

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
		messageClient(%this, '', "\c6It appears you do not have BL-Browser or have an outdated version, <a:forum.blockland.us/index.php?topic=305792.0>get it here.</a> It is \c0NOT \c6required for this server.");
	}

	function serverCmdAWS_Version(%this, %major, %minor, %revision) {
		messageClient(%this, '', "\c6Detected AWS version \c4" @ %major @ "." @ %minor @ "." @ %revision);

		%this.AWSMajor = %major;
		%this.AWSMinor = %minor;
		%this.AWSRevision = %revision;
		%this.canUtilizeRadio = true;

		%filename = "config/server/BLBrowserRadio/saves/" @ %this.bl_id;
		if(isFile(%filename)) {
			%file = new FileObject();
			%file.openForRead(filename);

			%this.hearRadio = %file.readLine();

			%file.close();
			%file.delete();
		} else {
			%this.hearRadio = true;
		}

		if(%major >= $AWS::Major && %minor >= $AWS::Minor && %revision >= $AWS::Revision) {
			cancel(%this.versionSchedule);
		}
	}
};
activatePackage("AwsClientCheck");