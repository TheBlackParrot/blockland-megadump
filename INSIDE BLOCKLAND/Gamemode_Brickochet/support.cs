function MinigameSO::playSound(%this,%datablock) {
	if(!isObject(%datablock)) {
		warn("Unable to find sound" SPC %datablock);
		return;
	}
	for(%i=0;%i<%this.numMembers;%i++) {
		%this.member[%i].play2D(%datablock);
	}
}

function RGBToHex(%rgb) {
	%rgb = getWords(%rgb,0,2);
	for(%i=0;%i<getWordCount(%rgb);%i++) {
		%dec = mFloor(getWord(%rgb,%i)*255);
		%str = "0123456789ABCDEF";
		%hex = "";

		while(%dec != 0) {
			%hexn = %dec % 16;
			%dec = mFloor(%dec / 16);
			%hex = getSubStr(%str,%hexn,1) @ %hex;    
		}

		if(strLen(%hex) == 1)
			%hex = "0" @ %hex;
		if(!strLen(%hex))
			%hex = "00";

		%hexstr = %hexstr @ %hex;
	}

	if(%hexstr $= "") {
		%hexstr = "FF00FF";
	}
	return %hexstr;
}

function getPositionString(%num) {
	if(strLen(%num)-2 >= 0) {
		%ident = getSubStr(%num,strLen(%num)-2,2);
	} else {
		%ident = %num;
	}
	if(%ident >= 10 && %ident < 20) {
		return %num @ "th";
	}

	%ident = getSubStr(%num,strLen(%num)-1,1);
	switch(%ident) {
		case 1:
			return %num @ "st";
		case 2:
			return %num @ "nd";
		case 3:
			return %num @ "rd";
		default:
			return %num @ "th";	
	}
}