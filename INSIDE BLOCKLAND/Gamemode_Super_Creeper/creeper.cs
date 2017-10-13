//creeper mod by Mr. Wallet (BL_ID 688, JJWallet@gmail.com, steam login JJWallet@aol.com)
//props to rot, truce, redliub, zor, and all the helpful testers and suggesters, without whom this mod would not be the complete game that it is.

//if(isFile("Add-Ons/System_ReturnToBlockland/server.cs")) {
//	RTB_registerPref("Growth Speed","Creep Mod","Creep::tickms","list Normal 20 Slow 50 Slower 100 Slowest 200","GameMode_Super_Creeper",50,0,0,"CreepPrefOnChangeCreepSpeed");
//	RTB_registerPref("Player-Hunting","Creep Mod","Creep::wanderlust","list Laser-Guided 0 Merciless 1 Dogged 3 Pesky 9 Normal 19 Relaxed 49 Subtle 99 Off -1","GameMode_Super_Creeper",19,0,0,"CreepPrefOnChangeCreepHunting");
//	RTB_registerPref("WaterFX Creeper","Creep Mod","Creep::undulo","bool","GameMode_Super_Creeper",1,0,0,0);
//	RTB_registerPref("Creep Color","Creep Mod","Creep::Color","int 0 63","GameMode_Super_Creeper",8,0,0,0);
//	RTB_registerPref("Color Mode","Creep Mod","Creep::ColorMode","list ShowDamage 0 Static 1","GameMode_Super_Creeper",0,0,0);
//	RTB_registerPref("Damage Color Direction","Creep Mod","Creep::DmgDir","list Down 0 Up 1","GameMode_Super_Creeper",0,0,0,0);
//	RTB_registerPref("Damage Color Distance","Creep Mod","Creep::ColorDist","int 1 63","GameMode_Super_Creeper",9,0,0,0);
//	RTB_registerPref("Creep HP","Creep Mod","Creep::HPSetting","list Juggernautical 1 Unyielding 2 Hardy 3 Normal 4 Weak 5 Fragile 6 Insta-Gib 7","GameMode_Super_Creeper",4,0,0,"CreepPrefOnChangeHPFactor");
  //  RTB_registerPref("Lagless Mode (Buggy)","Creep Mod","Creep::lagless","bool","GameMode_Super_Creeper",0,0,0,0);
    //RTB_registerPref("Multiply HP by # of Players","Creep Mod","Creep::scalehp","bool","GameMode_Super_Creeper",1,0,0,0);
    //RTB_registerPref("Maximum HP Factor","Creep Mod","Creep::maxHPFac","int 0 100","GameMode_Super_Creeper",1,0,0,0);
	//RTB_registerPref("Creep Regenerates","Creep Mod","Creep::regen","bool","GameMode_Super_Creeper",1,0,0,"CreepPrefOnChangeRegenerates");
	//RTB_registerPref("Regeneration Speed","Creep Mod","Creep::regenms","list Insane 150 Faster 500 Fast 1000 Normal 2000 Slow 3500 Slower 5000","GameMode_Super_Creeper",5000,0,0,"CreepPrefOnChangeRegenSpeed");
//} else {
	$Creep::tickms = 70;
	$Creep::wanderlust = 19;
	$Creep::undulo = 1;
	$Creep::Color = 8;
	$Creep::ColorMode = 0;
	$Creep::DmgDir = 0;
	$Creep::ColorDist = 9;
	$Creep::HPFactor = 1;
	$Creep::scalehp = 1;
	$Creep::regen = 1;
	$Creep::regenms = 5000;
	$Creep::StartTime = 10;
	$Creep::GameMode = 0;
	$Creep::TimeLimit = 8;
	$Creep::UseShop = 1;
	$Creep::RoundLimit = 3;
//}

switch($Creep::HPSetting) {
	case 1:
		$Creep::HPFactor = 100;
	case 2:
		$Creep::HPFactor = 2;
	case 3:
		$Creep::HPFactor = 1.5;
	case 4:
		$Creep::HPFactor = 1;
	case 5:
		$Creep::HPFactor = 0.67;
	case 6:
		$Creep::HPFactor = 0.33;
	case 7:
		$Creep::HPFactor = 0.01;
	default:
		$Creep::HPFactor = 1;
}

registerOutputEvent("MiniGame", "CreepKillAll", "", 1);
registerOutputEvent("fxDTSBrick", "CreepKillRadius", "float 0.5 100 0.5 10", 1);


function MiniGameSO::CreepKillAll(%obj, %client)
{
	if(!isobject(CreepGroup) || !$Creep::active) return;
	if(%client.minigame != $DefaultMiniGame) return;
    
	CreepGroup.chainDeleteAll();
}

function fxDTSBrick::CreepKillRadius(%obj, %r, %client)
{
	if(!isobject(CreepGroup) || !$Creep::active) return;
	if(%client.minigame != $DefaultMiniGame) return;
	initContainerRadiusSearch(%obj.position, %r, $TypeMasks::FxBrickAlwaysObjectType);
	while(%check = containerSearchNext()) {
		if(CreepGroup.ismember(%check)) {
			%check.delete();
		}
	}
}

$GrowMasks = $TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::StaticShapeObjectType;
// Oh, so some of these are unnecisary, are they? Why don't you try turning some off?

// creep spawn plant code
function brick1x1CreepSpawnData::onPlant(%this,%brick)
{
	if(!isobject(CreepGroup)){
		new ScriptGroup(CreepGroup){
			class = Creeper;
		};
		mainBrickGroup.add(CreepGroup);
	}
    
	scheduleNoQuota(50,0,spawnadddelay,%brick);
}

function brick1x1CreepSpawnData::onLoadPlant(%this,%brick)
{
	if(!isobject(CreepGroup)){
		new ScriptGroup(CreepGroup){
			class = Creeper;
		};
		mainBrickGroup.add(CreepGroup);
	}
    
	scheduleNoQuota(50,0,spawnadddelay,%brick);
}

function spawnadddelay(%brick)
{
	if(%brick.isplanted == 0 || !isobject(%brick)){
		return;
	}
    
	CreepGroup.add(%brick);
}


function brick1x1CreepSpawnData::onremove(%this,%brick)
{
	if(%brick.isplanted){
		CreepGroup.remove(%brick);
	
	   if(isObject(%obj.light))
	      %obj.light.delete();
	   if(isObject(%obj.emitter))
	      %obj.emitter.delete();
	   if(isObject(%obj.item))
	      %obj.item.delete();
	   if(isObject(%obj.AudioEmitter))
	      %obj.AudioEmitter.delete();
	   if(isObject(%obj.vehicle))
	      %obj.vehicle.delete();
	   if(isObject(%obj.vehicleSpawnMarker))
	    %obj.vehicleSpawnMarker.delete();

		if(isObject($CurrBrickKiller))
		{
            if(isObject($CurrBrickKiller.miniGame))
            {
                $CurrBrickKiller.incScore($CurrBrickKiller.miniGame.Points_BreakBrick);
            }
		}
	}
}


// brick planting
function ForceNewBrick(%datablock, %pos, %a, %color, %damage, %skipforce) {
	switch(%a) {
		case 0:
			%rot = "1 0 0 0";
		case 1:
			%rot = "0 0 1 90";
		case 2:
			%rot = "0 0 1 180";
		case 3:
			%rot = "0 0 1 -90";
	}
	%brick = new fxDTSBrick() {
			angleID = %a;
			colorFxID = "0";
			colorID = %color;
			dataBlock = %datablock;
			isPlanted = "1";
			position = %pos;
			printID = "0";
			rotation = %rot;
			scale = "1 1 1";
			shapeFxID = $Creep::undulo ? 2 : 0;
			damage = %damage;
	};
	%error = %brick.plant();
    
    if(%error == 1) { // blocked only - ignore everything else
        %brick.delete();
        return 0;
    }
    
	%brick.isBaseplate = "1";
	%brick.setTrusted(1);
	%brick.eventdelay0 = 0;
	%brick.eventenabled0 = 1;
	%brick.eventinput0 = "onPlayerTouch";
	%brick.eventinputidx0 = 1;
	%brick.eventoutput0 = "Kill";
	%brick.appendclient0 = 1;
	%brick.eventoutputidx0 = 1;
	%brick.eventtarget0 = "Player";
	%brick.eventtargetidx0 = 1;
	%brick.implicitcancelevents = 0;
	%brick.numevents = 1;
	%brick.justforced = true;
    
    // play a random sound from our list
    serverplay3d("GrowCreeper" @ getRandom(1, 3), %brick.getPosition() SPC "0 20 1 0");
    
	if(!%skipforce) scheduleNoQuota(500,0,"unforcebrick",%brick);
	return %brick;
}

function unforceBrick(%brick)
{
	if(!isobject(%brick)) return;
	%brick.justforced = false;
    
    // do a small radius search to make sure nobody's trying to cheat (doesn't happen in lagless mode)
    if(!$creep::lagless) {
        InitContainerRadiusSearch(%brick.getPosition(), 1, $TypeMasks::PlayerObjectType);
        while((%targetObject=containerSearchNext()) !$= 0) {
            if(isObject(%targetObject.client)) {
                // radiussearches won't go buglessly below a radius of 1, so we need to do a distance check on player objects we find
                %distance = vectorDist(%brick.getPosition(),%targetObject.getPosition());
                
                if(%distance < 0.7) {
                    %targetObject.kill();
                }
            }
        }
    }
}


function AttemptNewBrick(%datablock, %pos, %a, %color, %damage, %id) {
	switch(%a) {
		case 0:
			%rot = "1 0 0 0";
		case 1:
			%rot = "0 0 1 90";
		case 2:
			%rot = "0 0 1 180";
		case 3:
			%rot = "0 0 1 -90";
	}
	%brick = new fxDTSBrick() {
			angleID = %a;
			colorFxID = "0";
			colorID = %color;
			dataBlock = %datablock;
			position = %pos;
			printID = "0";
			rotation = %rot;
			isplanted = "1";
			scale = "1 1 1";
			shapeFxID = $Creep::undulo ? 2 : 0;
			damage = %damage;
			stackbl_id = %id;
	};
	%brick.client = findclientbybl_id(%id);
	%error = %brick.plant();
    
    if(%error == 1) { // blocked only - ignore everything else
        %brick.delete();
        return 0;
    }
    
	scheduleNoQuota(50, 0, "FinishBrickPlantStuff", %brick, %id);
	return(%brick);
}

function FinishBrickPlantStuff(%brick)
{
	if(!isobject(%brick)) return;
	CreepGroup.add(%brick);
	%brick.setTrusted(1);
	return;
	%brick.eventdelay0 = 0;
	%brick.eventenabled0 = 1;
	%brick.eventinput0 = "onPlayerTouch";
	%brick.eventinputidx0 = 1;
	%brick.eventoutput0 = "Kill";
	%brick.appendclient0 = 1;
	%brick.eventoutputidx0 = 1;
	%brick.eventtarget0 = "Player";
	%brick.eventtargetidx0 = 1;
	%brick.implicitcancelevents = 0;
	%brick.numevents = 1;
    
    serverplay3d("GrowCreeper" @ getRandom(1, 3), %brick.getPosition() SPC "0 20 1 0");
    
    // do a small radius search to make sure nobody's trying to cheat (doesn't happen in lagless mode)

    // this crap doesn't work, removed
}

function GrowTick(%counter)
{
	cancel($CreepGrowTick);
	if(!isobject($DefaultMiniGame)) {
		messageall("", "WARNING: The minigame was aborted unexpectedly. Why would you do that? (Creeper growth has been halted to avoid errors)");
		StopCreeper();
		return;
	}
    
    if(CreepGroup.getCount() == 0) {
        $DefaultMiniGame.endRound();
    }
    
    if(!$Creep::Active) { // little hotfix
        //messageall("", "ERROR: Unexpected creeper game abortion. (Creeper growth has been halted to avoid errors)");
        StopCreeper();
        return;
    }
    
	CreepGroup.grow();
	%counter++;
	if(%counter >= 10) {
		%counter = 0;
	}
    
    if($Creep::lagless && $Creep::tickms > 50) { // tick time below 50ms is not allowed in lagless mode
        %Creep::tickms = 50;
    }
    
    // optimization has been removed due to bugs. it didn't really ever work properly anyway.
	//CreepGroup.optimize(%counter);
    if($Creep::RoundType == 2) { // grow fast during insta-gib rounds
        $CreepGrowTick = scheduleNoQuota(10, 0, "GrowTick", %counter);
    } else {
        $CreepGrowTick = scheduleNoQuota($Creep::tickms, 0, "GrowTick", %counter);
    }
}

function Creeper::IdealCreeping(%this, %pos)
{
	%check = ContainerRayCast(%pos, getword(%pos, 0) SPC getword(%pos, 1) SPC (getword(%pos, 2)+0.2), $GrowMasks);
	if(isobject(%check)) return false;
	%check = ContainerRayCast(%pos, getword(%pos, 0) SPC getword(%pos, 1) SPC (getword(%pos, 2)-0.2), $GrowMasks);
	if(isobject(%check)) return false;
	for(%x = 0; %x <= 2; %x++) {
		for(%y = 0; %y <= 2; %y++) {
			for(%z = 0; %z <= 2; %z++) {
				if(((%x == 1 && (%y == 1 || %z == 1)) || (%z == 1 && %y == 1)) && (%x != 1 || %y != 1 || %z != 1)) {
					%check = ContainerRayCast(%pos, vectoradd(%pos, ((%x-1)*0.7) SPC ((%y-1)*0.7) SPC ((%z-1)*0.8)), $GrowMasks);
					if(isobject(%check) && !%this.ismember(%check)) {
						if(%check.getclassname() $= "fxDTSBrick") {
							if(%check.isrendering()) return true;
						} else return true;
					}
				}
			}
		}
	}
	return false;
}

function Creeper::GoodCreeping(%this, %pos)
{
	%check = ContainerRayCast(%pos, getword(%pos, 0) SPC getword(%pos, 1) SPC (getword(%pos, 2)+0.2), $GrowMasks);
	if(isobject(%check)) return false;
	%check = ContainerRayCast(%pos, getword(%pos, 0) SPC getword(%pos, 1) SPC (getword(%pos, 2)-0.2), $GrowMasks);
	if(isobject(%check)) return false;
	for(%x = 0; %x <= 2; %x++) {
		for(%y = 0; %y <= 2; %y++) {
			for(%z = 0; %z <= 2; %z++) {
				if(%z == 1) {
					%vec = ((%x-1)*0.7) SPC ((%y-1)*0.7) SPC "0";
					%check = ContainerRayCast(%pos, vectoradd(%pos, %vec), $GrowMasks);
				} else {
					if(%x != 1 || %y != 1) {
						%vec = ((%x-1)*0.5) SPC ((%y-1)*0.5) SPC "0";
						%glance = ContainerRayCast(%pos, vectoradd(%pos, %vec), $GrowMasks);
					}
					if(!isobject(%glance) || (%x == 1 && %y == 1)) {
						%vec = "0 0" SPC ((%z-1)*0.8);
						%tpos = (getword(%pos, 0)+((%x-1)*0.5)) SPC (getword(%pos, 1)+((%y-1)*0.5)) SPC getword(%pos, 2);
						%check = ContainerRayCast(%tpos, vectoradd(%tpos, %vec), $GrowMasks);
					}
				}
				if(isobject(%check) && !%this.ismember(%check)) {
					if(%check.getclassname() $= "fxDTSBrick") {
						if(%check.isrendering()) return true;
					} else return true;
				}
			}
		}
	}
	return false;
}

function SetCreepDmgColor(%brick)
{
	if($Creep::ColorMode) return;
	if(!%brick.damage) {
		%brick.setcolor($Creep::Color);
		return;
	}
	%damlimit = ((15 - (CreepGroup.getcount()/1000)));
	if(%damlimit < 0) %damlimit = 0;
	// %mg.playercount doesn't exist, it's numPlayers
	%damlimit = (%damlimit + 5) * ($Creep::scalehp ? $DefaultMiniGame.numPlayers : 1) * $Creep::HPFactor;
	%damlimit += mfloor(%damlimit * ($DefaultMiniGame.wave - 1) * 0.2);
	%damstep = %damlimit / $Creep::ColorDist + 1;
	if($Creep::DmgDir) {
		%color = $Creep::Color + mceil(%brick.damage/%damstep);
		if(%color > 63) %color -= 64;
	} else {
		%color = mfloor((%damlimit - %brick.damage)/%damstep) + $Creep::Color - $Creep::ColorDist;
		if(%color < 0) %color += 64;
	}
	%brick.setcolor(%color);
}

function BrickRegen(%brick)
{
	if(!isobject(%brick)) return;
	%brick.damage--;
	if(%brick.damage < 0) %brick.damage = 0;
	SetCreepDmgColor(%brick);
	if(%brick.damage > 0) %brick.regentick = scheduleNoQuota($Creep::regenms, 0, "BrickRegen", %brick);
}

function Creeper::Grow(%this)
{
	%this.searchrad++;
	if(($Creep::wanderlust >= 0 && !getrandom(0,$Creep::wanderlust)) || $Creep::RoundType == 1) {
		for(%i = 0; %i < $DefaultMiniGame.nummembers; %i++) {
			%creepfound = false;
			%pl = $DefaultMiniGame.member[%i].player;
			if(isobject(%pl)) {
				initContainerRadiusSearch(%pl.getposition(), 9 + %this.searchrad, $TypeMasks::FxBrickAlwaysObjectType);
				while(!%creepfound && %check = containerSearchNext()) {
					if(%this.ismember(%check) && !%check.isdead() && !%check.cantgrow) {
						%creepfound = true;
						%checkindex++;
						%checklist[%checkindex] = %check;
					}
				}
			}
		}
	}
	if(%checkindex) {
		for(%i = 1; %i <= %checkindex && !isobject(%brick); %i++) {
			%brick = %this.trytogrowbrick(%checklist[%i]);
			//if(%brick == 3) echo("FUCK ME");
		}
		if(isobject(%brick) && !%brick.isdead()) {
			for(%j = 1; %j <= 5 && !%sorted; %j++) {
				if(!isobject(%this.priority[%j])) {
					%this.priority[%j] = %brick;
					%sorted = true;
				}
			}
			if(!%sorted) {
				for(%j = 5; %j > 1 ; %j--) {
					%this.priority[%j] = %this.priority[%j - 1];
				}
				%this.priority[1] = %brick;
			}
			%planted = true;
		}
		%this.searchrad -= 2;
		if(%this.searchrad < 0) %this.searchrad = 0;
		if(%planted) return;
	}
	%this.genericgrowth();
}

function Creeper::GenericGrowth(%this)
{
	for(%i = 1; %i <= 10; %i++) {
		if(!isObject(%this.priority[%i])) %brick = %this.getobject(getrandom(0,%this.getcount()-1));
		else %brick = %this.priority[%i];
		if(%brick.damage <= 0 && !%brick.isdead()) {
		//if(%color $= "") %color = 9;
			%pos = %brick.getposition();
			%x = getword(%pos, 0);
			%y = getword(%pos, 1);
			%z = getword(%pos, 2);
			%db = %brick.getdatablock();
			%bsx = (%brick.angleid % 2 == 0 ? %db.bricksizex : %db.bricksizey);
			%bsy = (%brick.angleid % 2 == 0 ? %db.bricksizey : %db.bricksizex);
			if(!%brick.cantgrow) {
				%brick.cantgrow = true;
				for(%j = 1; %j <= %bsx; %j++) {
					for(%k = 1; %k <= %bsy; %k++) {
						for(%l = 1; %l <= %db.bricksizez/3; %l++) {
							%tmpx = %x - ((%bsx - 1) * 0.25) + ((%j - 1) * 0.5);
							%tmpy = %y - ((%bsy - 1) * 0.25) + ((%k - 1) * 0.5);
							%tmpz = %z - ((%db.bricksizez/3 - 1) * 0.3) + ((%l - 1) * 0.6);
							%tmppos = %tmpx SPC %tmpy SPC %tmpz;
							if((%j == 1 && (%k == 1 || %l == 1)) || (%k == 1 && %l == 1)) {
								if(%l == 1) {
									%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 -0.85"), $GrowMasks);
									if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0 -0.6"))) {
										%brick.cantgrow = false;
										%index++;
										%poslist[%index] = vectoradd(%tmppos, "0 0 -0.6");
									}
								}
								if(%brick.cantgrow) {
									if(%j == 1) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "-0.65 0 0"), $GrowMasks);
										if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "-0.5 0 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "-0.5 0 0");
											%brick.cantgrow = false;
										}
									}
									if(%j == %bsx) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0.65 0 0"), $GrowMasks);
										if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0.5 0 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0.5 0 0");
											%brick.cantgrow = false;
										}
									}
									if(%k == 1) {						
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 -0.65 0"), $GrowMasks);
										if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 -0.5 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0 -0.5 0");
											%brick.cantgrow = false;
										}
									}
									if(%k == %bsy) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0.65 0"), $GrowMasks);
										if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0.5 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0 0.5 0");
											%brick.cantgrow = false;
										}
									}
									if(%l == %db.bricksizez/3) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 0.85"), $GrowMasks);
										if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0 0.6"))) {
											if(getrandom(0,1)) {
												%index++;
												%poslist[%index] = vectoradd(%tmppos, "0 0 0.6");
											}
											%brick.cantgrow = false;
										}
									}
								}
							}
							if(%brick.cantgrow) {
								if(%l == 1) {
									%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 -0.85"), $GrowMasks);
									if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0 -0.6"))) {
										%brick.cantgrow = false;
										%index++;
										%poslist[%index] = vectoradd(%tmppos, "0 0 -0.6");
									}
								}
								if(%brick.cantgrow) {
									if(%j == 1) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "-0.65 0 0"), $GrowMasks);
										if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "-0.5 0 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "-0.5 0 0");
											%brick.cantgrow = false;
										}
									}
									if(%j == %bsx) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0.65 0 0"), $GrowMasks);
										if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0.5 0 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0.5 0 0");
											%brick.cantgrow = false;
										}
									}
									if(%k == 1) {						
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 -0.65 0"), $GrowMasks);
										if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 -0.5 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0 -0.5 0");
											%brick.cantgrow = false;
										}
									}
									if(%k == %bsy) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0.65 0"), $GrowMasks);
										if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0.5 0"))) {
											%index++;
											%poslist[%index] = vectoradd(%tmppos, "0 0.5 0");
											%brick.cantgrow = false;
										}
									}
									if(%l == %db.bricksizez/3) {
										%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 0.85"), $GrowMasks);
										if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0 0.6"))) {
											if(getrandom(0,1)) {
												%index++;
												%poslist[%index] = vectoradd(%tmppos, "0 0 0.6");
											}
											%brick.cantgrow = false;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	if(!%index) return;
	%datablock = "brick1x1Data";
	%rnd = getrandom(1, %index);
	%brick = forceNewBrick(%datablock, %poslist[%rnd], 0, $Creep::Color); if(%brick == 0) { return;}
	%this.add(%brick);
	if(isobject(%brick)) {
		%sorted = false;
		for(%i = 1; %i <= 5 && !%sorted; %i++) {
			if(!isobject(%this.priority[%i])) {
				%this.priority[%i] = %brick;
				%sorted = true;
			}
		}
		if(!%sorted) {
			for(%i = 5; %i > 1 ; %i--) {
				%this.priority[%i] = %this.priority[%i - 1];
			}
			%this.priority[1] = %brick;
		}
	}
	return %brick;
}

function Creeper::TryToGrowBrick(%this, %brick)
{
	if(%brick.damage > 0 || !isobject(%brick)) return;
	//if(%color $= "") %color = 9;
	%pos = %brick.getposition();
	%x = getword(%pos, 0);
	%y = getword(%pos, 1);
	%z = getword(%pos, 2);
	%db = %brick.getdatablock();
	%bsx = (%brick.angleid % 2 == 0 ? %db.bricksizex : %db.bricksizey);
	%bsy = (%brick.angleid % 2 == 0 ? %db.bricksizey : %db.bricksizex);
	if(!%brick.cantgrow) {
		%brick.cantgrow = true;
		for(%j = 1; %j <= %bsx; %j++) {
			for(%k = 1; %k <= %bsy; %k++) {
				for(%l = 1; %l <= %db.bricksizez/3; %l++) {
					%tmpx = %x - ((%bsx - 1) * 0.25) + ((%j - 1) * 0.5);
					%tmpy = %y - ((%bsy - 1) * 0.25) + ((%k - 1) * 0.5);
					%tmpz = %z - ((%db.bricksizez/3 - 1) * 0.3) + ((%l - 1) * 0.6);
					%tmppos = %tmpx SPC %tmpy SPC %tmpz;
					if((%j == 1 && (%k == 1 || %l == 1)) || (%k == 1 && %l == 1)) {
						if(%l == 1) {
							%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 -0.85"), $GrowMasks);
							if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0 -0.6"))) {
								%brick.cantgrow = false;
								%index++;
								%poslist[%index] = vectoradd(%tmppos, "0 0 -0.6");
							}
						}
						if(%brick.cantgrow) {
							if(%j == 1) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "-0.65 0 0"), $GrowMasks);
								if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "-0.5 0 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "-0.5 0 0");
									%brick.cantgrow = false;
								}
							}
							if(%j == %bsx) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0.65 0 0"), $GrowMasks);
								if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0.5 0 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0.5 0 0");
									%brick.cantgrow = false;
								}
							}
							if(%k == 1) {						
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 -0.65 0"), $GrowMasks);
								if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 -0.5 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0 -0.5 0");
									%brick.cantgrow = false;
								}
							}
							if(%k == %bsy) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0.65 0"), $GrowMasks);
								if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0.5 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0 0.5 0");
									%brick.cantgrow = false;
								}
							}
							if(%l == %db.bricksizez/3) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 0.85"), $GrowMasks);
								if(!%outcheck && %this.idealcreeping(vectoradd(%tmppos, "0 0 0.6"))) {
									if(getrandom(0,1)) {
										%index++;
										%poslist[%index] = vectoradd(%tmppos, "0 0 0.6");
									}
									%brick.cantgrow = false;
								}
							}
						}
					}
					if(%brick.cantgrow) {
						if(%l == 1) {
							%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 -0.85"), $GrowMasks);
							if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0 -0.6"))) {
								%brick.cantgrow = false;
								%index++;
								%poslist[%index] = vectoradd(%tmppos, "0 0 -0.6");
							}
						}
						if(%brick.cantgrow) {
							if(%j == 1) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "-0.65 0 0"), $GrowMasks);
								if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "-0.5 0 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "-0.5 0 0");
									%brick.cantgrow = false;
								}
							}
							if(%j == %bsx) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0.65 0 0"), $GrowMasks);
								if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0.5 0 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0.5 0 0");
									%brick.cantgrow = false;
								}
							}
							if(%k == 1) {						
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 -0.65 0"), $GrowMasks);
								if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 -0.5 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0 -0.5 0");
									%brick.cantgrow = false;
								}
							}
							if(%k == %bsy) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0.65 0"), $GrowMasks);
								if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0.5 0"))) {
									%index++;
									%poslist[%index] = vectoradd(%tmppos, "0 0.5 0");
									%brick.cantgrow = false;
								}
							}
							if(%l == %db.bricksizez/3) {
								%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 0.85"), $GrowMasks);
								if(!%outcheck && %this.goodcreeping(vectoradd(%tmppos, "0 0 0.6"))) {
									if(getrandom(0,1)) {
										%index++;
										%poslist[%index] = vectoradd(%tmppos, "0 0 0.6");
									}
									%brick.cantgrow = false;
								}
							}
						}
					}
				}
			}
		}
	}
	if(!%index) return -1; //return; alone was somehow always returning a value of dummyPlayer, causing massive console spam.
	%datablock = "brick1x1Data";
	%rnd = getrandom(1, %index);
	%brick = forceNewBrick(%datablock, %poslist[%rnd], 0, $Creep::Color); if(%brick == 0) { return;}
	%this.add(%brick);
	return %brick;
}

function reviveNeighborCells(%brick)
{
	%pos = %brick.getposition();
	%x = getword(%pos, 0);
	%y = getword(%pos, 1);
	%z = getword(%pos, 2);
	%db = %brick.getdatablock();
	%bsx = (%brick.angleid % 2 == 0 ? %db.bricksizex : %db.bricksizey);
	%bsy = (%brick.angleid % 2 == 0 ? %db.bricksizey : %db.bricksizex);
	for(%j = 1; %j <= %bsx; %j++) {
		for(%k = 1; %k <= %bsy; %k++) {
			for(%l = 1; %l <= %db.bricksizez/3; %l++) {
				%tmpx = %x - ((%bsx - 1) * 0.25) + ((%j - 1) * 0.5);
				%tmpy = %y - ((%bsy - 1) * 0.25) + ((%k - 1) * 0.5);
				%tmpz = %z - ((%db.bricksizez/3 - 1) * 0.3) + ((%l - 1) * 0.6);
				%tmppos = %tmpx SPC %tmpy SPC %tmpz;						
				if(%l == 1) {
					%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 -0.85"), $GrowMasks);
					if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
				}
				if(%brick.cantgrow) {
					if(%j == 1) {
						%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "-0.65 0 0"), $GrowMasks);
						if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
					}
					if(%j == %bsx) {
						%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0.65 0 0"), $GrowMasks);
						if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
					}
					if(%k == 1) {						
						%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 -0.65 0"), $GrowMasks);
						if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
					}
					if(%k == %bsy) {
						%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0.65 0"), $GrowMasks);
						if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
					}
					if(%l == %db.bricksizez/3) {
						%outcheck = ContainerRayCast(%tmppos, vectoradd(%tmppos, "0 0 0.85"), $GrowMasks);
						if(isobject(%outcheck) && CreepGroup.ismember(%outcheck)) %outcheck.cantgrow = false;
					}
				}
			}
		}
	}
}

function Creeper::startGrowth(%this, %client)
{
	$Creep::SavedSpawns[index] = 0;
	%this.searchrad = 0;
    
    // init creeper spawns
    for(%i = 0; %i < CreepGroup.getcount(); %i++) {
		%spawn = %this.getobject(%i);
		if(%spawn.getdatablock().getname() $= "brick1x1CreepSpawnData") {
			$Creep::SavedSpawns[index]++;
			%pos = %spawn.getposition();
			$Creep::SavedSpawns[$Creep::SavedSpawns[index]] = %pos;
			$Creep::SavedSpawns[$Creep::SavedSpawns[index], color] = %spawn.colorid;
			$Creep::SavedSpawns[$Creep::SavedSpawns[index], blid] = %spawn.stackbl_id;
			%brick = forceNewBrick("brick1x1Data", %pos, 0, $Creep::Color); if(%brick == 0) { continue; }
			%this.add(%brick);
			%spawn.delete();
		}
	}
    
	if($Creep::tickms < 20) $Creep::tickms = 20;
	return scheduleNoQuota($Creep::tickms, 0, "GrowTick");
}

function Creeper::ClearSpawns(%this) {
    $Creep::SavedSpawns[index] = 0; // they'll never know!
    
    // delete the creepgroup's contents (the spawns)
    if(isObject(CreepGroup)) {
        CreepGroup.chaindeleteall();
    }
}

function CreepKillProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHit(%a, %b, %c, %d, %e, %f, %g);
}

function CreepKillWeakProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHit(%a, %b, %c, %d, %e, %f, %g);
}

function CreepKillStrongProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHit(%a, %b, %c, %d, %e, %f, %g);
}

function CreepKillVStrongProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHit(%a, %b, %c, %d, %e, %f, %g);
}

function CreepKillMowerProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHit(%a, %b, %c, %d, %e, %f, %g);
}

function CreepBombProjectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHitBomb(%a, %b, %c, %d, %e, %f, %g, 4);
    Parent::OnCollision(%a, %b, %c, %d, %e, %f, %g);
}

function CreepBomb2Projectile::OnCollision(%a, %b, %c, %d, %e, %f, %g)
{
	CreeperHitBomb(%a, %b, %c, %d, %e, %f, %g, 8);
    Parent::OnCollision(%a, %b, %c, %d, %e, %f, %g);
}

function CreeperHitBomb(%a, %b, %c, %d, %e, %f, %g, %rad) {
    if(!isobject(CreepGroup)) return;
    if($Creep::Active && $DefaultMiniGame == %b.sourceobject.client.minigame) {
        if(!isobject(CreepGroup) || !$Creep::active) return;
        
        initContainerRadiusSearch(%c.position, %rad, $TypeMasks::FxBrickAlwaysObjectType);
        while(%check = containerSearchNext()) {
            if(CreepGroup.isMember(%check)) {
                if(iseventpending(%check.regentick)) cancel(%check.regentick);
                %check.damage += (%rad*3)/vectorDist(%c.Position,%check.Position);
                if(%check.justforced) {
                    %check.justforced = 0;
                }
                %damlimit = ((15 - (CreepGroup.getcount()/1000)));
                if(%damlimit < 0) %damlimit = 0;
                %damlimit = (%damlimit + 5) * ($Creep::scalehp ? $DefaultMiniGame.nummembers : 1) * ($Creep::RoundType == 1 ? $Creep::HPFactor*3 : $Creep::HPFactor);
                %damlimit += mfloor(%damlimit * 1 * 0.2);
                
                if(%damlimit > $Creep::maxHPFac) {
                    %damlimit = $Creep::maxHPFac;
                }
                
                if(%check.damage >= %damlimit || $Creep::RoundType == 2) {
                    if($Creep::RoundType != 3) {
                        %check.delete();
                        %b.sourceobject.client.kills++;
                        //UpgradeMan.incValue(%b.sourceobject.client.bl_id,"totalCreepKills",1);
                    } else {
                        %check.damage = %damlimit;
                        if($Creep::regen) %check.regentick = scheduleNoQuota($Creep::regenms, 0, "BrickRegen", %check);
                    }
                } else {
                    SetCreepDmgColor(%check);
                    if($Creep::regen) %check.regentick = scheduleNoQuota($Creep::regenms, 0, "BrickRegen", %check);
                }
            }
        }
        
        if(isObject(%c)) {
            if(CreepGroup.isMember(%c)) {
                %c.delete();
            }
        }
    }
}

function CreeperHit(%image, %client, %hit) {
    if(!isobject(CreepGroup)) return;
	if(CreepGroup.ismember(%hit) && $Creep::Active && $DefaultMiniGame == %client.minigame) {
		if(iseventpending(%hit.regentick)) cancel(%hit.regentick);
		%hit.damage += %image.creepdamage;
		if(%hit.justforced) {
			%hit.justforced = 0;
		}
		%damlimit = ((15 - (CreepGroup.getcount()/1000)));
		if(%damlimit < 0) %damlimit = 0;
		%damlimit = (%damlimit + 5) * ($Creep::scalehp ? $DefaultMiniGame.nummembers : 1) * ($Creep::RoundType == 1 ? $Creep::HPFactor*3 : $Creep::HPFactor);
		%damlimit += mfloor(%damlimit * 1 * 0.2);
        
        if(%damlimit > $Creep::maxHPFac) {
            %damlimit = $Creep::maxHPFac;
        }
        
		if(%hit.damage >= %damlimit) {
            if($Creep::RoundType != 3) {
                %hit.delete();
                %client.kills++;
                //UpgradeMan.incValue(%b.sourceobject.client.bl_id,"totalCreepKills",1);
            } else {
                %hit.damage = %damlimit;
                if($Creep::regen) {
                	cancel(%hit.regentick);
                	%hit.regentick = scheduleNoQuota($Creep::regenms, 0, "BrickRegen", %hit);
                }
            }
		} else {
			SetCreepDmgColor(%hit);
			if($Creep::regen) {
				cancel(%hit.regentick);
				%hit.regentick = scheduleNoQuota($Creep::regenms, 0, "BrickRegen", %hit);
			}
		}
	}
}

package CreeperPackage
{	
	function gettrustlevel(%a, %b)
	{
		if(isobject(CreepGroup)) {
			if((creepgroup.ismember(%a) || creepgroup.ismember(%b)) && (getbrickgroupfromobject(%a).client.isadmin || getbrickgroupfromobject(%b).client.isadmin)) return 3;
		}
		parent::gettrustlevel(%a, %b);
	}
	
	function creeper::add(%this, %add, %count)
	{
		if(%this.getname() $= "CreepGroup" && isobject(%add)) {
			if(%add.getdatablock() $= "") {
				%count++;
				if(%count < 2) %this.scheduleNoQuota(50, "add", %add, %count);
				return;
			} else if(%add.getdatablock().getname() $= "brick1x1CreepSpawnData" || $Creep::Active) parent::add(%this, %add);
		} else parent::add(%this, %add);
	}
	
	function fxdtsbrick::ondeath(%a, %b, %c, %d, %e, %f, %g)
	{
		if(isobject(CreepGroup)) {
			if(CreepGroup.ismember(%a) && $Creep::Active) reviveNeighborCells(%a);
		}
		parent::ondeath(%a, %b, %c, %d, %e, %f, %g);
	}
};
activatepackage(CreeperPackage);

function StartCreeper()
{
    if($Creep::Clearing == 1) { // spawn bricks won't have reloaded yet
        echo("Waiting to start a new creep game...");
        scheduleNoQuota(1000,0,StartCreeper); // wait a second then try to start again
    }
    
    if(isObject(CreepGroup)) {
        if(!CreepGroup.getcount()) {
            messageAll('',"ERROR: Couldn't start a creeper minigame because there are no creeper spawns!");
            return;
        }
    } else {
        messageAll('',"ERROR: Couldn't start a creeper minigame because there is no CreepGroup object! Did you delete it manually? Why would you do that?");
        messageAll('',"(This error is also caused by no creeper spawns on new server. Check to see if that's the case.)");
        return;
    }
    
    // BEGIN
    $Creep::Started = 0;
    $Creep::GameStarted = 1;
    $Creep::active = 1;
    $CreepGrowTick = CreepGroup.startgrowth(%client);
}

function StopCreeper()
{
	if($Creep::active == 1)
	{
        // stop creeper from growing;
        // you can restart it with StartCreeper(); if you want, it'll just carry on in theory
		cancel($CreepGrowTick);
		$Creep::active = 0;
	}
}

function ClearCreeper() { // cleans up after a creeper game.
    if(!$Creep::GameStarted) {
        return;
    }
    
    $Creep::GameStarted = 0;
    $Creep::Clearing = 1;
    
    if(isObject(CreepGroup)) {
        CreepGroup.chaindeletecallback = "ClearCreeperPhase2();";
        CreepGroup.chaindeleteall();
    }
}

function ClearCreeperPhase2() {
    $Creep::Clearing = 0;
    
    // put the spawn bricks back!
	for(%i = 1; %i <= $Creep::SavedSpawns[index]; %i++) {
		%new = AttemptNewBrick("brick1x1CreepSpawnData", $Creep::SavedSpawns[%i], 0, $Creep::SavedSpawns[%i, color], $Creep::SavedSpawns[%i, blid]);
	}
}