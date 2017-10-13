datablock ItemData(coinSingleItem)
{
	category = "Weapon";
	className = "ItemDrop";
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/Coin_Single.dts";
	emap = true;
	rotate = true;
	uiName = "";

	isMoneyDrop = true;
	createNum = 25;
	netWorth = 1;
};

datablock ItemData(coinStackItem : coinSingleItem)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/Coin_Stack.dts";
	createNum = 20;
	netWorth = 5;
};

datablock ItemData(coinPileItem : coinSingleItem)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/Coin_Pile.dts";
	createNum = 15;
	netWorth = 15;
};

datablock ItemData(dollarStackItem : coinSingleItem)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/Dollar_Stack.dts";
	createNum = 10;
	netWorth = 40;
};

datablock ItemData(dollarPileItem : coinSingleItem)
{
	shapeFile = "Add-Ons/GameMode_Rising_Lava/res/shapes/Dollar_Pile.dts";
	createNum = 5;
	netWorth = 100;
};

function ItemDrop::onAdd(%this, %obj)
{
	%obj.canPickup = true;
	%obj.rotate = true;

	Parent::onAdd(%this, %obj);
}

function RL_createDrops()
{
	RL_clearDrops();

	if(isObject(DropGroup))
	{
		DropGroup.delete();
	}

	new SimSet(DropGroup);

	%drop[0] = coinSingleItem;
	%drop[1] = coinStackItem;
	%drop[2] = coinPileItem;
	%drop[3] = dollarStackItem;
	%drop[4] = dollarPileItem;

	for(%a = 0; %a < 5; %a++)
	{
		RL_createDrop(%drop[%a]);
	}
}

function RL_createDrop(%this)
{
	if(!isObject(%this) || !%this.isMoneyDrop || !%this.netWorth)
	{
		return;
	}

	%num = %this.createNum;

	for(%a = BrickGroup_888888.getCount() - 1; %a >= 0; %a--)
	{
		%obj = BrickGroup_888888.getObject(%a);

		if(DropGroup.usedBrick[%obj])
		{
			continue;
		}

		if(!isObject(%obj) || %obj.getNumUpBricks())
		{
			continue;
		}

		%db = %obj.getDatablock();

		if(%db.brickSizeX < 2 || %db.brickSizeY < 2)
		{
			continue;
		}

		if(getRandom(%num, 100) > %num)
		{
			continue;
		}

		%pos = VectorAdd( %obj.getPosition(), "0 0" SPC (%db.brickSizeY * 0.1) + 0.6 );

		if(%pos $= "")
		{
			continue;
		}

		%item = new Item()
		{
			datablock = %this;
			canPickup = true;
			position = %pos;
		};

		ItemDrop::onAdd(%this, %item);

		DropGroup.add(%item);
		DropGroup.usedBrick[%obj] = true;
	}
}

function RL_clearDrops()
{
	if(isObject(DropGroup))
	{
		DropGroup.deleteAll();
	}
}

if(isPackage(RisingLavaDropPackage))
{
	deactivatePackage(RisingLavaDropPackage);
}

package RisingLavaDropPackage
{
	function Armor::onCollision(%this, %obj, %col, %a, %b, %c, %d, %e, %f)
	{
		if(%col.getDatablock().isMoneyDrop && %obj.getDamagePercent() < 1.0 && isObject(%obj.client))
		{
			serverPlay3D( dropPickupSound, %obj.getPosition() );
			%col.schedule(10, delete);

			%worth = %col.getDatablock().netWorth;
			%obj.client.addMoney(%worth);

			%this.onPickup(%obj);
			return;
		}

		Parent::onCollision(%this, %obj, %col, %a, %b, %c, %d, %e, %f);
	}
};

activatePackage(RisingLavaDropPackage);