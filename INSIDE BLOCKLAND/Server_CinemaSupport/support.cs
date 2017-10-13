talk("reloaded support.cs");

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

	function serverCmdLoadUrl(%this, %url) {
		if(!%this.isSuperAdmin) {
			return;
		}

		messageAll('', "\c6Loading url \c4" @ %url);
		commandToAll('AWS_LoadUrl', $AwsSupport::Url = %url);

		$AwsSupport::UrlType = "";
	}
	
	function serverCmdYT(%this, %video, %offset) {
		loadYouTube(%video, %offset);
	}
};
activatePackage("AwsSupport");

function parseYTURL(%url) {
	%urls[0] = "youtube.com/watch?v=";
	%urls[1] = "youtu.be/";

	%final = strReplace(%url, "http://", "");
	%final = strReplace(%final, "https://", "");
	%final = strReplace(%final, "www.", "");
	for(%i=0;%i<2;%i++) {
		%final = strReplace(%final, %urls[%i], "");
	}
	%final = strReplace(%final, "/", "");
	%final = getSubStr(%final, 0, 11);

	return %final;
}

function loadYouTube(%video, %offset) {
	%final = parseYTURL(%video);

	%offset = atoi(%offset);

	messageAll('', "\c6Playing YouTube video \c4" @ %final @ "\c6 at an offset of " @ %offset);

	$AwsSupport::YoutubeID = %final;
	$AwsSupport::YoutubeStart = atoi($Sim::Time+%offset);
	$AwsSupport::UrlType = "Youtube";

	commandToAll('AWS_PlayYoutube', %final, %offset);
}

function fxDTSBrick::getBrowserPos(%this, %location) {
	%vec = vectorSub(%location, %this.getTransform());
	return ((getWord(%vec, (%this.angleID&1) ? 0:1) * ((%this.angleID>>1) ? "-1":1) )+ 4)/8*1024 SPC 768-(getWord(%vec, 2)+3)/6*768;
}

package browserSupport {
	function GameConnection::browserSupport(%this) {
		cancel(%this.bs);
		if(!%this.isSuperAdmin) {
			return;
		}

		if(isObject(%player=%this.player)) {
			%ray = containerRaycast(%player.getEyePoint(), vectorAdd(vectorScale(%player.getEyeVector(), 30), %player.getEyePoint()), $TypeMasks::FxBrickObjectType, "");
			if(isObject(%ray) && %ray.getDatablock().printAspectRatio $= "Screen") {
				%playero = %ray.getBrowserPos(posFromRaycast(%ray));
				bottomPrint(%this, "\c6" @ %playero, 3);
				commandToAll('AWS_MouseEvent', getWord(%playero, 0) | 0, getWord(%playero, 1) | 0, 4);
			}
		}
		%this.bs = %this.schedule(100, browserSupport);
	}
	function Player::activateStuff(%player) { 
		if(isObject(%client = %player.client)) {
			if(!%client.isSuperAdmin) {
				return;
			}
			%ray = containerRaycast(%player.getEyePoint(), vectorAdd(vectorScale(%player.getEyeVector(), 30), %player.getEyePoint()), $TypeMasks::FxBrickObjectType, "");
			if(isObject(%ray) && %ray.getDatablock().printAspectRatio $= "Screen") {
				%playero = %ray.getBrowserPos(posFromRaycast(%ray));
				commandToAll('AWS_MouseEvent', getWord(%playero, 0) | 0, getWord(%playero, 1) | 0, 0);

			}
		}
		return Parent::activateStuff(%player);
	}
};
//activatePackage("browserSupport");