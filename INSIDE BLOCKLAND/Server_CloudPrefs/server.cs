exec("./lib/lesseval.cs");

function getCloudPrefs() {
	if(isObject(CloudPrefsTCP)) {
		CloudPrefsTCP.delete();
	}

	if($Pref::Server::CloudPrefs::URL $= "") {
		warn("No URL defined for cloud preferences, not running.");
		return;
	}
	if($Pref::Server::CloudPrefs::Port $= "") {
		warn("No port defined for cloud preferences, defaulting to 80.");
		$Pref::Server::CloudPrefs::Port = 80;
	}
	if($Pref::Server::CloudPrefs::File $= "") {
		warn("No file defined for cloud preferences, not running.");
		return;
	} else {
		if(getSubStr($Pref::Server::CloudPrefs::File, 0, 1) !$= "/") {
			$Pref::Server::CloudPrefs::File = "/" @ $Pref::Server::CloudPrefs::File;
		}
	}

	%tcp = new TCPObject(CloudPrefsTCP) {
		setvars = false;
	};
	%fullpath = $Pref::Server::CloudPrefs::URL @ ":" @ $Pref::Server::CloudPrefs::Port;
	echo("\c1Connecting to\c0" SPC %fullpath @ "\c1...");
	%tcp.connect(%fullpath);
}

function CloudPrefsTCP::onConnected(%this) {
	%cloudflare_cache_bypass = "?cache_bypass=" @ urlEnc(sha1(getRandom(-999999, 999999)));

	echo("\c1Connected to\c0" SPC $Pref::Server::CloudPrefs::URL @ ":" @ $Pref::Server::CloudPrefs::Port SPC "\c1for cloud prefs...");
	
	echo("\c1Trying to obtain file\c0" SPC $Pref::Server::CloudPrefs::File @ "\c1...");
	%this.send("GET" SPC $Pref::Server::CloudPrefs::File @ %cloudflare_cache_bypass SPC "HTTP/1.0\r\nHost:" SPC $Pref::Server::CloudPrefs::URL @ "\r\nUser-Agent: Torque/1.0\r\n\r\n");
}
function CloudPrefsTCP::onConnectFailed(%this) {
	echo("\c2Failed to connect to\c0" SPC $Pref::Server::CloudPrefs::URL @ ":" @ $Pref::Server::CloudPrefs::Port SPC "\c2for cloud prefs.");
}
function CloudPrefsTCP::onConnectFailed(%this) {
	echo("\c2Could not resolve DNS for\c0" SPC $Pref::Server::CloudPrefs::URL @ ":" @ $Pref::Server::CloudPrefs::Port SPC "\c2for cloud prefs.");
}

function CloudPrefsTCP::onLine(%this, %line) {
	switch$(trim(%line)) {
		case "START":
			if(!%this.setvars) {
				%this.setvars = true;
			}

		case "END":
			if(%this.setvars) {
				%this.setvars = false;
			}

		default:
			if(%this.setvars) {
				%pref = getField(%line, 0);
				%val = getField(%line, 1);

				echo("\c1Setting variable\c0" SPC %pref SPC "\c1to\c5" SPC %val);
				if($Pref::Server::CloudPrefs::Announce) {
					messageAll('', "\c4" @ %pref SPC "\c6was changed to\c2" SPC %val SPC "\c6remotely.");
				}
				cp_setGlobalByName(%pref, %val);
			}
	}
}
getCloudPrefs();