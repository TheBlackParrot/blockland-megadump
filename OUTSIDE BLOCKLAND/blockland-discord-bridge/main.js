const Discord = require('discord.js');
const net = require('net');
const emoji = require('node-emoji');
const request = require('request');
const table = require('cli-table2');
const stripansi = require('strip-ansi');

var settings = require('./settings.json');
function swap(json){
	var ret = {};
	for(var key in json){
		ret[json[key]] = key;
	}
	return ret;
}
settings.channels.ports = swap(settings.channels.bridge);
settings.channels.ports_voice = swap(settings.channels.voice);

var bot = new Discord.Client();
bot.login(settings.discord_token);

bot.on('ready', function(event) {
	console.log('Logged in as %s - %s\n', bot.user.username, bot.user.id);
});

var TCPclients = [];
var player_tab = new table({
	head: ['', 'Name', 'BL_ID', 'On For'],
	chars: { 'top': '' , 'top-mid': '' , 'top-left': '' , 'top-right': ''
	, 'bottom': '' , 'bottom-mid': '' , 'bottom-left': '' , 'bottom-right': ''
	, 'left': '' , 'left-mid': '' , 'mid': '' , 'mid-mid': ''
	, 'right': '' , 'right-mid': '' , 'middle': ' ' },
	style: { 'padding-left': 1, 'padding-right': 1 }
});

function parseMentions(channel, str) {
	var mention_str = "";
	for(var x in settings.staff_roles) {
		var staffRoleObject = channel.guild.roles.find('name', settings.staff_roles[x]);
		mention_str += staffRoleObject + " ";
	}

	return str.replace(new RegExp('@staff', 'g'), mention_str.trim());
}

function writeGlobal(data) {
	for(var x in TCPclients) {
		var socket = TCPclients[x];
		socket.write(data);
	}
}

net.createServer(function(socket) {
	socket.name = socket.remoteAddress + ":" + socket.remotePort;
	TCPclients.push(socket);

	socket.write("Connected.\r\n");

	socket.on('data', function (data) {
		var parts = data.toString().split("\t").map(function(part) {
			return part.trim();
		});

		if(!(data.toString().trim())) {
			return;
		}

		var cmd = parts[0];
		var serv = parts[1];
		var port = parts[2];

		socket.lastKnownServ = serv;
		if(typeof socket.lastKnownPort === "undefined") {
			socket.lastKnownPort = port;
		}

		var channel = bot.channels.get(settings.channels.bridge[port.toString()]);
		if(typeof channel === "undefined") {
			console.log("channel returned as undefined with port " + port + ", stopping here");
			return;
		}

		switch(cmd) {
			case "new":
				var msg_send = ":white_check_mark: **" + serv + "** has come online.";
				break;

			case "chat":
				var who = parts[3];
				var msg = parts[4];
				var msg_send = "**" + who + "** *on " + serv + "*\n" + parseMentions(channel, msg);
				break;

			case "connect":
				if(typeof parts[3] !== "undefined") {
					var who = parts[3];
					var msg_send = ":new: **" + who + "** joined " + serv;
				}
				break;

			case "disconnect":
				if(typeof parts[3] !== "undefined") {
					var who = parts[3];
					var msg_send = ":runner: **" + who + "** left " + serv;
				}
				break;

			case "cmd_ok":
				var msg_send = ":ok_hand: Success";
				break;

			case "cmd_failed":
				if(typeof parts[3] !== "undefined") {
					var reason = parts[3];
					var msg_send = ":warning: Command did not work: `" + reason + "`";
				} else {
					var msg_send = ":warning: Command did not work";
				}
				break;

			case "topic":
				channel.setTopic(":busts_in_silhouette: " + parts[3] + " / " + parts[4] + " players online      :stopwatch: running for " + parts[5]);
				break;

			case "players_start":
				channel.startTyping();
				player_tab.length = 0;
				break;

			case "players_end":
				player_tab.sort(function(a, b) {
					return a[0] - b[0];
				});

				channel.stopTyping();
				channel.send('```' + stripansi(player_tab.toString()) + '```');
				break;

			case "players_line":
				player_tab.push([parts[3], parts[4], parts[5], parts[6]]);
				break;
		}

		//channel.send(msg_send.replace(/\@/g, "(at)"));
		if(typeof msg_send !== "undefined") {
			channel.send(msg_send);
		}
	});

	socket.on('end', function () {
		var channel = bot.channels.get(settings.channels.bridge[socket.lastKnownPort.toString()]);
		channel.send(":warning: **" + socket.lastKnownServ + "** may have just went down.");

		TCPclients.splice(TCPclients.indexOf(socket), 1);
	});

	function broadcast(message, sender) {
		TCPclients.forEach(function(client) {
			if(client === sender) {
				return;
			}
			client.write(message);
		});
		process.stdout.write(message)
	}
}).listen(59999, "127.0.0.1");

function getConnectedBLServer(channel) {
	for(var x in TCPclients) {
		var socket = TCPclients[x];
		if(socket.lastKnownPort == settings.channels.ports[channel.id]) {
			return socket;
		}
	}
}

//bot.on('message', function(user, userID, channelID, message, event) {
bot.on('message', function(message) {
	var parts = message.content.split(" ");
	var name = message.author.username + "#" + message.author.discriminator;

	var global = (message.channel.id == settings.channels.global);

	switch(parts[0]) {
		case "!servers":
			request("http://master2.blockland.us", function(err, res, body) {
				var tab = new table({
					head: ['Pwd', 'Name', '#', 'of', 'Plyrs', 'Gamemode'],
					chars: { 'top': '' , 'top-mid': '' , 'top-left': '' , 'top-right': ''
					, 'bottom': '' , 'bottom-mid': '' , 'bottom-left': '' , 'bottom-right': ''
					, 'left': '' , 'left-mid': '' , 'mid': '' , 'mid-mid': ''
					, 'right': '' , 'right-mid': '' , 'middle': ' ' },
					style: { 'padding-left': 1, 'padding-right': 1 }
				});

				body.split("\n").map(function(line_raw) {
					var line = line_raw.trim();
					if(!isNaN(parseInt(line.charAt(0)))) {
						var parts = line.split("\t");

						var locked = (parseInt(parts[2]) ? "P" : "");
						var name = parts[4];
						var players = parseInt(parts[5]);
						var maxplayers = parseInt(parts[6]);
						var gamemode = parts[7];

						tab.push(
							[locked, name, players, "/", maxplayers, gamemode]
						);
					}
				});
				
				tab.sort(function(a, b) {
					return b[2] - a[2];
				});
				tab.splice(10);

				message.channel.send('```' + stripansi(tab.toString()) + '```');
			});
			return;
			break;

		case "!kick":
		case "!kill":
			var permission = message.guild.member(message).hasPermission("KICK_MEMBERS");
			if(!permission) {
				return;
			}
			
			if(global) {
				writeGlobal(parts[0].replace("!", "") + "\t" + name + "\t" + parts.slice(1).join(" ") + "\r\n");
				return;
			}

			var socket = getConnectedBLServer(message.channel);
			if(typeof socket === "undefined") {
				message.channel.send(":warning: This channel is not connected to a Blockland server.");
			} else {
				socket.write(parts[0].replace("!", "") + "\t" + name + "\t" + parts.slice(1).join(" ") + "\r\n");
			}
			return;
			break;

		case "!players":
			var socket = getConnectedBLServer(message.channel);
			if(typeof socket === "undefined") {
				message.channel.send(":warning: This channel is not connected to a Blockland server.");
			} else {
				socket.write("players" + "\t" + name + "\r\n");
			}
			return;
			break;

		case "!reload":
			var permission = message.guild.member(message).hasPermission("MANAGE_GUILD");
			if(!permission) {
				return;
			}

			if(global) {
				writeGlobal(parts[0].replace("!", "") + "\t" + name + "\t" + parts.slice(1).join(" ") + "\r\n");
				return;
			}

			var socket = getConnectedBLServer(message.channel);
			if(typeof socket === "undefined") {
				message.channel.send(":warning: This channel is not connected to a Blockland server.");
			} else {
				socket.write("reload" + "\t" + name + "\r\n");
			}

			return;
			break;

		case "!ban":
			var permission = message.guild.member(message).hasPermission("BAN_MEMBERS");
			if(!permission) {
				return;
			}

			if(parts.length < 3) {
				message.channel.send("[bl_id] [time or -1] [reason]");
				return;
			}

			var bl_id = parts[1];
			var time = parts[2];

			if(parts.length == 3) {
				var reason = "Remotely banned";
			} else {
				var reason = parts[3];
			}
			
			if(global) {
				writeGlobal(parts[0].replace("!", "") + "\t" + name + "\t" + [bl_id, time, reason].join("\t") + "\r\n");
				return;
			}

			var socket = getConnectedBLServer(message.channel);
			if(typeof socket === "undefined") {
				message.channel.send(":warning: This channel is not connected to a Blockland server.");
			} else {
				socket.write(parts[0].replace("!", "") + "\t" + name + "\t" + [bl_id, time, reason].join("\t") + "\r\n");
			}
			return;
			break;
	}

	if((Object.values(settings.channels.bridge).indexOf(message.channel.id) != -1 || message.channel.id == settings.channels.global) && message.author.id != bot.user.id) {
		if(message.content.trim()) {
			if(global) {
				writeGlobal("msg\t" + name + "\t" + emoji.unemojify(message.cleanContent).replace("\n", " ") + "\tglobal\r\n");
				return;
			}

			for(var x in TCPclients) {
				var socket = TCPclients[x];
				if(socket.lastKnownPort == settings.channels.ports[message.channel.id] || global) {
					socket.write("msg\t" + name + "\t" + emoji.unemojify(message.cleanContent).replace("\n", " ") + "\r\n");
					break;
				}
			}
		}
	}
});

function getConnectedBLServerVoice(member) {
	for(var x in TCPclients) {
		var socket = TCPclients[x];
		if(socket.lastKnownPort == settings.channels.ports_voice[member.voiceChannelID]) {
			return socket;
		}
	}
}

bot.on('voiceStateUpdate', function(oldMember, newMember) {
	var newChannel = newMember.voiceChannel;
	var oldChannel = oldMember.voiceChannel;

	var name = newMember.user.username + "#" + newMember.user.discriminator;
	var state = undefined;

	if(typeof oldChannel === "undefined" && typeof newChannel !== "undefined") {
		var n = getConnectedBLServerVoice(newMember);
		n.write("voice\t" + name + "\tjoin\r\n");
	} else if(typeof oldChannel !== "undefined" && typeof newChannel === "undefined") {
		var o = getConnectedBLServerVoice(oldMember);
		o.write("voice\t" + name + "\tleave\r\n");
	} else if(typeof oldChannel !== "undefined" && typeof newChannel !== "undefined") {
		if(oldMember.voiceChannelID != newMember.voiceChannelID) {
			var n = getConnectedBLServerVoice(newMember);
			n.write("voice\t" + name + "\tjoin\r\n");
			var o = getConnectedBLServerVoice(oldMember);
			o.write("voice\t" + name + "\tleave\r\n");
		}
	}
});