$Pictionary::GeneratePlurals = true;
$Pictionary::DatabaseFilenames = "Add-Ons/Gamemode_Pictionary/words.txt\tconfig/server/Pictionary/words.txt";

if(!isObject(PictionaryWordDB)) {
	dbg("Creating word database...", "words.cs");
	new ScriptGroup(PictionaryWordDB) {
		ignore = "";
	};
}

if(!isObject(PictionaryWordMemory)) {
	dbg("Creating word memory...", "words.cs");
	// before you ask, yes, i'm out of my mind
	new GuiTextListCtrl(PictionaryWordMemory);
}

// attempted to port from my phrase generator
// which is in python
// holy hell kms
// -- https://github.com/TheBlackParrot/getaphrase-rewrite/blob/d69637578ea743c594ab3b344a522c5109ff0c88/generator.py#L72
function pluralize(%word) {
	%last_2 = getSubStr(%word, strLen(%word)-2, 2);
	%last = getSubStr(%last_2, 1, 1);

	if(%last_2 !$= "ch" && %last_2 !$= "sh") {
		if(stripos("oxs", %last) != -1) {
			return %word @ "es";
		} else if(%last $= "y") {
			if(stripos("aeiou", getSubStr(%last_2, 0, 1)) != -1) {
				return %word @ "s";
			} else {
				return getSubStr(%word, 0, strLen(%word)-1) @ "ies";
			}
		} else {
			return %word @ "s";
		}
	} else {
		return %word @ "es";
	}

	return %word;
}

function PictionaryWordDB::loadDB(%this) {
	PictionaryWordDB.deleteAll();
	PictionaryWordMemory.clear();

	%count = 0;
	for(%i=0;%i<getFieldCount($Pictionary::DatabaseFilenames);%i++) {
		%filename = getField($Pictionary::DatabaseFilenames, %i);
		dbg("Loading word database" SPC %filename, "words.cs");

		%file = new FileObject();
		%file.openForRead(%filename);

		while(!%file.isEOF()) {
			%line = %file.readLine();

			%plurals = "";
			for(%j=0;%j<getFieldCount(%line);%j++) {
				%plurals = trim(%plurals TAB pluralize(getField(%line, %j)));
			}

			%entry = new ScriptObject(PictionaryWord) {
				words = %line;
				count = getFieldCount(%line);
				plurals = %plurals;
				nospaces = strReplace(%plurals, " ", "") TAB strReplace(%line, " ", "");
			};
			PictionaryWordDB.add(%entry);

			// %count = id
			// %word, object ID reference, random number to shuffle with
			PictionaryWordMemory.addRow(%count, getField(%line, 0) TAB %entry.getID() TAB getRandom(-999999, 999999));

			%count++;
		}
	}

	PictionaryWordMemory.sortNumerical(2, 0);

	dbg(PictionaryWordDB.getCount() SPC "words are in the database.", "words.cs");
}

function PictionaryWordDB::getNextEntry(%this) {
	if(!%this.getCount()) {
		%this.loadDB();
	}

	%list = PictionaryWordMemory;
	%next = %list.getRowID(0);
	
	%list.removeRow(0);
	%list.sortNumerical(2, getRandom(0, 1));

	if(!%list.rowCount()) {
		dbg("Reshuffling the word database...", "words.cs");

		%count = 0;

		for(%i=0;%i<%this.getCount();%i++) {
			%entry = %this.getObject(%i);

			PictionaryWordMemory.addRow(%count, getField(%entry.words, 0) TAB %entry.getID() TAB getRandom(-999999, 999999));

			%count++;
		}
	}

	dbg("Returned index" SPC %next SPC "as the next entry", "words.cs", 2);

	return %this.getObject(%next);
}