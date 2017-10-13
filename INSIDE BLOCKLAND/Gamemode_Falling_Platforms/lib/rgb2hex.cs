function _FP_RGBToHex(%rgb) {
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

function mInterpolate(%var1, %var2, %weight) {
	return (1 - %weight) * %var1 + (%weight * %var2);
}
function interpolateColor(%col1, %col2, %weight, %mode) {
	for(%i=0;%i<getWordCount(%col1);%i++) {
		%num[1] = getWord(%col1, %i);
		%num[2] = getWord(%col2, %i);
		if(%mode) {
			%str = trim(%str SPC mInterpolate(%num[1], %num[2], %weight)/255);
		} else {
			%str = trim(%str SPC mFloatLength(mInterpolate(%num[1], %num[2], %weight), 0));
		}
	}
	return %str;
}