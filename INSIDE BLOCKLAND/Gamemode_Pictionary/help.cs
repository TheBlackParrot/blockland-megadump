function cacheHelp() {
	$Pictionary::HelpLines = 0;

	%file = new FileObject();
	%file.openForRead("Add-Ons/Gamemode_Pictionary/help.txt");

	while(!%file.isEOF()) {
		$Pictionary::HelpLine[$Pictionary::HelpLines] = %file.readLine();

		$Pictionary::HelpLines++;
	}
}
cacheHelp();

function serverCmdHelp(%client) {
	for(%i=0;%i<$Pictionary::HelpLines;%i++) {
		%line = $Pictionary::HelpLine[%i];
		%tag = getSubStr(%line, 0, 3);

		switch$(%tag) {
			case "**S":
				if(%client.isSuperAdmin) {
					messageClient(%client, '', getSubStr(%line, 3, strLen(%line)));
				}

			case "**A":
				if(%client.isSuperAdmin) {
					messageClient(%client, '', getSubStr(%line, 3, strLen(%line)));
				}

			default:
				messageClient(%client, '', %line);
		}
	}
}