function vectorRand(%v0, %v1) {
	%rx = getRandom(getWord(%v0, 0), getWord(%v1, 0));
	%ry = getRandom(getWord(%v0, 1), getWord(%v1, 1));
	%rz = getRandom(getWord(%v0, 2), getWord(%v1, 2));

	return %rx SPC %ry SPC %rz;
}

function MinigameSO::playSound(%this, %data) {
	if(!isObject(%data)) {
		return;
	}

	for(%i=0;%i<%this.numMembers;%i++) {
		%this.member[%i].play2D(%data);
	}
}

function mInterpolate(%var1, %var2, %weight) {
	return (1 - %weight) * %var1 + (%weight * %var2);
}

// Event_addItem
// (who the hell uses 3 space tabs wtf)
function Player::addItem(%player,%image,%client)
{
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
	{
		%tool = %player.tool[%i];
		if(%tool == 0)
		{
			%player.tool[%i] = %image;
			%player.weaponCount++;
			messageClient(%client,'MsgItemPickup','',%i,%image);
			break;
		}
	}
}

function MinigameSO::setItem(%this, %image, %slot) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		
		if(isObject(%client.player)) {
			%player = %client.player;

			if(!isObject(%image)) {
				%image = -1;
			} else {
				%image = %image.getID();
			}

			%player.tool[%slot] = %image;
			%player.weaponCount = %slot+1;
			messageClient(%client, 'MsgItemPickup', '', %slot, %image);

			if(%image == -1) {
				if(%player.currTool == %slot) {
					%player.updateArm(0);
					%player.unMountImage(0);
				}
			}
		}
	}
}