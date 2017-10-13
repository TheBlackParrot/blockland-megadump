function setCookie(cname, cvalue, exdays) {
	var d = new Date();
	d.setTime(d.getTime() + (exdays*24*60*60*1000));
	var expires = "expires="+d.toUTCString();
	document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
	var name = cname + "=";
	var ca = document.cookie.split(';');
	for(var i=0; i<ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1);
		if (c.indexOf(name) == 0) return c.substring(name.length,c.length);
	}
	return "";
}

var server_data;
$.get('includes/getdata.php?server=1', function(data) {
	server_data = data;
	getServerLines(data);
	$(".server_list").tablesorter({
		sortList: [[3,1]],
		sortReset: true,
		sortRestart: true,
		debug: false
	});
});

/*
var addresses = [];
function testPings(data) {
	var servers = data.split("\n");
	for(i in servers) {
		var row = servers[i].split("\t");

		var validTest = parseInt(row[0]).toString();
		if(validTest == "NaN") {
			continue;
		}
		
		address = row[0] + ":" + row[1];
		console.log(address);
		addresses.push(address);
	}
	pingAddresses(0);
}

function pingAddresses(index) {
	if(typeof addresses[index] === "undefined") {
		return "end";
	}
	ping(addresses[index]).then(function(delta){
		console.log(addresses[index] + ": " + delta + "ms");
		pingAddresses(index + 1);
	}).catch(function(error) {
		console.log(String(error));
	});
}
*/

var ver = 21;

function getServerLines(data) {
	var servers = data.split("\n");
	var total_players = 0;
	var total_servers = 0;

	$(".server_list").empty();
	$(".server_list").append($(initial_table));

	var table;
	for(i in servers) {
		var row = servers[i].split("\t");

		var validTest = parseInt(row[0]).toString();
		if(validTest == "NaN") {
			continue;
		}
		total_servers++;
		
		address = row[0] + ":" + row[1];
		var pass = row[2];
		var ded = row[3];
		var name = row[4];
		var players = row[5];
		total_players += parseInt(players);
		var max_players = row[6];
		var gamemode = row[7];
		var bricks = row[8];
		var now = Date.now();

		var element = $("<tr></tr>");
		element.attr("ip", row[0]);
		element.attr("port", row[1]);
		element.attr("timestamp", now);
		element.append("<td>" + (parseInt(pass) ? "Yes" : "No") + "</td>");
		element.append("<td>" + (parseInt(ded) ? "Yes" : "No") + "</td>");
		element.append("<td>" + name + "</td>");
		element.append("<td>" + players + "</td>");
		element.append("<td>/</td>");
		element.append("<td>" + max_players + "</td>");
		element.append("<td>" + bricks + "</td>");
		element.append("<td>" + gamemode + "</td>");
		element.addClass("server");

		if(players == max_players) {
			element.css("color", "#2196F3");
			element.addClass("full");
		}
		if(parseInt(pass)) {
			element.css("color", "#FFC107");
			element.addClass("passworded");
		}

		$(".server_list").append(element);
	}

	$("#total").remove();
	var element2 = $('<span id="total"></span>');
	element2.text(total_servers + " servers :: " + total_players + " players");
	$(".wrapper").append(element2);

	if(total_servers) {
		$('.server_list').trigger('updateCache');
		$('.server_list').trigger('updateAll', [true, function(){
			$('.server_list').trigger('sorton', [[[3,"d"]]]); // this is legitimately in the documentation
			// why the HELL ARE WE GOING THIS DEEP INTO AN ARRAY
		}]);
		// tablesorter is probably one of the worst jQuery plugins I've ever had to deal with
	}
	
	var rect = $("#table_fix")[0].getBoundingClientRect();
	$("#tables_are_the_worst").css("top", rect.top + "px").css("left", rect.left + "px");
}