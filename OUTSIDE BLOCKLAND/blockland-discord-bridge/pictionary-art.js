const chokidar = require('chokidar');
const easyimage = require('easyimage');
const Discord = require('discord.io');

var settings = require('./settings.json');

var bot = new Discord.Client({
	autorun: true,
	token: settings.discord_token
});

bot.on('ready', function(event) {
	console.log('Logged in as %s - %s\n', bot.username, bot.id);
});

var watcher = chokidar.watch(settings.art_directory, {
	ignored: /[\/\\]\./,
	persistent: true,
	awaitWriteFinish: {
		stabilityThreshold: 2000,
		pollInterval: 100
	}
});

var ready = false;

watcher
	.on('add', function(path) {
		if(!ready) {
			return;
		}

		easyimage.resize({
			src: path,
			dst: "/tmp/pictionary-discord.png",
			width: 476,
			height: 164
		}).then(
			function(image) {
				bot.uploadFile({
					to: settings.channels.pictionary,
					file: "/tmp/pictionary-discord.png"
				});
			},
			function(err) {
				console.log(err);
			}			
		);
	})
	.on('error', function(error) { 
		console.log('Error happened', error);
	})
	.on('ready', function() {
		ready = true;
		console.log("Directory watcher ready.");
	});