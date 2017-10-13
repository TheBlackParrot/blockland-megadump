// decided to leave this in and turned on
// images are saved to an XPM2 format
$Pictionary::XPM::Symbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+=";

function _XPM_RGBToHex(%rgb) {
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

// so i'm going to assume anyone coding some sort of XPM -> PNG converter is going to end up using node
// since pretty much any package you'd use will end up using "convert" from imagemagick, i have to use XPM1 instead of XPM2
// i've left the XPM2 lines commented out

function saveDrawing() {
	%group = $Pictionary::BrickGroup;
	%old_z = -999999;
	%line = -1;

	%file = new FileObject();
	%file.openForWrite("config/server/Pictionary/art/" @ getUTC() @ ".xpm");

	//%file.writeLine("! XPM2");
	//%file.writeLine("! Drawn by" SPC $DefaultMinigame.currentDrawer.name SPC "(BL_ID" SPC $DefaultMinigame.currentDrawer.bl_id @ ")");
	//%file.writeLine("! Word was" SPC strReplace($Pictionary::CurrentWord.words, "\t", "/"));
	//%file.writeLine($Pictionary::XPM::Width SPC $Pictionary::XPM::Height SPC "64 1");

	%file.writeLine("/* XPM */");

	%file.writeLine("static char * art[] = {");
	%file.writeLine("\"" @ $Pictionary::XPM::Width SPC $Pictionary::XPM::Height SPC "64 1\",");
	for(%i=0;%i<64;%i++) {
		%char = getSubStr($Pictionary::XPM::Symbols, %i, 1);
		%col = "#" @ _XPM_RGBToHex(getColorIDTable(%i));
		%file.writeLine("\"" @ %char SPC "c" SPC %col @ "\",");
	}

	for(%x=0;%x<$Pictionary::XPM::Width;%x++) {
		for(%y=0;%y<$Pictionary::XPM::Height;%y++) {
			%brick = $Pictionary::BoardBrick[%x, %y];
			%col = %brick.colorID;

			%line[%y] = %line[%y] @ getSubStr($Pictionary::XPM::Symbols, %col, 1);
		}
	}

	%com = ",";
	for(%y=$Pictionary::XPM::Height-1;%y>=0;%y--) {
		if(%y == 0) {
			%com = "";
		}
		%file.writeLine("\"" @ %line[%y] @ "\"" @ %com);
	}
	%file.writeLine("};");

	%file.close();
	%file.delete();
}

function serverCmdSaveDrawing(%client) {
	if(!%client.isAdmin) {
		return;
	}

	if($Pictionary::DrawingSaved) {
		messageClient(%client, '', "\c6The drawing has already been saved this round.");
		return;
	}

	$Pictionary::DrawingSaved = true;

	messageAll('', "\c4" @ %client.name SPC "\c6saved the drawing to a file.");
	saveDrawing();
}

function serverCmdSave(%client) { serverCmdSaveDrawing(%client); }