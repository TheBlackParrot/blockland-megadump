$ChatLogFolder = "config/server/logs/chat";
if(!isObject($ChatLogFO)) {
	$ChatLogFO = new FileObject(ChatLogFO);
}

function sanitizeDate() {
	return strReplace(getWord(getDateTime(), 0), "/", "");
}

function getTime() {
	return getWord(getDateTime(), 1);
}

function logChat(%who, %msg) {
	%file = $ChatLogFO;
	%file.openForAppend($ChatLogFolder @ "/" @ sanitizeDate());

	%file.writeLine(getTime() TAB %who.bl_id TAB %who.name TAB %msg);

	%file.close();
}

package ChatLoggerPackage {
	function serverCmdMessageSent(%client, %msg) {
		logChat(%client, %msg);
		return parent::serverCmdMessageSent(%client, %msg);
	}

	function GameConnection::autoAdminCheck(%client) {
		logChat(%client, "** --> connected **");
		return parent::autoAdminCheck(%client);
	}

	function GameConnection::onClientLeaveGame(%client) {
		logChat(%client, "** disconnected --> **");
		return parent::onClientLeaveGame(%client);
	}
};
activatePackage(ChatLoggerPackage);