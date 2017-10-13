exec("./json.cs");

function getGradients() {
	if(jettisonReadFile("Add-Ons/Gamemode_Crumbling_Arena/colors.json")) {
		error("Parse error at" SPC $JSON::Index @ ":" SPC $JSON::Error);
		return;
	}

	$CrumblingArena::Gradients = $JSON::Value.colors;
}
if(!isObject($CrumblingArena::Gradients)) {
	getGradients();
}

function setGradient(%index) {
	%gradients = $CrumblingArena::Gradients;

	if(%index >= %gradients.length || %index < 0) {
		return;
	}

	for(%i=0;%i<%gradients.value[%index].length;%i++) {
		%row = %gradients.value[%index];
		setColorTable(%i, %row.value[%i] SPC "1");
	}

	%mg = $DefaultMinigame;
	for(%i=0;%i<%mg.numMembers;%i++) {
		%mg.member[%i].transmitStaticBrickData();
		commandToClient(%mg.member[%i], 'PlayGui_LoadPaint');
	}
}

function getGradientPart(%want, %total, %inverse) {
	if(!%inverse) {
		return mClamp(mFloor(%want * (16/(%total-1))), 0, 15);
	} else {
		return mAbs(mClamp(mFloor(%want * (16/(%total-1))), 0, 15) - 15);
	}
}