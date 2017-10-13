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