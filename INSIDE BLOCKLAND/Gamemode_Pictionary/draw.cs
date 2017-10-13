$Pictionary::CurrentColorBricks = 6;
$Pictionary::MaxAllowedBrushRadius = 9;

function GameConnection::setActiveColor(%this, %colorID) {
	%this.activeColor = %colorID;

	for(%i = 0; %i < $Pictionary::CurrentColorBricks; %i++) {
		%b = ("_currentColor" @ %i).getID();
		%b.setColor(%colorID);
	}

	dbg("Set active color to" SPC %colorID, "draw.cs");
}

function GameConnection::setBrushRadius(%this, %amnt) {
	if(%amnt < 1) { %amnt = $Pictionary::MaxAllowedBrushRadius; }
	else if(%amnt > $Pictionary::MaxAllowedBrushRadius) { %amnt = 1; }

	%this.radius = %amnt;

	_brushSize.setPrintCount(%amnt);
	dbg("Set brush radius to" SPC %amnt, "draw.cs");
}

function Player::drawPixel(%this) {
	%client = %this.client;
	if(!%client.canDraw) {
		return;
	}

	%eye = vectorScale(%this.getEyeVector(), 100);
	%pos = %this.getEyePoint();
	%mask = $TypeMasks::FXBrickObjectType;
	%hit = getWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %this), 0);

	if(isObject(%hit)) {
		if(%hit.getGroup().getID() == $Pictionary::BrickGroup.getID()) {
			%color = (%client.activeColor $= "" ? 58 : %client.activeColor);

			if(%client.radius > 1) {
				%pos = %hit.getPosition();
				%radius = (%client.radius $= "") ? 1 : %client.radius-1;
				
				%shape = "circle";
				if(%client.shape $= "circle" || %client.shape $= "square") {
					%shape = %client.shape;
				}

				if(%shape $= "square") {
					initContainerBoxSearch(%pos, %radius SPC %radius SPC %radius, %mask);
				} else {
					initContainerRadiusSearch(%pos, %radius/2, %mask);
				}

				while(%object = containerSearchNext()) {
					if(%object.getGroup().getID() == $Pictionary::BrickGroup.getID()) {
						%object.setColor(%color);
					}
				}
			} else {
				%hit.setColor(%color);
			}
		}
	}
}

package PictionaryDrawingPackage {
	function Armor::onTrigger(%this, %player, %slot, %val) {
		%client = %player.client;
		if(%client.canDraw) {
			if(%slot == 0) {
				if(%val) {
					%eye = vectorScale(%player.getEyeVector(), 100);
					%pos = %player.getEyePoint();
					%mask = $TypeMasks::FXBrickObjectType;
					%hit = getWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %player), 0);

					switch$(%hit.getName()) {
						case "_color":
							%client.setActiveColor(%hit.colorID);

						case "_brushSizeUp":
							%client.setBrushRadius(%client.radius + 1);

						case "_brushSizeDown":
							%client.setBrushRadius(%client.radius - 1);

						default:
							dbg("Player should \c2start \c6drawing...", "draw.cs", 3);
							%player.drawLoop();
					}
				} else {
					dbg("Player should \c0stop \c6drawing...", "draw.cs", 3);
					cancel(%player.drawSched);
				}
			}
		} else {
			if(%val && %slot == 0) {
				dbg("Client" SPC %client SPC "not allowed to draw.", "draw.cs", 3);
			}
		}
		return parent::onTrigger(%this, %player, %slot, %val);
	}
	function Player::drawLoop(%this) {
		cancel(%this.drawSched);
		%this.drawSched = %this.schedule(1, drawLoop);

		%this.drawPixel();
	}
};
activatePackage(PictionaryDrawingPackage);

// COMMANDS
function serverCmdFill(%client, %colorID) {
	if(!%client.canDraw) {
		return;
	}

	if(%colorID $= "") {
		%colorID = %client.activeColor;
	}

	fillBoard(%colorID);
}

function serverCmdClear(%client) {
	if(!%client.canDraw) {
		return;
	}

	fillBoard(63);
}

// shortcuts
function serverCmdF(%client) { serverCmdFill(%client); }
function serverCmdC(%client) { serverCmdClear(%client); }