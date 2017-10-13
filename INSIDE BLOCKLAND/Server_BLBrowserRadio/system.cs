function messageAllWithRadio(%tag, %msg) {
	for(%i=0;%i<ClientGroup.getCount();%i++) {
		%client = ClientGroup.getObject(%i);
		if(%client.canUtilizeRadio) {
			messageClient(%client, %tag, %msg);
		}
	}
}

function refreshRadioQueue() {
	if(isObject(RadioListQueue)) {
		RadioListQueue.clear();
	} else {
		new GuiTextListCtrl(RadioListQueue);
	}

	%list = RadioListQueue;
	%count = $Server::BLBrowserRadio::Count;

	for(%i=0;%i<%count;%i++) {
		%row_str = $Server::BLBrowserRadio::Title[%i];
		%row_str = %row_str TAB $Server::BLBrowserRadio::Artist[%i];
		%row_str = %row_str TAB $Server::BLBrowserRadio::Duration[%i];
		%row_str = %row_str TAB $Server::BLBrowserRadio::Path[%i];

		%list.addRow(%i, trim(%row_str) TAB getRandom(-999999, 999999));
	}

	%list.sortNumerical(4, getRandom(0, 1));
}

function radioStep() {
	cancel($Server::BLBrowserRadio::Sched);

	if(!isObject(RadioListQueue)) {
		new GuiTextListCtrl(RadioListQueue);
	}

	%list = RadioListQueue;

	if(%list.rowCount() == 0) {
		refreshRadioQueue();
	}

	%next = $Server::BLBrowser::CurrentlyPlaying = %list.getRowText(0);
	%list.removeRow(0);

	%title = getField(%next, 0);
	%artist = getField(%next, 1);
	%duration = getField(%next, 2);
	%path = getField(%next, 3);

	for(%i=0;%i<ClientGroup.getCount();%i++) {
		%client = ClientGroup.getObject(%i);
		if(%client.hearRadio) {
			commandToClient(%client, 'AWS_LoadUrl', $Pref::Server::BLBrowserRadio::AWS_URL @ "/" @ %path);
		}
	}
	messageAllWithRadio('', "\c2Now Playing:\c6" SPC %artist SPC "-" SPC %title SPC "\c7[" @ getTimeString(mCeil(%duration/1000)) @ "]");

	$Server::BLBrowserRadio::Sched = schedule(%duration + 2000, 0, radioStep);

	$Server::BLBrowserRadio::StartedAt = $Sim::Time;
}