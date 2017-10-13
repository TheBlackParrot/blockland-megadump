$CrumblingArena::Root = "Add-Ons/Gamemode_Crumbling_Arena";
if($Pref::Server::CrumblingArena::AllowedBricks $= "") {
	$Pref::Server::CrumblingArena::AllowedBricks = "2x2 2x4 2x6 2x8 4x4 4x8 8x8 2x2f 2x4f 2x6f 2x8f 4x4f 4x8f 8x8f 1x1 1x1f 4x4Cube";
}

function dbg(%msg, %file, %func, %parent_obj, %lvl, %echo_only) {
	if(!$CrumblingArena::Debug) {
		return;
	}

	if(%lvl $= "" || %lvl <= 0) {
		%lvl = 1;
	}

	if(%lvl > $CrumblingArena::DebugLevel) {
		return;
	}

	%time_str = "\c7[" @ mFloatLength($Sim::Time, 2) @ "]";
	%file_str = "\c7[\c5" @ %file;
	if(%func !$= "") {
		if(%parent_obj !$= "") {
			if(%parent_obj.class $= "") {
				%class = %parent_obj.getClassName();
			} else {
				%class = %parent_obj.class;
			}
			%func_str = "\c7/\c3" @ %class @ "\c6-->\c4" @ %func @ "\c7]";
		} else {
			%func_str = "\c7/\c4" @ %func @ "\c7]";
		}
	} else {
		 %file_str = %file_str @ "\c7] ";
	}

	if(isObject(%parent_obj)) {
		%obj_str = " \c7[\c0OBJ" SPC %parent_obj @ "\c7]";
	}

	echo(stripMLControlChars(%time_str SPC %file_str @ %func_str SPC %msg @ %obj_str));
	if(!%echo_only || %echo_only $= "") {
		messageAll('', %time_str SPC %file_str @ %func_str SPC "\c6" @ strReplace(%msg, "\t", "\c1^\c6") @ %obj_str);
	}
}

exec("./sounds.cs");
exec("./gradients.cs");
exec("./support.cs");
exec("./board.cs");
exec("./cheat_prevention.cs");
exec("./system.cs");
exec("./interaction.cs");
exec("./commands.cs");
exec("./env.cs");

deactivatePackage(CrumblingArenaServerPackage);
package CrumblingArenaServerPackage {
	function onServerDestroyed() {
		deleteVariables("$CrumblingArena::*");
		parent::onServerDestroyed();
	}
};
activatePackage(CrumblingArenaServerPackage);